using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BilgiYarismasi.Web.Areas.Yonetim.Models
{
    public class SoruViewModel
    {
        public System.Guid Id { get; set; }
        public System.Guid KonuId { get; set; }
        public string Sorusu { get; set; }

        public Nullable<int> SecenekSayisi { get; set; }

        public SelectList Konular { get; set; }

    }
}