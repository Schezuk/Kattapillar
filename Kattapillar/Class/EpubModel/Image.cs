using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace VShawnEpub.Model
{
    public class Image
    {
        public Image()
        {
            ReplaceName = StoreName = "";
        }
        /// <summary>
        /// 图片在原始txt中的占位字符串
        /// </summary>
        public string ReplaceName;
        /// <summary>
        /// 图片导入epub后的存储名称 不带后缀
        /// </summary>
        public string StoreName;


        private string url;
        /// <summary>
        /// 图片URL，设置后自动设置Ext
        /// </summary>
        public string Url
        {
            set { url = value;
                Ext = url.Substring(url.LastIndexOf("."));
            }
            get { return url; }
        }
        /// <summary>
        /// 图片下载后地址
        /// </summary>
        public string Path;
        /// <summary>
        /// 图片后缀 如 “.jpg”
        /// </summary>
        public string Ext;
        public ImageType Type;
        public enum ImageType
        {
            Cover,//封面图
            Contents,//目录图
            Illustration,//扉页插画
            InBook,//书内插图
            BackCover//封底图片
        }
    }
}
