using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace VShawnEpub.Discuz
{
    public class LKDiscuzBook:BaseBook
    {
        /// <summary>
        /// 楼主是谁
        /// </summary>
        public string Master;
        /// <summary>
        /// 已经处理过的HTML
        /// </summary>
        protected List<string> URLs;
        /// <summary>
        /// 初始化本类，稍后需要执行Initialize手动初始化其余参数
        /// </summary>
        public LKDiscuzBook(): base()
        {
            URLs = new List<string>();
            Master = "";
            EvenAllCompleted += delegate(object sender, EventArgs args)
            {
                OutPutTxt(OutPutDir);
            };
        }
        /// <summary>
        /// 初始化本类，设置书本标题，主页面，txt输出页面
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mainURL"></param>
        /// <param name="outPutDir"></param>
        public LKDiscuzBook(string title, string mainURL,string outPutDir) : base(title, mainURL,outPutDir)
        {
            URLs = new List<string>();
            Master = "";
            EvenAllCompleted += delegate(object sender, EventArgs args)
            {
                OutPutTxt(OutPutDir);
            };
        }
        public override event EventHandler EvenAllCompleted;
        /// <summary>
        /// 开始处理数据，把网页转为txt数据
        /// </summary>
        public void Start()
        {
            if (URLs.Count >= Browsers.Count)
                URLs.Clear();
            if (BroswersUsingIndex < Browsers.Count && base.Status != BookStatus.Completed)
            {
                base.Status = BookStatus.Running;
                //若浏览器未加载完，等待，并设置状态。
                if (!IsBroswerOK(base.Browsers[base.BroswersUsingIndex]))
                {
                    base.Status = BookStatus.WaitingBroswer;
                }
                else
                {
                    Add(Browsers[BroswersUsingIndex].Url.AbsoluteUri, Browsers[BroswersUsingIndex].DocumentText);
                    BroswersUsingIndex++;
                    Start();
                }
            }
        }
        /// <summary>
        /// 添加一个页面，并分析
        /// </summary>
        /// <param name="url">页面URL</param>
        public override void Add(string url)
        {
            //WebBrowser wb = new WebBrowser();
            //wb.DocumentCompleted += delegate(object sender, WebBrowserDocumentCompletedEventArgs args)
            //{
            //    if (wb.ReadyState == WebBrowserReadyState.Loaded || (wb.ReadyState == WebBrowserReadyState.Complete && wb.IsBusy == false))
            //    {
            //        Add(url, wb.DocumentText);
            //    }
            //};
            //wb.Navigate(url);
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
            MatchCollection divs = getRegExs(html, @"<div\s*id=""?post_(\d+)""?\s*>([\s\S]*?)<div\sid=""?comment_\d+");
            for (int i = 0; i < divs.Count; i++)
            {
                string master = getRegEx(divs[i].ToString(), @"<strong><a[\s\S]*?>([\s\S]*?)</a>", "$1");
                if (this.Master == "")//记录楼主
                    Master = master;
                else if (Master != master) //如果不再是楼主的帖子，认为内容已经发完，结束。
                {
                    base.Status = BookStatus.Completed;
                    EvenAllCompleted(this, null);
                    break;
                }

                string strBody = getRegEx(divs[i].ToString(), @"id=""?postmessage_\d*[\s\S]*?>([\s\S]*?)<div id=""?comment_", "$1");
                //提取出纯文字
                string content = getRegEx(strBody, @"([\s\S]*?)</td></tr>(</tbody>)?</table>");
                //content = Regex.Replace(content, "</div>", "\r\n</div>", RegexOptions.IgnoreCase);
                //content = Regex.Replace(content, "</p>", "\r\n</p>", RegexOptions.IgnoreCase);
                //content = Regex.Replace(content, @"<br\s*/?\s*>", "\r\n", RegexOptions.IgnoreCase);

                //去除【本帖最后由 XXXX 于 XXXXXX 编辑】
                content = Regex.Replace(content, @"<i\s?class=""?pstatus""?>.*?</i>", "", RegexOptions.IgnoreCase);

                //处理img标签为[IMG]+图片URL
                //处理LK外链图片(或者全链)
                content = Regex.Replace(content, @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>http://[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", "\r\n[IMG]$1\r\n", RegexOptions.IgnoreCase);
                //处理LK内链图
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
                //若最后一楼还是楼主的帖子，且下一页的浏览器尚未创建。
                if (i == (divs.Count - 1) && Browsers.Count == (BroswersUsingIndex + 1))
                {
                    string nextUrl = GetTrueUrl(Browsers[0].Url.AbsoluteUri) + (Browsers.Count + 1);
                    WebBrowser wb = new WebBrowser();
                    base.Browsers.Add(wb);
                    wb.Navigate(nextUrl);
                    wb.DocumentCompleted += delegate(object sender, WebBrowserDocumentCompletedEventArgs args)
                    {
                        WebBrowserReadyState w = wb.ReadyState;
                        string h = wb.DocumentText;
                        bool x = wb.IsBusy;
                        if (IsBroswerOK(base.Browsers[base.BroswersUsingIndex]))
                        {
                            Start();
                        }
                    };
                    base.Status = BookStatus.WaitingBroswer;


                    //Add(nextUrl);
                }
            }
        }


        /// <summary>
        /// 获得原始URL（第一页）
        /// </summary>
        /// <param name="originalURL"></param>
        /// <returns>形如//http://www.lightnovel.cn/forum.php?mod=viewthread&tid=825316&extra=page%3D1&page=</returns>
        public static string GetTrueUrl(string originalURL)
        {
            //www.lightnovel.cn/thread-825316-1-1.html
            //www.lightnovel.cn/forum.php?mod=viewthread&tid=825316&extra=page%3D1&page=1

            if (originalURL.IndexOf("/thread") >= 0)
            {
                string tid = getRegEx(originalURL, @"www.lightnovel.cn/thread-(\d*)-(\d*)-\d.html", "$1");
                return @"http://www.lightnovel.cn/forum.php?mod=viewthread&tid=" + tid + @"&page=";
            }
            else if (originalURL.IndexOf("tid=") > 0)
            {
                string tid = getRegEx(originalURL, @"tid=(\d*)", "$1");
                return @"http://www.lightnovel.cn/forum.php?mod=viewthread&tid=" + tid + @"&page=";  
            }
            return "";
        }
        /// <summary>
        /// 获得书本的名称
        /// </summary>
        /// <param name="webtitle"></param>
        /// <returns></returns>
        public static string GetBookName(string webtitle)
        {

            if (webtitle.IndexOf("-轻之国度") > 0)
            {
                webtitle = webtitle.Substring(0, webtitle.IndexOf("-轻之国度"));
            }
            if (webtitle.IndexOf(" - 轻之国度") > 0)
            {
                webtitle = webtitle.Substring(0, webtitle.IndexOf(" - 轻之国度"));
            }
            webtitle = webtitle.Replace(@"/", "").Replace(@"\", "").Replace("?", "").Replace("*", "").Replace("<", "").Replace(">", "").Replace("|", "").Replace(":", "").Replace("\"", "");
            return webtitle;
        }

        /// <summary>
        /// 判断浏览器是否加载完成
        /// </summary>
        /// <returns></returns>
        public static bool IsBroswerOK(WebBrowser wb)
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
