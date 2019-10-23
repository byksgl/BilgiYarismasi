using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BilgiYarismasi.Web
{
    public abstract class ACevapKisa
    {
        public virtual string Cevabi { get; set; }

        public virtual bool cevapVarmi()
        {
            bool b1 = string.IsNullOrEmpty(Cevabi);
            return !b1;
        }
    }
}