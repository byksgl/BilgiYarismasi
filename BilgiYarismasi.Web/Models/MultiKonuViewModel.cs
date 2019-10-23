using BilgiYarismasi.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BilgiYarismasi.Web.Models
{
    public class MultiKonuViewModel
    {
        public List<KonuKisiModel> konular { get; set; }
    }

    public class KonuKisiModel
    {
        public Konu konu { get; set; }
        public int KisiSayisi { get; set; }
    }
}