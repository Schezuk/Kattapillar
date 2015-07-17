using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace epubFactory
{
    static class Version
    {
        public const string version = "0.0.1";            //当前版本号
        public const string edition = "青虫 0.0.1";
        public const int date = 20141207;            //当前版本时间
        public const string sofeId = "Kattapillar PROJECT";      //标识符
        public const string versionXml = "http://singlex.sinaapp.com/Kattapillar/version.php";
        private static Dictionary<string, string> xmlData =new Dictionary<string, string>();


        /// <summary>
        /// 检查更新
        /// </summary>
        public static void Check(bool auto)
        {
            ReadVersionXML();
            //try
            //{
            //    int newDate = int.Parse(GetDate());
            //    //判断如果设置了不再提示，则停止检查自动更新
            //    if (auto && SofeSetting.Get("ignoreUpdate") == newDate.ToString())
            //    {
            //        return;
            //    }
            //    if (newDate > date) 
            //    {
            //        //MessageBox.Show("更新！");
            //        Update u = new Update(auto);
            //        DialogResult d = u.ShowDialog();
            //        if (d == DialogResult.Ignore)
            //        {
            //            SofeSetting.Set("ignoreUpdate", newDate.ToString());
            //        }
            //    }
            //    else if (!auto)
            //    {
            //        MessageBox.Show("您的程序已经是最新版本！");
            //    }
            //}
            //catch (Exception)
            //{
            //    if (!auto)
            //        MessageBox.Show("您的程序已经是最新版本！");
            //}
        }
        public static string GetVersion()
        {
            return GetXML("version");
        }
        public static string GetDate()
        {
            return GetXML("date");
        }
        public static string GetDocuments()
        {
            return GetXML("documents");
        }
        public static string GetDownload()
        {
            return GetXML("download");
        }
        public static string GetInfo()
        {
            return GetXML("info");
        }
        private static string GetXML(string key)
        {
            if (xmlData.Count == 0)
            {
                ReadVersionXML();
            }
            if (xmlData.ContainsKey(key))
            {
                return xmlData[key];
            }
            return "";
        }
        private static void ReadVersionXML()
        {
            Dictionary<string, string> tmp = new Dictionary<string, string>();

            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(versionXml);
                XmlNode Lefe = xmldoc.SelectSingleNode("Lefe");
                foreach (XmlNode node in Lefe)
                {
                    tmp.Add(node.Name, node.InnerText);
                }
            }
            catch
            {

            }
            finally
            {
                xmlData = tmp;
            }

            
        }
    }
}