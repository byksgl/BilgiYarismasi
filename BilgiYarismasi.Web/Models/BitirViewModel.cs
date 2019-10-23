using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BilgiYarismasi.Web.Models
{
    public class BitirViewModel:IBitis
    {
        public int toplam { get; set; }
        public int cevaplanan { get; set; }
        public int dogruSayisi { get; set; }
        public int yanlisSayisi { get; set; }
    }
}