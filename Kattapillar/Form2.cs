using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
            lkwkb = null;
            lkdb = null;
            //skinButton1.Enabled = false;
            if (tbUrl.Text.IndexOf("www.lightnovel.cn/thread") > 0 || tbUrl.Text.IndexOf("www.lightnovel.cn/forum.php?mod=viewthread") > 0)
            {
                lkdb = new VShawnEpub.Discuz.LKDiscuzBook();


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

                        label1.Text = lkdb.Percentage.ToString();
                        label2.Text = lkdb.PercentageTotal.ToString();
                        label3.Text = lkdb.GetPercentage().ToString();
                    }
                };
            }
            else if (tbUrl.Text.IndexOf("lknovel.lightnovel.cn/main/book/") > 0)
            {
                lkwkb = new LKWenKuBook(); 

                webBrowser1.Navigate(tbUrl.Text);
                webBrowser1.DocumentCompleted += delegate(object o, WebBrowserDocumentCompletedEventArgs args)
                {
                    if (lkwkb != null && lkwkb.IsBroswerOK(webBrowser1) && lkwkb.Epub.Title == "")
                    {
                        lkwkb.ProcessMainPage(webBrowser1.Url.AbsoluteUri, webBrowser1.DocumentText);

                        label1.Text = lkwkb.Percentage.ToString();
                        label2.Text = lkwkb.PercentageTotal.ToString();
                        label3.Text = lkwkb.GetPercentage().ToString();
                    }
                };
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(delegate()
            {
                //LK论坛
                if (lkdb != null)
                {
                    lkdb.Start();
                }
                //LK文库
                else if (lkwkb != null)
                {
                    lkwkb.Start();
                }
            });
            t.Start();

            Thread t2 = new Thread(delegate()
            {                //LK论坛
                if (lkdb != null)
                {
                    while (lkdb.Status != BaseBook.BookStatus.Completed)
                    {
                        Thread.Sleep(200);
                        BeginInvoke(new System.Threading.ThreadStart(delegate()
                        {
                            label1.Text = lkdb.Percentage.ToString();
                            label2.Text = lkdb.PercentageTotal.ToString();
                            label3.Text = lkdb.GetPercentage().ToString();
                        }));
                    }
                }
                    //LK文库
                else if (lkwkb != null)
                {
                    while (lkwkb.Status != BaseBook.BookStatus.Completed)
                    {
                        Thread.Sleep(200);
                        BeginInvoke(new System.Threading.ThreadStart(delegate()
                        {
                            label1.Text = lkwkb.Percentage.ToString();
                            label2.Text = lkwkb.PercentageTotal.ToString();
                            label3.Text = lkwkb.GetPercentage().ToString();
                        }));
                    }
                }
            });
            t2.Start();
        }
    }
}
