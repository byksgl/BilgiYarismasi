using BilgiYarismasi.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BilgiYarismasi.Web.Models
{
    public class ProfilIndexViewModel
    {
        public Kullanici kullanici { get; set; }
        public int ToplamYarismaSayisi { get; set; }
        public int ToplamDogruSayisi { get; set; }
        public int ToplamYanlisSayisi { get; set; }
        public int SingleBasariPuani { get; set; }

        public string AdSoyad
        {
            get
            {
                if (kullanici == null) return string.Empty;
                return string.Format("{0} {1}", kullanici.Ad, kullanici.Soyad);
            }
        }


        public ProfilIndexViewModel()
        {
            //kullanici = new Kullanici();
        }
    }
}