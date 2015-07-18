using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VShawnEpub.Wenku
{
    public abstract class WenKuBook:BaseBook
    {
        public WenKuBook(): base()
        {
        }
        public abstract void ProcessMainPage(string url, string html);
    }
}
