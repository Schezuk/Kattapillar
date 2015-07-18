using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CCWin;
using VShawnEpub;
using VShawnEpub.Wenku;

namespace Kattapillar
{
    public partial class Form1 : CCSkinMain
    {
        private VShawnEpub.Discuz.LKDiscuzBook lkdb;
        private VShawnEpub.Wenku.LKWenKuBook lkwkb; 
        
        public Form1()
        {
            InitializeComponent();
        }
        private void skinButton1_Click(object sender, EventArgs e)
        {
            //skinButton1.Enabled = false;
            if (tbUrl.Text.IndexOf("www.lightnovel.cn/thread") > 0 || tbUrl.Text.IndexOf("www.lightnovel.cn/forum.php?mod=viewthread") > 0)
            {
                lkdb = new VShawnEpub.Discuz.LKDiscuzBook();
                lkwkb = null;

                lkdb.Browsers.Add(webBrowser1);
                string url = VShawnEpub.Discuz.LKDiscuzBook.GetTrueUrl(tbUrl.Text);
                lkdb.Browsers[lkdb.BroswersLodedIndex].Navigate(url);
                lkdb.Browsers[lkdb.BroswersLodedIndex].DocumentCompleted +=
                    delegate(object o, WebBrowserDocumentCompletedEventArgs args)
                    {
                        //预加载帖子第二页
                        if (lkdb.IsBroswerOK(lkdb.Browsers[lkdb.BroswersLodedIndex]) && lkdb.Browsers.Count <= 2)
                        {
                            lkdb.BroswersLodedIndex++;
                            WebBrowser wb = new WebBrowser();
                            wb.Navigate(url + (lkdb.Browsers.Count + 1));
                            lkdb.Browsers.Add(wb);
                        }
                    };
            }
            else if (tbUrl.Text.IndexOf("lknovel.lightnovel.cn/main/book/") > 0)
            {
                lkdb = null;
                lkwkb = new LKWenKuBook();

                webBrowser1.Navigate(tbUrl.Text);
                webBrowser1.DocumentCompleted += delegate(object o, WebBrowserDocumentCompletedEventArgs args)
                {
                    if(lkwkb.IsBroswerOK(webBrowser1) && lkwkb.Epub.Title == "")
                        lkwkb.ProcessMainPage(webBrowser1.Url.AbsoluteUri, webBrowser1.DocumentText);
                };
            }
        }

        private void skinButton2_Click(object sender, EventArgs e)
        {
            //LK论坛
            if (tbUrl.Text.IndexOf("www.lightnovel.cn/thread") > 0 || tbUrl.Text.IndexOf("www.lightnovel.cn/forum.php?mod=viewthread") > 0)
            {
                lkdb.BroswersUsingIndex = 0;
                string title = VShawnEpub.Discuz.LKDiscuzBook.GetBookName(webBrowser1.DocumentTitle);
                lkdb.Initialize(title, lkdb.Browsers[lkdb.BroswersUsingIndex].Url.AbsoluteUri, @"C:\新建文件夹\");
                //开始
                if (!lkdb.IsBroswerOK(lkdb.Browsers[lkdb.BroswersUsingIndex]))
                {
                    MessageBox.Show("请先加载网页!");
                    return;
                }
                lkdb.Status = VShawnEpub.Discuz.LKDiscuzBook.BookStatus.Running;
                lkdb.Start();
            }
            //LK文库
            else if (tbUrl.Text.IndexOf("lknovel.lightnovel.cn") > 0)
            {
                lkwkb.Start();
            }
        }

        private void skinButton3_Click(object sender, EventArgs e)
        {
            WebBrowser wb = new WebBrowser();
            wb.Navigate(tbUrl.Text);
            wb.Navigated+= delegate(object o, WebBrowserNavigatedEventArgs args) { if(wb.ReadyState == WebBrowserReadyState.Complete)MessageBox.Show(wb.Text); };
        }

        private void skinButton4_Click(object sender, EventArgs e)
        {
        }
    }
}
