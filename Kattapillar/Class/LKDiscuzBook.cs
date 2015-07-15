using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace VShawnEpub.Discuz
{
    public class LKDiscuzBook:BaseBook
    {
        public string Master;
        //public List<Floor> Floors;
        protected List<string> URLs;
        public LKDiscuzBook(string title, string mainURL,string outPutDir) : base(title, mainURL,outPutDir)
        {
            URLs = new List<string>();
            Master = ""; 
            EvenAllCompleted+= delegate(object sender, EventArgs args)
            {
                OutPutTxt(OutPutDir);
            };
        }
        public override event EventHandler EvenAllCompleted;
        public override void Add(string url)
        {
            WebBrowser wb = new WebBrowser();
            wb.DocumentCompleted += delegate(object sender, WebBrowserDocumentCompletedEventArgs args)
            {
                if (wb.ReadyState == WebBrowserReadyState.Loaded || (wb.ReadyState == WebBrowserReadyState.Complete && wb.IsBusy == false))
                {
                    Add(url, wb.DocumentText);
                }
            };
            wb.Navigate(url);
        }
        /// <summary>
        /// 添加一个页面，并分析
        /// </summary>
        /// <param name="url">页面URL</param>
        /// <param name="html">页面HTML</param>
        public void Add(string url, string html)
        {
            if (URLs.Contains(url))
            {
                return;
            }
            URLs.Add(url);
            this.Htmls.Add(html);
            MatchCollection divs = base.getRegExs(html, @"<div\s*id=""?post_(\d+)""?\s*>([\s\S]*?)<div\sid=""?comment_\d+");
            for (int i = 0; i < divs.Count; i++)
            {
                string master = base.getRegEx(divs[i].ToString(), @"<strong><a[\s\S]*?>([\s\S]*?)</a>", "$1");
                if (this.Master == "")//记录楼主
                    Master = master;
                else if (Master != master) //如果不再是楼主的帖子，认为内容已经发完，结束。
                {
                    EvenAllCompleted(this, null);
                    break;
                }

                string strBody = base.getRegEx(divs[i].ToString(), @"id=""?postmessage_\d*[\s\S]*?>([\s\S]*?)<div id=""?comment_", "$1");
                //提取出纯文字
                string content = base.getRegEx(strBody, @"([\s\S]*?)</td></tr>(</tbody>)?</table>");
                //content = Regex.Replace(content, "</div>", "\r\n</div>", RegexOptions.IgnoreCase);
                //content = Regex.Replace(content, "</p>", "\r\n</p>", RegexOptions.IgnoreCase);
                //content = Regex.Replace(content, @"<br\s*/?\s*>", "\r\n", RegexOptions.IgnoreCase);

                //去除【本帖最后由 XXXX 于 XXXXXX 编辑】
                content = Regex.Replace(content, @"<i\s?class=""?pstatus""?>.*?</i>", "", RegexOptions.IgnoreCase);

                //处理img标签为[IMG]+图片URL
                //处理LK外链图片(或者全链)
                content = Regex.Replace(content, @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>http://[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", "\r\n[IMG]$1\r\n", RegexOptions.IgnoreCase);
                //处理LK内链图
                //content = Regex.Replace(content, @"<ignore_js_op>[\s\S]*?(<img[\s\S]*?>)[\s\S]*?</ignore_js_op>", "$1", RegexOptions.IgnoreCase);
                //将<ignore_js_op>中内容只保留img标签
                content = Regex.Replace(content, @"<ignore_js_op>[\s\S]*?<img[\s\S]*?zoomfile=""?([\s\S]*?)""?\s[\s\S]*?>[\s\S]*?</ignore_js_op>", "\r\n[IMG]http://www.lightnovel.cn/$1\r\n", RegexOptions.IgnoreCase);
                content = Regex.Replace(content, @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", "\r\n[IMG]http://www.lightnovel.cn/$1\r\n", RegexOptions.IgnoreCase);

                content = base.NoHTML(content);
                while (content.IndexOf("\r\n\r\n\r\n") >= 0)
                    content = content.Replace("\r\n\r\n\r\n", "\r\n\r\n");
                while (content.StartsWith("\n") || content.StartsWith("\r"))
                    content = content.Remove(0, 1);
                while (content.EndsWith("\n") || content.EndsWith("\r"))
                    content = content.Remove(content.Length - 1, 1);
                //Floor f = new Floor(master, divs[i].ToString(), content, URLs.Count - 1);
                //Floors.Add(f);
                this.Txts.Add(content);
                if (i == (divs.Count - 1) && url.EndsWith("-1.html"))
                {
                    int next = 0;
                    int.TryParse(url.Substring(url.IndexOf("-1.html") - 1, 1), out next);
                    next++;
                    if (next > 1)
                    {
                        string nextUrl = url.Substring(0, url.IndexOf("-1.html") - 1) + next + "-1.html";
                        Add(nextUrl);
                    }
                }
            }
        }
    }
}
