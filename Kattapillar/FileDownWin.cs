using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using VShawnEpub.FileDown;

namespace Kattapillar
{
    public partial class FileDownWin : Form
    {
        /// <summary>
        /// 同时下载数
        /// </summary>
        private int clintCount = 2;
        //临时下载目录
        private string tempPath = Application.StartupPath + "\\temp\\";
        //下载控件
        List<WebClient> listwWebClients = new List<WebClient>();
        //当前下载文件
        private List<FileDown> listCurFile = new List<FileDown>();
        //下载完成数
        private int finishFileCount = 0;
        public FileDownWin()
        {
            InitializeComponent();
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
        }

        private void Add_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "")
            {
                return;
            }

            FileDown file = FileDownDao.AddDownFile(this.textBox1.Text);
            if (file == null)
            {
                return;
            }
            ShowFile(file);
            StartDown();
            this.textBox1.Text = "";
            this.textBox1.Focus();
        }
        private void FileDown_Load(object sender, EventArgs e)
        {
            this.listFileDown.Columns.Add("名称");
            this.listFileDown.Columns.Add("状态");
            this.listFileDown.Columns.Add("大小");
            this.listFileDown.Columns.Add("URL");
            this.listFileDown.Columns.Add("文件路径");
            this.listFileDown.Columns.Add("备注");
            ResetColumnWidth();

            for (int i = 0; i < clintCount; i++)
            {
                listwWebClients.Add(new WebClient());
                listwWebClients[i].DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                listwWebClients[i].DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                FileDown curFile = null;
                listCurFile.Add(curFile);
            }

            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
        }
        #region 下载事件
        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            int index = -1;
            WebClient MyWebClient = (WebClient)sender;
            bool allWBIsBusy = true;
            for (int i = 0; i < listwWebClients.Count; i++)
            {
                if (MyWebClient == listwWebClients[i])
                {
                    index = i;
                    break;
                }
            }
            if (index == -1)
            {
                return;
            }
            FileDown file = listCurFile[index];


            if (e.Cancelled)
            {
                return;
            }
            if (e.Error != null)
            {
                file.Status = FileDown.FileDownStatus.Error;
                file.ErrorCount++;
                file.ErrorMsg = e.Error.Message;
            }
            //从临时文件夹中转移出文件
            if (file.Status == FileDown.FileDownStatus.Doing)
            {
                string fileTempPath = tempPath + file.Id + ".tmp";
                File.Copy(fileTempPath, file.SavePath, true);
                File.Delete(fileTempPath);
                file.EndTime = DateTime.Now;
                file.UseTime = DateTime.Now - file.BeginTime;
                file.Status = FileDown.FileDownStatus.Finish;
            }
            UpdateListItem(file.Id);
            if (file.Status == FileDown.FileDownStatus.Finish)
            {
                finishFileCount++;
                //FileDownDao.FileDownList.Remove(file);
            }
            StartDown();
        }
        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int index = -1;
            WebClient MyWebClient = (WebClient)sender;
            bool allWBIsBusy = true;
            for (int i = 0; i < listwWebClients.Count; i++)
            {
                if (MyWebClient == listwWebClients[i])
                {
                    index = i;
                    break;
                }
            }
            if (index == -1)
            {
                return;
            }
            FileDown file = listCurFile[index];
            ListViewItem item = GetListItem(file.Id);
            file.FileSize = e.TotalBytesToReceive;
            item.SubItems[1].Text = e.ProgressPercentage.ToString() + "%";
            item.SubItems[2].Text = UtilTool.FormatSize(file.FileSize);
        }
        #endregion

        #region 辅助
        private void ShowFile(FileDown file)
        {
            ListViewItem item = new ListViewItem();
            item.Text = file.FileName;
            item.SubItems.Add(FileDown.GetFileDownStatusName(file.Status));
            item.SubItems.Add(UtilTool.FormatSize(file.FileSize));
            item.SubItems.Add(file.Url);
            item.SubItems.Add(file.SavePath);
            item.SubItems.Add(file.GetMemo());
            item.Tag = file.Id;
            item.ForeColor = GetItemColor(file.Status);

            this.listFileDown.Items.Add(item);
            ResetColumnWidth();
        }

        private void StartDown()
        {
            List<int> freeWCIndex = new List<int>();
            for (int i = 0; i < listwWebClients.Count; i++)
            {
                if (!listwWebClients[i].IsBusy)
                    freeWCIndex.Add(i);
            }
            if (freeWCIndex.Count == 0)
                return;


            string tempPath = Application.StartupPath + "\\temp\\";
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }


            IList<FileDown> oldFileDown = new List<FileDown>();
            foreach (int i in freeWCIndex)
            {
                foreach (FileDown r in FileDownDao.FileDownList)
                {
                    if (r.Status == FileDown.FileDownStatus.Wait)
                    {
                        if (File.Exists(r.SavePath))
                        {
                            oldFileDown.Add(r);
                            continue;
                        }
                        FileInfo fileInfo = new FileInfo(r.SavePath);
                        if (!fileInfo.Directory.Exists)
                        {
                            try
                            {
                                fileInfo.Directory.Create();
                            }
                            catch (Exception ex)
                            {
                                r.Status = FileDown.FileDownStatus.Error;
                                r.ErrorCount++;
                                r.ErrorMsg = "创建文件路径失败！" + ex.Message;

                                //if (r.ErrorCount == 1)
                                //{
                                //    AddLog(r);
                                //}
                                UpdateListItem(r.Id);
                                continue;
                            }
                        }
                        r.Status = FileDown.FileDownStatus.Doing;
                        r.BeginTime = DateTime.Now;
                        listwWebClients[i].DownloadFileAsync(new Uri(r.Url), tempPath + r.Id + ".tmp");
                        UpdateListItem(r.Id);
                        listCurFile[i] = r;
                        break;
                    }
                }
            }

            //处理已下载的文件
            foreach (FileDown old in oldFileDown)
            {
                old.Status = FileDown.FileDownStatus.Error;
                old.ErrorMsg = "原文件已存在";
                old.Status = FileDown.FileDownStatus.Exixted;
                ListViewItem item = GetListItem(old.Id);
                UpdateListItem(old.Id);
                //FileDownDao.FileDownList.Remove(old);
            }

            ResetColumnWidth();
        }

        private void UpdateListItem(string id)
        {
            ListViewItem item = GetListItem(id);
            if (item == null)
            {
                return;
            }
            FileDown file = null;
            foreach (FileDown r in FileDownDao.FileDownList)
            {
                if (r.Id == id)
                {
                    file = r;
                    break;
                }
            }
            if (file == null) return;
            //if (file.Status == FileDown.FileDownStatus.Finish)
            //{
            //    item.Remove();
            //}
            item.SubItems[1].Text = FileDown.GetFileDownStatusName(file.Status);
            item.SubItems[5].Text = file.GetMemo();
            item.ForeColor = GetItemColor(file.Status);
            ResetColumnWidth();
        }

        /// <summary>
        /// 获取列表项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private ListViewItem GetListItem(string id)
        {
            ListViewItem item = null;
            foreach (ListViewItem r in this.listFileDown.Items)
            {
                if (r.Tag.ToString() == id)
                {
                    item = r;
                    break;
                }
            }
            return item;
        }


        /// <summary>
        /// 重置列宽
        /// </summary>
        private void ResetColumnWidth()
        {
            if (this.listFileDown.Items.Count == 0)
            {
                foreach (ColumnHeader r in this.listFileDown.Columns)
                {
                    r.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                }
            }
            else
            {
                foreach (ColumnHeader r in this.listFileDown.Columns)
                {
                    r.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                }
            }

            this.listFileDown.Columns[1].Width = 80;
            this.listFileDown.Columns[2].Width = 60;

            ColumnHeader lastCol = this.listFileDown.Columns[this.listFileDown.Columns.Count - 1];
            if (lastCol.Width < 20)
            {
                lastCol.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            }

        }

        /// <summary>
        /// 获取颜色
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private Color GetItemColor(FileDown.FileDownStatus status)
        {
            Color color = Color.Black;
            switch (status)
            {
                case FileDown.FileDownStatus.Wait:
                    color = Color.Black;
                    break;
                case FileDown.FileDownStatus.Doing:
                    color = Color.Blue;
                    break;
                case FileDown.FileDownStatus.Error:
                    color = Color.Red;
                    break;
                case FileDown.FileDownStatus.Finish:
                    color = Color.Green;
                    break;
                case FileDown.FileDownStatus.Exixted:
                    color = Color.DarkOliveGreen;
                    break;
                default:
                    break;
            }

            return color;
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            FileDown file1 = FileDownDao.AddDownFile("http://www.lightnovel.cn/data/attachment/forum/201506/24/085353hvy7aldvvlf72lng.jpg");
            FileDown file2 = FileDownDao.AddDownFile("http://www.lightnovel.cn/data/attachment/forum/201506/24/085420f4a1kjstn7afa6wk.jpg");
            FileDown file3 = FileDownDao.AddDownFile("http://www.lightnovel.cn/data/attachment/forum/201507/02/120427xsnnjsdkiuzknn7r.jpg");
            FileDown file4 = FileDownDao.AddDownFile("http://www.lightnovel.cn/data/attachment/forum/201507/02/120424wqstddodgq8mg3dd.jpg");
            FileDown file5 = FileDownDao.AddDownFile("http://www.lightnovel.cn/data/attachment/forum/201507/02/120425ggwbg9bqgeuu3jbz.jpg");
            ShowFile(file1);
            ShowFile(file2);
            ShowFile(file3);
            ShowFile(file4);
            ShowFile(file5);
            StartDown();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            List<int> busyWCIndex = new List<int>();
            int index = 0;
            for (int i = 0; i < listwWebClients.Count; i++)
            {
                if (listwWebClients[i].IsBusy)
                {
                    busyWCIndex.Add(i);
                    break;
                }
            }
            if (e.CloseReason == CloseReason.UserClosing && busyWCIndex.Count > 0)
            {
                DialogResult msgBox = MessageBox.Show("有任务正在下载，您确认要终止并退出吗？", "退出确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (msgBox == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            foreach (int i in busyWCIndex)
            {
                listwWebClients[i].CancelAsync();
            }

            for (int i = 0; i < listwWebClients.Count; i++)
            {
                listwWebClients[i] = null;
            }
        }
    }
}
