using BilgiYarismasi.DAL;
using BilgiYarismasi.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BilgiYarismasi.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HomeIndexViewModel2 model = new HomeIndexViewModel2();

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                model.multi = ent.VKullaniciPuanTipli
                    .Where(p => p.Tip == (int)EnmMasaTipi.MultiPlayerMasa)
                    .OrderByDescending(p=>p.PUAN)
                    .Take(5)
                    .Select(p => new HomeIndexViewModel1
                    {
                        Ad = p.Ad,
                        Soyad = p.Soyad,
                        Puan = p.PUAN.Value,
                    }).ToList();
                model.single = ent.VKullaniciPuanTipli
                    .Where(p => p.Tip == (int)EnmMasaTipi.SinglePlaeyerMasa)
                    .OrderByDescending(p => p.PUAN)
                    .Take(5)
                    .Select(p => new HomeIndexViewModel1
                    {
                        Ad = p.Ad,
                        Soyad = p.Soyad,
                        Puan = p.PUAN.Value,
                    }).ToList();

            }

            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}