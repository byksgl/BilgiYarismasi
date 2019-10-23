using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BilgiYarismasi.DAL;

namespace BilgiYarismasi.Web.Areas.Yonetim.Models
{
    public class KonuSonucDetayViewModel
    {
        public AspNetUsers user { get; set; }
        public Kullanici kullanici { get; set; }

        public Yarisma yarisma { get; set; }
        public int cevaplanan { get; set; }
        public int dogru { get; set; }
        public int yanlis { get; set; }
        public string Tip { get; set; }
    }

    public class KonuSonucViewModel
    {
        public Konu konu { get; set; }
        public List<KonuSonucDetayViewModel> detaylar { get; set; }

        public KonuSonucViewModel()
        {
            detaylar = new List<KonuSonucDetayViewModel>();
        }
    }


    public class KullaniciSonucDetayViewModel
    {
        public Konu konu { get; set; }
        public Yarisma yarisma { get; set; }
        public int cevaplanan { get; set; }
        public int dogru { get; set; }
        public int yanlis { get; set; }

    }

    public class KullaniciSonucViewModel
    {
        public AspNetUsers user { get; set; }
        public Kullanici kullanici { get; set; }
        public List<KullaniciSonucDetayViewModel> detaylar { get; set; }

        public KullaniciSonucViewModel()
        {
            detaylar = new List<KullaniciSonucDetayViewModel>();
        }
    }
}