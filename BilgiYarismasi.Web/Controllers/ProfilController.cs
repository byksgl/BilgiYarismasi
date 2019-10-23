using BilgiYarismasi.DAL;
using BilgiYarismasi.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace BilgiYarismasi.Web.Controllers
{
    [Authorize]
    public class ProfilController : Controller
    {
        // GET: Profil
        public ActionResult Index()
        {
            ProfilIndexViewModel model = new ProfilIndexViewModel();
            model.ToplamYarismaSayisi = 0;
            model.ToplamDogruSayisi = 0;
            model.ToplamYanlisSayisi = 0;
            model.SingleBasariPuani = 0;

            using (BilgiYarismasiEntities2 ent = new DAL.BilgiYarismasiEntities2())
            {
                Guid userId = Guid.Parse(User.Identity.GetUserId());
                model.kullanici = ent.Kullanici
                    .Where(p => p.Id == userId)
                    .FirstOrDefault();

                var yarismalar = ent.Yarisma.Where(p => p.MasaKullanici.KullaniciId == userId).ToList();
                model.ToplamYarismaSayisi = yarismalar.Count();

                List<YarismaSoru> yarismaSorular = ent.YarismaSoru
                    .Where(p => p.Yarisma.MasaKullanici.KullaniciId == userId
                        && ent.Yarisma.Where(r => r.MasaKullanici.KullaniciId == userId).Any(t => t.Id == p.YarismaId))
                    .ToList();

                model.ToplamDogruSayisi = yarismaSorular.Where(p => p.CevapId!=null && p.Cevap.Dogrumu).Count();

                model.ToplamYanlisSayisi = yarismaSorular
                    .Where(p => p.CevapId != null && !p.Cevap.Dogrumu)
                    .Count();

                var singleSorular = yarismaSorular.Where(p => p.Yarisma.MasaKullanici.Masa.Tip == (int)EnmMasaTipi.SinglePlaeyerMasa).ToList();
                var multiSorular = yarismaSorular.Where(p => p.Yarisma.MasaKullanici.Masa.Tip == (int)EnmMasaTipi.MultiPlayerMasa).ToList();

                model.SingleBasariPuani = singleSorular.Sum(p => p.Puan).HasValue ? singleSorular.Sum(p => p.Puan).Value : 0;
            }

            return View(model);
        }
    }
}