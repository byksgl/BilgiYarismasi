using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BilgiYarismasi.Web.Models
{
    public class YarisIndexViewModel
    {
        public Guid Id { get; set; }
        public int EnYuksekSinglePuan { get; set; }
        public int EnSonSinglePuan { get; set; }
        public int MultiEnsonPuan { get; set; }
        public int MultiEnYuksekPuan { get; set; }

    }
}