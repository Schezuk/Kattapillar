using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VShawnEpub.Model;

namespace VShawnEpub.Wenku
{
    public class LKWenKuBook : WenKuBook
    {
        public override event EventHandler EvenAllCompleted;
        public LKWenKuBook() : base()
        {
            Initialize("", "", "");
        }
        public override void Add(string url,string html)
        {
            
        }

        public void Start()
        {
            if (base.Browsers.Count == 0)
            {
                MessageBox.Show("请先点击加载按钮加载文库网页。");
                return;
            }

            while (base.Status != BookStatus.BroswerCompleted)
            {
                Thread.Sleep(100);
            }
            for (int i = 0; i < base.Epub.Capaters.Count; i++)
            {
                string strBody = base.Epub.Capaters[i].Html;
                strBody = getRegEx(strBody, @"<div id=""J_view"" class=""mt-20"">([\s\S]*?)<div class=""text-center mt-20"">", "$1");
                strBody = Regex.Replace(strBody, @"<h2([\s\S]*?)>([\s\S]*?)</h3>", "");
                //去Html标签
                strBody = NoHTML(strBody).Trim();
            }
        }

        /// <summary>
        /// 分析文库主页，获得各章URL
        /// </summary>
        /// <param name="url"></param>
        /// <param name="html"></param>
        public override void ProcessMainPage(string url, string html)
        {
            base.MainURL = url;
            string div = getRegEx(html, @"<!--\s*左侧\s*-->([\s\S]*?)<!--\s*右侧\s*-->","$1");
            base.Epub.CoverImg.Url = @"http://lknovel.lightnovel.cn/" + getRegEx(div, @"<div class=""lk-book-cover""><a href=""#"" target=""_blank""><img src=""([\s\S]*?)""/>", "$1");
            base.Epub.CoverImg.StoreName = "cover";

            base.Epub.Title = base.ClearBlank(getRegEx(div, @"<h1 class=""ft-24"">[\s\S]*<strong>([\s\S]*)</strong>[\s\S]*</h1>", "$1", RegexOptions.IgnoreCase, false));
            base.Epub.SubTitle = base.ClearBlank(getRegEx(div, @"<td width=""40"">标 题：</td>[\s\S]*<td>([\s\S]*)</td>[\s\S]*<td width=""40"">作 者：</td>", "$1", RegexOptions.IgnoreCase, false));
            base.Epub.Author = base.ClearBlank(getRegEx(div, @"<td width=""40"">作 者：</td>[\s\S]*<td><a target=""_blank"" href=""[\s\S]*"">([\s\S]*)</a></td>", "$1", RegexOptions.IgnoreCase, false));
            base.Epub.Illustration = base.ClearBlank(getRegEx(div, @"<td>插 画：</td>[\s\S]*<td>([\s\S]*)</td>[\s\S]*<td>文 库：</td>", "$1", RegexOptions.IgnoreCase, false));

            div = getRegEx(html, @"<ul class=""lk-chapter-list unstyled pt-10 pb-10 mt-20"">([\s\S]*)</ul>", "$1", RegexOptions.IgnoreCase, false);
            MatchCollection matchCollection = getRegExs(div, @"<a\s?href=""(?<url>[\s\S]*?)""\s?>([\s\S]*?)<span\s?class=""lk-ellipsis"">(?<text>[\s\S]*?)</span>([\s\S]*?)</a>");
            for (int i = 0; i < matchCollection.Count; i++)
            {
                Capater c = new Capater();
                //章标题
                c.Index = i;
                c.Title = base.ClearBlank(matchCollection[i].Groups["text"].Value.Replace("\r", "").Replace("\n", " ").Replace("\t", " ").Replace("　", " ").Trim());
                c.Url = matchCollection[i].Groups["url"].Value.Trim();
                base.Epub.Capaters.Add(c);
            }

            base.Status = BookStatus.Running;
            CreateWB(base.Epub.Capaters[0]);
        }
        /// <summary>
        /// 创建浏览器加载各个章节
        /// </summary>
        /// <param name="c"></param>
        private void CreateWB(Capater c)
        {
            WebBrowser wb = new WebBrowser();
            base.Browsers.Add(wb);
            BroswersLodedIndex++;
            wb.DocumentCompleted += delegate(object sender, WebBrowserDocumentCompletedEventArgs args)
            {
                if (wb != null && IsBroswerOK(wb))
                {
                    c.Html = wb.DocumentText;
                    if (base.Epub.Capaters.Count > base.Browsers.Count)
                    {
                        CreateWB(base.Epub.Capaters[base.Browsers.Count]);
                    }
                    else
                    {
                        Status = BookStatus.BroswerCompleted;
                    }
                    //删除浏览器
                    wb = null;
                }
            };
            wb.Navigate(c.Url);
        }



        /// <summary>
        /// 判断浏览器是否加载完成
        /// </summary>
        /// <returns></returns>
        public override bool IsBroswerOK(WebBrowser wb)
        {
            string html = wb.DocumentText;
            if (wb.ReadyState == WebBrowserReadyState.Complete || html.IndexOf("闽ICP备0702546号-1") != -1)
            {
                return true;
            }
            return false;
        }
    }
}
