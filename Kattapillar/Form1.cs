using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CCWin;

namespace Kattapillar
{
    public partial class Form1 : CCSkinMain
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate(tbUrl.Text);
        }

        private void skinButton2_Click(object sender, EventArgs e)
        {
            //开始爬取
            if (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
            {
                MessageBox.Show("请先加载网页");
                return;
            }
            string html = webBrowser1.DocumentText;
            string title = webBrowser1.DocumentTitle;
            if (title.IndexOf("-轻之国度") > 0)
            {
                title = title.Substring(0, title.IndexOf("-轻之国度"));
            }
            if (title.IndexOf(" - 轻之国度") > 0)
            {
                title = title.Substring(0, title.IndexOf(" - 轻之国度"));
            }
            title = title.Replace(@"/", "").Replace(@"\", "").Replace("?", "").Replace("*", "").Replace("<", "").Replace(">", "").Replace("|", "").Replace(":", "").Replace("\"", "");

            VShawnEpub.Discuz.LKDiscuzBook lkdb = new VShawnEpub.Discuz.LKDiscuzBook(title, tbURL.Text, @"C:\新建文件夹\");
            lkdb.Add(tbUrl.Text, html);
        }

        private void skinButton3_Click(object sender, EventArgs e)
        {
            WebBrowser wb = new WebBrowser();
            wb.Navigate(tbUrl.Text);
            wb.Navigated+= delegate(object o, WebBrowserNavigatedEventArgs args) { if(wb.ReadyState == WebBrowserReadyState.Complete)MessageBox.Show(wb.Text); };
        }
    }
}
