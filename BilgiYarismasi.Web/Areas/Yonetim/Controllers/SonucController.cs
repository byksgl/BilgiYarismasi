using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BilgiYarismasi.DAL;
using BilgiYarismasi.Web.Areas.Yonetim.Models;

namespace BilgiYarismasi.Web.Areas.Yonetim.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SonucController : Controller
    {
        // GET: Yonetim/Sonuc
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Konu()
        {
            List<Konu> model = new List<DAL.Konu>();
            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                model = ent.Konu.OrderBy(p => p.Ad).ToList();
            }
            return View(model);
        }

        public ActionResult KonuSonuc(Guid id)
        {
            KonuSonucViewModel ksvm = new KonuSonucViewModel();

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                ksvm.konu = ent.Konu.Where(p => p.Id == id).FirstOrDefault();

                var yarismalar = ent.Yarisma
                    .Where(p => p.MasaKullanici.Masa.KonuId == ksvm.konu.Id)
                    .ToList();

                foreach (var yarisma in yarismalar)
                {
                    KonuSonucDetayViewModel detay = new KonuSonucDetayViewModel();
                    detay.yarisma = yarisma;
                    detay.Tip = yarisma.MasaKullanici.Masa.Tip == (int)EnmMasaTipi.SinglePlaeyerMasa ? "Single" : "Multi";
                    string kullaniciId = yarisma.MasaKullanici.KullaniciId.ToString();
                    detay.user = ent.AspNetUsers
                        .Where(p => p.Id == kullaniciId)
                        .FirstOrDefault();

                    detay.kullanici = ent.Kullanici
                        .Where(p => p.Id == yarisma.MasaKullanici.KullaniciId)
                        .FirstOrDefault();

                    var sorular = ent.YarismaSoru
                        .Where(p => p.YarismaId == yarisma.Id)
                        .ToList();

                    detay.cevaplanan = 0;
                    detay.dogru = 0;
                    detay.yanlis = 0;

                    foreach (var soru in sorular)
                    {
                        if (soru.CevapId != null)
                        {
                            detay.cevaplanan++;
                            var cevap = ent.Cevap.Where(p => p.Id == soru.CevapId).FirstOrDefault();
                            if (cevap.Dogrumu)
                            {
                                detay.dogru++;
                            }
                            else
                            {
                                detay.yanlis++;
                            }

                        }
                    }

                    ksvm.detaylar.Add(detay);

                }

            }

            return View(ksvm);
        }

        public ActionResult Kullanici()
        {
            List<Kullanici> model = new List<Kullanici>();
            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                model = ent.Kullanici.OrderBy(p => p.Ad).ToList();
            }
            return View(model);
        }

        public ActionResult KullaniciSonuc(Guid id)
        {
            KullaniciSonucViewModel ksvm = new KullaniciSonucViewModel();

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                string kullaniciId = id.ToString();
                ksvm.kullanici = ent.Kullanici.Where(p => p.Id == id).FirstOrDefault();
                ksvm.user = ent.AspNetUsers.Where(p => p.Id == kullaniciId).FirstOrDefault();

                var yarismalar = ent.Yarisma
                    .Where(p => p.MasaKullanici.KullaniciId == ksvm.kullanici.Id)
                    .OrderBy(p => p.BaslangicTarihi)
                    .ToList();

                foreach (var yarisma in yarismalar)
                {
                    KullaniciSonucDetayViewModel detay = new KullaniciSonucDetayViewModel();
                    detay.yarisma = yarisma;

                    detay.konu = ent.Konu.Where(p => p.Id == yarisma.MasaKullanici.Masa.KonuId).FirstOrDefault();

                    var sorular = ent.YarismaSoru
                        .Where(p => p.YarismaId == yarisma.Id)
                        .ToList();

                    detay.cevaplanan = 0;
                    detay.dogru = 0;
                    detay.yanlis = 0;

                    foreach (var soru in sorular)
                    {
                        if (soru.CevapId != null)
                        {
                            detay.cevaplanan++;
                            var cevap = ent.Cevap.Where(p => p.Id == soru.CevapId).FirstOrDefault();
                            if (cevap != null)
                            {
                                if (cevap.Dogrumu)
                                {
                                    detay.dogru++;
                                }
                                else
                                {
                                    detay.yanlis++;
                                }
                            }
                        }
                    }

                    ksvm.detaylar.Add(detay);

                }

            }

            return View(ksvm);
        }

    }
}