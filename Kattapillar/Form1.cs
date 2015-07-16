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
        /// <summary>
        /// 浏览器序列，用于预加载网页
        /// </summary>
        private List<WebBrowser> browsers;
        /// <summary>
        /// 浏览器序列指针
        /// </summary>
        private int broswersIndex = 0;

        public Form1()
        {
            InitializeComponent();
        }
        private void skinButton1_Click(object sender, EventArgs e)
        {
            broswersIndex = 0;
            browsers = new List<WebBrowser>();
            browsers.Add(webBrowser1);
            webBrowser1.Navigate(tbUrl.Text);
            string next = "";
            if (tbUrl.Text.IndexOf("www.lightnovel.cn") > 0)
            {
            }
            else if (tbUrl.Text.IndexOf("lknovel.lightnovel.cn") > 0)
            {
                
            }


            webBrowser1.Navigated += delegate(object o, WebBrowserNavigatedEventArgs args)
            {
                if (webBrowser1.ReadyState != WebBrowserReadyState.Loaded)
                {
                    WebBrowser wb = new WebBrowser();
                    wb.Navigate(next);
                    browsers.Add(wb);
                }
            };
        }

        private void skinButton2_Click(object sender, EventArgs e)
        {
            //开始爬取
            if (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
            {
                MessageBox.Show("请先加载网页!");
                return;
            }
            string html = webBrowser1.DocumentText;
            string title = VShawnEpub.Discuz.LKDiscuzBook.GetBookName(webBrowser1.DocumentTitle);
            VShawnEpub.Discuz.LKDiscuzBook lkdb = new VShawnEpub.Discuz.LKDiscuzBook(title, tbUrl.Text, @"C:\新建文件夹\");
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
