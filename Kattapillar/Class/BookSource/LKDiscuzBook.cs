using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using VShawnEpub.Model;

namespace VShawnEpub.Discuz
{
    public class LKDiscuzBook:BaseBook
    {
        public const string HOST = "http://www.lightnovel.cn/";
        /// <summary>
        /// 楼主是谁
        /// </summary>
        public string Master;
        protected List<string> URLs;
        protected List<string> Htmls;
        /// <summary>
        /// 初始化本类，稍后需要执行Initialize手动初始化其余参数
        /// </summary>
        public LKDiscuzBook(): base()
        {
            URLs = new List<string>();
            Htmls = new List<string>();
            Master = "";
            //EvenAllCompleted += delegate(object sender, EventArgs args)
            //{
            //    Epub.OutPutTxt(OutPutDir);
            //};
        }
        public override event EventHandler EvenAllCompleted;
        /// <summary>
        /// 开始处理数据，把网页转为txt数据
        /// </summary>
        public void Start()
        {
            Thread t = new Thread(delegate()
            {
                while (base.Status != BookStatus.BroswerCompleted && base.Status != BookStatus.Completed)
                    Thread.Sleep(100);
                base.Epub.Capaters = new List<Capater>();
                string html0 = Htmls[0];
                if (Add(html0))
                {
                    while (Htmls.Count != 2)
                        Thread.Sleep(100);
                    Add(Htmls[1]);
                }
                ProcessInfo(base.Epub.Capaters[0].Html);

                Status = VShawnEpub.Discuz.LKDiscuzBook.BookStatus.Completed;
                base.Epub.OutPutTxt(base.OutPutDir);
            });
            t.Start();
        }
        /// <summary>
        /// 处理主页，并加载后续页面
        /// </summary>
        /// <param name="url"></param>
        /// <param name="html"></param>
        public override void ProcessMainPage(string url, string html)
        {
            URLs.Add(url);
            URLs.Add(url + "2");
            Htmls.Add(html);
            Status = BookStatus.BroswerCompleted;
            CreateWB(URLs[1]);
        }

        /// <summary>
        /// 创建浏览器加载帖子第二页
        /// </summary>
        /// <param name="c"></param>
        private void CreateWB(string url)
        {
            WebBrowser wb = new WebBrowser();
            base.Browsers.Add(wb);
            BroswersLodedIndex++;
            wb.DocumentCompleted += delegate(object sender, WebBrowserDocumentCompletedEventArgs args)
            {
                if (wb != null && IsBroswerOK(wb))
                {
                    //第二页预加载完毕
                    string html = wb.DocumentText.Clone().ToString();
                    //删除浏览器
                    wb = null;
                    //清理内存
                    IntPtr pHandle = GetCurrentProcess();
                    SetProcessWorkingSetSize(pHandle, -1, -1);
                    Htmls.Add(html);
                }
            };
            wb.Navigate(url);
        }
        /// <summary>
        /// 为书籍添加作者、插图、翻译等信息。
        /// </summary>
        private void ProcessInfo(string html)
        {
            base.Epub.Author = base.ClearBlank(getRegEx(html, @"(作者|原著).*[:|：]\s*(.*)<br", "$1", RegexOptions.IgnoreCase, false));
            base.Epub.OriginalTitle = base.ClearBlank(getRegEx(html, @"(日文名|原名|书名).*[:|：]\s*(.*)<br", "$1", RegexOptions.IgnoreCase, false));
            base.Epub.Illustration = base.ClearBlank(getRegEx(html, @"(插图|插画|插圖|插畫).*[:|：]\s*(.*)<br", "$1", RegexOptions.IgnoreCase, false));
            base.Epub.Translator = base.ClearBlank(getRegEx(html, @"翻译.*[:|：]\s*(.*)<br", "$1", RegexOptions.IgnoreCase, false));
        }
        /// <summary>
        /// 添加一个页面，并分析，返回是否需要添加下一页
        /// </summary>
        /// <param name="url">页面URL</param>
        /// <param name="html">页面HTML</param>
        public bool Add(string html)
        {
            MatchCollection divs = getRegExs(html, @"<div\s*id=""?post_(\d+)""?\s*>([\s\S]*?)<div\sid=""?comment_\d+");
            for (int i = 0; i < divs.Count; i++)
            {
                string master = getRegEx(divs[i].ToString(), @"<strong><a[\s\S]*?>([\s\S]*?)</a>", "$1");
                if (this.Master == "")//记录楼主
                    Master = master;
                else if (Master != master) //如果不再是楼主的帖子，认为内容已经发完，结束。
                {
                    base.Status = BookStatus.Completed;
                    return false;
                }
                VShawnEpub.Model.Capater c = new VShawnEpub.Model.Capater();
                c.Html = divs[i].ToString();
                string strBody = getRegEx(divs[i].ToString(), @"id=""?postmessage_\d*[\s\S]*?>([\s\S]*?)<div id=""?comment_", "$1");
                //提取出纯文字
                string content = getRegEx(strBody, @"([\s\S]*?)</td></tr>(</tbody>)?</table>").Replace("\r","").Replace("\n","");
                //去除【本帖最后由 XXXX 于 XXXXXX 编辑】
                content = Regex.Replace(content, @"<i\s?class=""?pstatus""?>.*?</i>", "", RegexOptions.IgnoreCase);
                //处理img标签为[IMG]+图片URL
                //处理LK外链图片(或者全链)
                content = Regex.Replace(content, @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>http://[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", "\r\n[IMG]$1\r\n", RegexOptions.IgnoreCase);
                //处理LK内链图
                //将<ignore_js_op>中内容只保留img标签
                content = Regex.Replace(content, @"<ignore_js_op>[\s\S]*?<img[\s\S]*?zoomfile=""?([\s\S]*?)""?\s[\s\S]*?>[\s\S]*?</ignore_js_op>", "\r\n[IMG]" + HOST + "$1\r\n", RegexOptions.IgnoreCase);
                content = Regex.Replace(content, @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", "\r\n[IMG]" + HOST + "", RegexOptions.IgnoreCase);

                content = base.NoHTML(content, HOST);

                c.Txt = content;
                base.Epub.Capaters.Add(c);

                //若最后一楼还是楼主的帖子
                if (i == (divs.Count - 1))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 获得原始URL（第一页）,后面加数字几，就是第几页的链接
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
                return HOST + @"forum.php?mod=viewthread&tid=" + tid + @"&page=";
            }
            else if (originalURL.IndexOf("tid=") > 0)
            {
                string tid = getRegEx(originalURL, @"tid=(\d*)", "$1");
                return HOST + @"forum.php?mod=viewthread&tid=" + tid + @"&page=";  
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
