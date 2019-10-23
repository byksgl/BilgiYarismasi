using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BilgiYarismasi.Web.Models
{
    public class HomeIndexViewModel1
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public int Puan { get; set; }
    }
    public class HomeIndexViewModel2
    {
        public HomeIndexViewModel2()
        {
            multi = new List<HomeIndexViewModel1>();
            single = new List<HomeIndexViewModel1>();
        }
        public List<HomeIndexViewModel1> multi { get; set; }
        public List<HomeIndexViewModel1> single { get; set; }
    }
}