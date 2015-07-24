using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using VShawnEpub.Model;

namespace VShawnEpub
{
    public abstract class BaseBook
    {
        #region 浏览器内存释放相关
        //调用系统内核，设置进程内存
        [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);
        [DllImport("KERNEL32.DLL", EntryPoint = "GetCurrentProcess", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr GetCurrentProcess();
        #endregion
        public EpubModel Epub;
        /// <summary>
        /// 书本主页
        /// </summary>
        public string MainURL;
        public string Host;
        /// <summary>
        /// 最终输出路径
        /// </summary>
        public string OutPutDir = @"C:\新建文件夹\";
        /// <summary>
        /// 书本处理状态
        /// </summary>
        public BookStatus Status = BookStatus.NotInit;
        public enum BookStatus
        {
            NotInit,
            Inited,
            WaitingBroswer,
            BroswerCompleted,
            Running,
            Completed
        }
        //浏览器
        /// <summary>
        /// 浏览器序列，用于预加载网页
        /// </summary>
        public List<WebBrowser> Browsers;
        /// <summary>
        /// 浏览器创建序列指针
        /// </summary>
        public int BroswersLodedIndex = 0;
        /// <summary>
        /// 浏览器使用序列指针
        /// </summary>
        public int BroswersUsingIndex = 0;


        public BaseBook(string title, string mainURL,string outPutDir)
        {
            Browsers = new List<WebBrowser>();
            BroswersLodedIndex = 0;
            BroswersUsingIndex = 0;

            Initialize(title, mainURL, outPutDir);
        }
        /// <summary>
        /// 手动初始化类
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mainURL"></param>
        /// <param name="outPutDir"></param>
        public virtual void Initialize(string title, string mainURL, string outPutDir)
        {
            Epub = new EpubModel();
            Epub.Title = title;
            BroswersUsingIndex = 0;
            Host = "";
            MainURL = mainURL;
            OutPutDir = outPutDir;
            if(OutPutDir == "")
                OutPutDir = @"C:\新建文件夹\";
            Status = BookStatus.Inited;
        }
        public BaseBook()
        {
            Browsers = new List<WebBrowser>();
            BroswersLodedIndex = 0;
            Initialize("", "", "");
        }
        /// <summary>
        /// 处理主页，并加载后续页面
        /// </summary>
        /// <param name="url"></param>
        /// <param name="html"></param>
        public abstract void ProcessMainPage(string url, string html);
        /// <summary>
        /// 所有书本完成事件，其响应为输出Txt
        /// </summary>
        public abstract event EventHandler EvenAllCompleted;
        /// <summary>
        /// 判断浏览器是否加载完成
        /// </summary>
        /// <param name="wb"></param>
        /// <returns></returns>
        public abstract bool IsBroswerOK(WebBrowser wb);






        ///工作进度规则
        /// 一个页面百分数分母增加 11，其中页面加载占10，文本处理占1



        //工作进度
        public int Percentage = 0;
        public int PercentageTotal = 0;

        protected void AddPercentageTotal()
        {
            PercentageTotal += 11;
        }
        protected void AddPercentage(PercentageType pt)
        {
            if (pt == PercentageType.PageLoaded)
                Percentage += 10;
            else
                Percentage += 1;
        }

        /// <summary>
        /// 获得工作进度
        /// </summary>
        public int GetPercentage()
        {
            if (Status == BookStatus.Inited || PercentageTotal == 0)
                return 0;
            if (Status == BookStatus.Completed || Percentage >= PercentageTotal)
                return 100;
            int p = (int)((Percentage/(double)PercentageTotal)*100);
            return p;
        }
        protected enum PercentageType
        {
            PageLoaded,
            PageProcessed
        }













        /// <summary>
        /// 获得一个正则表达式的匹配
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="get">$1 $2...表示获得第几个()匹配</param>
        /// <param name="o">匹配模式</param>
        /// <param name="ThrowException">是否抛出异常</param>
        /// <returns></returns>
        protected static string getRegEx(string str, string pattern, string get = "", RegexOptions o = RegexOptions.IgnoreCase, bool ThrowException = true)
        {
            Match m = Regex.Match(str, pattern, o);
            if (ThrowException)
                ThrowMissMatch(m);
            if (!m.Success)
            {
                return "";
            }
            if (get != "")
            {
                return m.Result(get);
            }
            return m.ToString();
        }
        /// <summary>
        /// 获得多个正则表达式的匹配
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        protected static MatchCollection getRegExs(string str, string pattern, RegexOptions o = RegexOptions.IgnoreCase, bool ThrowException = true)
        {
            MatchCollection rs = Regex.Matches(str, pattern, o);
            if (ThrowException)
                ThrowMissMatch(rs);
            return rs;
        }
        private static void ThrowMissMatch(Match m)
        {
            if (!m.Success)
            {
                MessageBox.Show("Miss Mach! 可能是由于网页未加载完毕或网站源代码已经改变所导致的，请重试几次或联系程序开发者修改程序。", "错误！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception();
            }
        }
        private static void ThrowMissMatch(MatchCollection m)
        {
            if (m.Count == 0)
            {
                MessageBox.Show("Miss Mach! 可能是由于网页未加载完毕或网站源代码已经改变所导致的，请重试几次或联系程序开发者修改程序。", "错误！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception();
            }
        }
        /// <summary>
        /// 删除Html 将img标签替换为一行url，形如[IMG]http://........
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        protected string NoHTML(string Htmlstring,string host = "")
        {
            Htmlstring = Htmlstring.Replace("\t", " ");
            Htmlstring = Htmlstring.Replace("\r", "");
            Htmlstring = Htmlstring.Replace("\n", "");
            Htmlstring = Regex.Replace(Htmlstring, "<br />", "\r\n");
            Htmlstring = Regex.Replace(Htmlstring, "</p>", "\r\n</p>");
            //Htmlstring = Regex.Replace(Htmlstring, "</div>", "\r\n</div>");
            //图片统一格式到Txt
            Htmlstring = Regex.Replace(Htmlstring, @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>http://[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", "\r\n[IMG]$1\r\n", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", "\r\n[IMG]" + host + "$1\r\n", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"<br\s*/>", "\r", RegexOptions.IgnoreCase);
            //删除脚本
            //Htmlstring = Regex.Replace(Htmlstring, @"<.*>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);

            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);



            while (Htmlstring.IndexOf("\r\n\r\n\r\n") >= 0)
                Htmlstring = Htmlstring.Replace("\r\n\r\n\r\n", "\r\n\r\n");
            while (Htmlstring.StartsWith("\n") || Htmlstring.StartsWith("\r"))
                Htmlstring = Htmlstring.Remove(0, 1);
            while (Htmlstring.EndsWith("\n") || Htmlstring.EndsWith("\r"))
                Htmlstring = Htmlstring.Remove(Htmlstring.Length - 1, 1);
            //Htmlstring.Replace("<", "");
            //Htmlstring.Replace(">", "");
            //Htmlstring = Regex.Replace(Htmlstring, @"([\r])[\s]+", "", RegexOptions.IgnoreCase);
            //Htmlstring.Replace("\r", "");

            return Htmlstring.Trim();
        }
        /// <summary>
        /// 去除空格 nbsp /t
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected string ClearBlank(string str)
        {
            str = str.Trim();
            str = str.Replace("\t", " ");
            str = str.Replace("\r", " ");
            str = str.Replace("\n", " ");
            str = str.Replace("&nbsp;", " ");
            while (str.IndexOf("  ") >= 0)
            {
                str = str.Replace("  ", " ");
            }
            return str;
        }
    }
}
