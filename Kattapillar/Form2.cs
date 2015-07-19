using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VShawnEpub;
using VShawnEpub.Wenku;

namespace Kattapillar
{
    public partial class Form2 : Form
    {
        private VShawnEpub.Discuz.LKDiscuzBook lkdb;
        private VShawnEpub.Wenku.LKWenKuBook lkwkb; 
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //skinButton1.Enabled = false;
            if (tbUrl.Text.IndexOf("www.lightnovel.cn/thread") > 0 || tbUrl.Text.IndexOf("www.lightnovel.cn/forum.php?mod=viewthread") > 0)
            {
                lkdb = new VShawnEpub.Discuz.LKDiscuzBook();
                lkwkb = null;

                lkdb.Browsers.Add(webBrowser1);
                string url = VShawnEpub.Discuz.LKDiscuzBook.GetTrueUrl(tbUrl.Text);
                lkdb.Status = VShawnEpub.Discuz.LKDiscuzBook.BookStatus.WaitingBroswer;
                webBrowser1.Navigate(url);
                webBrowser1.DocumentCompleted += delegate(object o, WebBrowserDocumentCompletedEventArgs args)
                {
                    if (lkdb != null && lkdb.IsBroswerOK(webBrowser1) && lkdb.Epub.Title == "")
                    {
                        lkdb.Epub.Title = VShawnEpub.Discuz.LKDiscuzBook.GetBookName(webBrowser1.DocumentTitle);
                        lkdb.ProcessMainPage(webBrowser1.Url.AbsoluteUri, webBrowser1.DocumentText);
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
                    if (lkwkb!= null && lkwkb.IsBroswerOK(webBrowser1) && lkwkb.Epub.Title == "")
                        lkwkb.ProcessMainPage(webBrowser1.Url.AbsoluteUri, webBrowser1.DocumentText);
                };
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //LK论坛
            if ((tbUrl.Text.IndexOf("www.lightnovel.cn/thread") > 0 || tbUrl.Text.IndexOf("www.lightnovel.cn/forum.php?mod=viewthread") > 0) && lkdb != null)
            {
                lkdb.Start();
            }
            //LK文库
            else if (tbUrl.Text.IndexOf("lknovel.lightnovel.cn") > 0 && lkwkb != null)
            {
                lkwkb.Start();
            }
        }
    }
}
