using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilgiYarismasi.Web
{
    public interface IBitis
    {
        int toplam { get; set; }
        int cevaplanan { get; set; }
        int dogruSayisi { get; set; }
        int yanlisSayisi { get; set; }
    }
}
