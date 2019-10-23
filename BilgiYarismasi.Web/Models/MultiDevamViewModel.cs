using BilgiYarismasi.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BilgiYarismasi.Web.Models
{
    public class MultiDevamViewModel
    {
        public bool YarismaBasladimi { get; set; }
        public YarismaSoru OncekiSoru { get; set; }
        public bool oncekiCevapDogrumu { get; set; }
    }
}