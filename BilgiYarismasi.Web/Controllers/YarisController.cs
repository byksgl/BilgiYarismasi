using BilgiYarismasi.DAL;
using BilgiYarismasi.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Text;

namespace BilgiYarismasi.Web.Controllers
{
    [Authorize]
    public class YarisController : Controller
    {
        // GET: Yaris
        public ActionResult Index()
        {
            YarisIndexViewModel model = new YarisIndexViewModel();

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                Guid userId = User.Identity.GetUserId().toGuid();

                Yarisma sonSingleYarisma = ent.Yarisma
                    .Where(p => p.MasaKullanici.Masa.Tip == (int)EnmMasaTipi.SinglePlaeyerMasa)
                    .Where(p => p.MasaKullanici.KullaniciId == userId)
                    .OrderByDescending(p => p.BaslangicTarihi)
                    .FirstOrDefault();

                Yarisma sonMultiYarisma = ent.Yarisma
                    .Where(p => p.MasaKullanici.Masa.Tip == (int)EnmMasaTipi.MultiPlayerMasa)
                    .Where(p => p.MasaKullanici.KullaniciId == userId)
                    .OrderByDescending(p => p.BaslangicTarihi)
                    .FirstOrDefault();

                if (sonSingleYarisma == null)
                {
                    return View(model);
                }

                List<YarismaSoru> sonSorular = ent.YarismaSoru
                    .Where(p => p.YarismaId == sonSingleYarisma.Id)
                    .ToList();

                model.EnSonSinglePuan = sonSorular.Sum(p => p.Puan).HasValue ? sonSorular.Sum(p => p.Puan).Value : 0;

                var enYuksekSingle = ent.VKullaniciPuan
                    .Where(p => p.KullaniciId == userId)
                    .OrderByDescending(p => p.PUAN)
                    .FirstOrDefault();

                model.EnYuksekSinglePuan = enYuksekSingle.PUAN.HasValue ? enYuksekSingle.PUAN.Value : 0;

                if (sonMultiYarisma != null)
                {
                    var multiKisiSayisi = ent.YarismaSoru
                    .Where(p => p.YarismaId == sonMultiYarisma.Id)
                    .Sum(p => p.Puan);
                    model.MultiEnsonPuan = multiKisiSayisi.HasValue?multiKisiSayisi.Value:0;
                    var enYuksekMulti = ent.VKullaniciPuanTipli
                        .Where(p => p.KullaniciId == userId && p.Tip == (int)EnmMasaTipi.MultiPlayerMasa)
                        .OrderByDescending(p => p.PUAN)
                        .FirstOrDefault();
                    model.MultiEnYuksekPuan = enYuksekMulti == null ? 0 : enYuksekMulti.PUAN.HasValue ? enYuksekMulti.PUAN.Value : 0;
                }

            }

            return View(model);
        }

        public ActionResult SingleKonu()
        {
            SingleKonuViewModel model = new SingleKonuViewModel();
            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                model.konular = ent.Konu
                    .Where(p => p.Soru.Count() > 9)
                    .OrderBy(p => p.Ad)
                    .ToList();
            }

            return View(model);
        }

        public ActionResult SingleBasla(Guid id)
        {
            Session[SessionNames.KONU] = id;

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                Konu konu = ent.Konu.Where(p => p.Id == id).FirstOrDefault();

                Masa masa = new Masa();
                masa.Id = Guid.NewGuid();
                masa.KisiSayisi = 1;
                masa.KonuId = id;
                masa.SoruSayisi = 0;
                masa.SureDk = konu.SureDk;
                masa.Tip = (int)EnmMasaTipi.SinglePlaeyerMasa;
                masa.KayitTarihi = DateTime.Now;
                masa.BaslangicTarihi = masa.KayitTarihi;

                ent.Masa.Add(masa);

                MasaKullanici masaKullanici = new MasaKullanici();
                masaKullanici.Id = Guid.NewGuid();
                masaKullanici.KullaniciId = User.Identity.GetUserId().toGuid();
                masaKullanici.MasaId = masa.Id;
                masaKullanici.SiraNo = 1;
                masaKullanici.Tip = (int)enmMasaKullaniciTipi.Yonetici;
                masaKullanici.KayitTarihi = DateTime.Now;

                ent.MasaKullanici.Add(masaKullanici);

                Yarisma yarisma = new Yarisma();
                yarisma.BaslangicTarihi = DateTime.Now;
                yarisma.Id = Guid.NewGuid();
                yarisma.MasaKullaniciId = masaKullanici.Id;
                yarisma.SoruSayisi = 0;
                yarisma.SureDk = masa.SureDk;

                ent.Yarisma.Add(yarisma);
                ent.SaveChanges();

                yarismayaSoruEkle(ent, yarisma);

                ent.SaveChanges();

                Session[SessionNames.MASA] = masa.Id;

            }

            return RedirectToAction("SingleYaris");
        }

        public ActionResult SingleYaris()
        {

            SingleYarisViewModel model = new SingleYarisViewModel();

            if (Utils.AktifMasa == null)
            {
                return RedirectToAction("index");
            }

            Masa aktifMasa = Utils.AktifMasa;

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                Masa masa = ent.Masa.Where(p => p.Id == aktifMasa.Id).FirstOrDefault();

                Guid userId = User.Identity.GetUserId().toGuid();

                model.SiraNo = masa.MasaKullanici.FirstOrDefault()
                    .Yarisma.OrderBy(p => p.BaslangicTarihi).FirstOrDefault()
                    .YarismaSoru.OrderBy(p => p.SiraNo).LastOrDefault()
                    .SiraNo.Value;

                if (masa.BaslangicTarihi.Value.AddMinutes(masa.SureDk) < DateTime.Now)
                {
                    return RedirectToAction("SingleBitir");
                }

                model.Sorusu = masa.MasaKullanici.FirstOrDefault()
                    .Yarisma.OrderBy(p => p.BaslangicTarihi).FirstOrDefault()
                    .YarismaSoru.OrderBy(p => p.SiraNo).LastOrDefault()
                    .Soru.Sorusu;

                model.cevaplar = masa.MasaKullanici.FirstOrDefault()
                    .Yarisma.OrderBy(p => p.BaslangicTarihi).FirstOrDefault()
                    .YarismaSoru.OrderBy(p => p.SiraNo).LastOrDefault()
                    .YarismaSoruCevap
                    .Select(p => new CevapKisaModel
                    {
                        Cevabi = p.Cevap.Cevabi,
                        Id = p.Cevap.Id
                    })
                    .ToList();

                var oncekiSoru = ent.YarismaSoru
                    .Where(p => p.Yarisma.MasaKullanici.MasaId == masa.Id
                        && p.Yarisma.MasaKullanici.KullaniciId == userId
                        && p.SiraNo == model.SiraNo - 1)
                    .FirstOrDefault();

                if (oncekiSoru == null)
                {
                    model.OncekiDogrumu = 0;
                }
                else
                {
                    model.OncekiDogrumu = oncekiSoru.Cevap.Dogrumu ? 1 : 2;
                }

            }

            ViewBag.seconds = (int)((aktifMasa.BaslangicTarihi.Value.AddMinutes(aktifMasa.SureDk) - DateTime.Now).TotalSeconds);

            return View(model);
        }

        [HttpPost]
        public ActionResult SingleYaris(SingleYarisViewModel model)
        {
            if (model.cevapId == null || model.cevapId == Guid.Empty)
            {
                return RedirectToAction("SingleYaris");
            }

            var aktifMasa = Utils.AktifMasa;
            if (aktifMasa == null)
            {
                return RedirectToAction("index");
            }

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {

                Masa masa = ent.Masa.Where(p => p.Id == aktifMasa.Id).FirstOrDefault();
                Yarisma yarisma = masa.MasaKullanici.FirstOrDefault()
                    .Yarisma.OrderBy(p => p.BaslangicTarihi).FirstOrDefault();
                YarismaSoru yarismaSoru = yarisma.YarismaSoru.OrderBy(p => p.SiraNo).LastOrDefault();
                Cevap cevap = ent.Cevap.Where(p => p.Id == model.cevapId).FirstOrDefault();

                if (yarismaSoru.SiraNo != model.SiraNo)
                {
                    return RedirectToAction("SingleYaris");
                }

                yarismaSoru.CevapId = model.cevapId;

                if (cevap.Dogrumu)
                {
                    yarismaSoru.Puan = YarismaDegerler.BaslangicPuani;
                }

                ent.SaveChanges();


                if (yarisma.YarismaSoru.Count() >= masa.Konu.Soru.Count())
                {
                    return RedirectToAction("SingleBitir");
                }

                yarismayaSoruEkle(ent, yarisma);

                ent.SaveChanges();

            }

            return RedirectToAction("SingleYaris");
        }

        public ActionResult SingleBitir()
        {
            SingleBitirViewModel model = new SingleBitirViewModel();

            var aktifMasa = Utils.AktifMasa;
            if (aktifMasa == null)
            {
                return RedirectToAction("index");
            }

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                Masa masa = ent.Masa.Where(p => p.Id == aktifMasa.Id).FirstOrDefault();
                Yarisma yarisma = masa.MasaKullanici.FirstOrDefault()
                    .Yarisma.FirstOrDefault();

                model.cevaplanan = yarisma.SoruSayisi;
                model.dogruSayisi = 0;
                model.yanlisSayisi = 0;

                foreach (var item in yarisma.YarismaSoru)
                {
                    if (item.CevapId.HasValue)
                    {
                        if (item.Cevap.Dogrumu)
                        {
                            model.dogruSayisi++;
                        }
                        else
                        {
                            model.yanlisSayisi++;
                        }
                    }
                }

                masa.BitisTarihi = DateTime.Now;
                yarisma.BitisTarihi = masa.BitisTarihi;
                ent.SaveChanges();

            }

            return View(model);
        }


        public ActionResult MultiKonu()
        {
            MasaTemizle();
            MultiMasaKullaniciTemizle();
            MultiKonuViewModel model = new MultiKonuViewModel();
            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                model.konular = ent.Konu
                    .Where(p => p.Soru.Count() > 9)
                    .OrderBy(p => p.Ad)
                    .ToList()
                    .Select(p => new KonuKisiModel
                    {
                        konu = p,
                        KisiSayisi = ent.MasaKullanici
                            .Where(t => t.Masa.Tip == (int)EnmMasaTipi.MultiPlayerMasa
                                && !t.Masa.BitisTarihi.HasValue
                                && t.Masa.KonuId == p.Id
                            ).Count()
                    })
                    .ToList();

            }

            return View(model);
        }

        public ActionResult MasaSec(Guid id)
        {
            MasaSecViewModel model = new MasaSecViewModel();

            MasaTemizle();
            MultiMasaKullaniciTemizle();

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                Konu konu = ent.Konu.FirstOrDefault(p => p.Id == id);

                model.KonuId = id;

                model.masalar = ent.Masa
                    .Where(p => p.KonuId == id && p.Tip == (int)EnmMasaTipi.MultiPlayerMasa
                        && !p.BitisTarihi.HasValue
                    )
                    .Select(p => new MasaModel
                    {
                        Id = p.Id,
                        masa = p,
                        KisiSayisi = ent.MasaKullanici
                            .Where(t => t.MasaId == p.Id)
                            .Count()
                    })
                    .ToList();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult MasaAc(Guid konuId, int kisiSayisi, int sure, int soruSayisi)
        {

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {

                Masa masa = new Masa();
                masa.KayitTarihi = DateTime.Now;
                masa.Id = Guid.NewGuid();
                masa.KisiSayisi = kisiSayisi;
                masa.KonuId = konuId;
                masa.SoruSayisi = soruSayisi;
                masa.SureDk = sure;
                masa.Tip = (int)EnmMasaTipi.MultiPlayerMasa;

                ent.Masa.Add(masa);

                MasaKullanici masaKullanici = new MasaKullanici();
                masaKullanici.Id = Guid.NewGuid();
                masaKullanici.KullaniciId = User.Identity.GetUserId().toGuid();
                masaKullanici.MasaId = masa.Id;
                masaKullanici.SiraNo = 1;
                masaKullanici.Tip = (int)enmMasaKullaniciTipi.Yonetici;
                masaKullanici.KayitTarihi = DateTime.Now;

                ent.MasaKullanici.Add(masaKullanici);

                // diger masalardaki hesaplari cikar

                List<MasaKullanici> masaKullanicilari = ent.MasaKullanici
                    .Where(p => p.KullaniciId == masaKullanici.KullaniciId
                        && p.Id != masaKullanici.Id
                        && !p.Masa.BaslangicTarihi.HasValue
                        )
                    .ToList();

                foreach (var item in masaKullanicilari)
                {
                    ent.MasaKullanici.Remove(item);
                }

                Session[SessionNames.MASA] = masa.Id;

                ent.SaveChanges();

                return RedirectToAction("MasaBekle", new { id = masa.Id });

            }

            return View();
        }

        public ActionResult MasaBekle(Guid id)
        {
            MasaBekleViewModel model = new MasaBekleViewModel();

            return View(model);
        }

        [OutputCache(Duration = 0)]
        public ActionResult getMasaBekle()
        {
            Guid masaId = Session[SessionNames.MASA].ToString().toGuid();
            Guid userId = User.Identity.GetUserId().toGuid();

            MultiMasaKullaniciTemizle();

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {

                Masa masa = ent.Masa.FirstOrDefault(p => p.Id == masaId);

                var masaKullanicilar = ent.MasaKullanici
                    .Where(p => p.MasaId == masaId)
                    .OrderBy(p => p.SiraNo)
                    .ToList();

                int mSiraNo = 0;
                foreach (var item in masaKullanicilar)
                {
                    mSiraNo++;
                    item.Tip = mSiraNo == 1 ? (int)enmMasaKullaniciTipi.Yonetici : (int)enmMasaKullaniciTipi.Yarismaci;
                    if (mSiraNo > masa.KisiSayisi)
                    {
                        item.Tip = (int)enmMasaKullaniciTipi.Izleyici;
                    }
                    item.SiraNo = mSiraNo;
                }

                var masaKullanici = masaKullanicilar.FirstOrDefault(p => p.KullaniciId == userId);
                masaKullanici.Ping = DateTime.Now;
                ent.SaveChanges();

                if (masaKullanicilar.Count >= masa.KisiSayisi)
                {
                    return Content("1");
                }


                StringBuilder sb = new StringBuilder();

                int i = 0;
                foreach (var item in masaKullanicilar)
                {
                    i++;
                    sb.AppendFormat("{1} nolu yarışmacı yarışmaya katıldı: {0} <br />", item.Kullanici.Nick, i);
                }

                return Content(sb.ToString());
            }

        }

        public ActionResult MasaGir(Guid id)
        {

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                var masa = ent.Masa.FirstOrDefault(p => p.Id == id);

                MasaKullanici masaKullanici = new MasaKullanici();
                masaKullanici.Id = Guid.NewGuid();
                masaKullanici.KullaniciId = User.Identity.GetUserId().toGuid();
                masaKullanici.MasaId = masa.Id;
                masaKullanici.SiraNo = 2;
                masaKullanici.Tip = (int)enmMasaKullaniciTipi.Yarismaci;
                masaKullanici.KayitTarihi = DateTime.Now;

                ent.MasaKullanici.Add(masaKullanici);

                // diger masalardaki hesaplari cikar

                List<MasaKullanici> masaKullanicilari = ent.MasaKullanici
                    .Where(p => p.KullaniciId == masaKullanici.KullaniciId
                        && p.Id != masaKullanici.Id
                        && !p.Masa.BaslangicTarihi.HasValue
                        )
                    .ToList();

                foreach (var item in masaKullanicilari)
                {
                    ent.MasaKullanici.Remove(item);
                }

                ent.SaveChanges();

                Session[SessionNames.MASA] = masa.Id;

                return RedirectToAction("MasaBekle", new { id = masa.Id });

            }

        }

        public ActionResult MultiBasla()
        {
            Guid masaId = Session[SessionNames.MASA].ToString().toGuid();
            Guid userId = User.Identity.GetUserId().toGuid();

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {

                var masa = ent.Masa.FirstOrDefault(p => p.Id == masaId);
                var adminKullanici = masa.MasaKullanici.Where(p => p.Tip == (int)enmMasaKullaniciTipi.Yonetici).FirstOrDefault();

                if (adminKullanici.KullaniciId != userId)
                {
                    // yarisma baslamismi kontrol edecekler
                    return RedirectToAction("MultiDevam");
                }

                var masaKullanicilar = masa.MasaKullanici.Where(p => p.Tip != (int)enmMasaKullaniciTipi.Izleyici).ToList();

                foreach (var masaKullanici in masaKullanicilar)
                {
                    Yarisma yarisma = new Yarisma();
                    yarisma.BaslangicTarihi = DateTime.Now;
                    yarisma.Id = Guid.NewGuid();
                    yarisma.MasaKullaniciId = masaKullanici.Id;
                    yarisma.SoruSayisi = 0;
                    yarisma.SureDk = masa.SureDk;

                    ent.Yarisma.Add(yarisma);
                    ent.SaveChanges();
                }

                masayaSoruEkle(ent, masa);

                masa.BaslangicTarihi = DateTime.Now;

                ent.SaveChanges();
            }

            return RedirectToAction("MultiDevam");
        }

        public ActionResult MultiDevam()
        {
            MultiDevamViewModel model = new MultiDevamViewModel();

            Guid masaId = Session[SessionNames.MASA].ToString().toGuid();
            Guid userId = User.Identity.GetUserId().toGuid();

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                var masa = ent.Masa.FirstOrDefault(p => p.Id == masaId);
                var masaKullanici = masa.MasaKullanici.Where(p => p.KullaniciId == userId).FirstOrDefault();

                model.YarismaBasladimi = masa.BaslangicTarihi.HasValue;

                if (model.YarismaBasladimi)
                {
                    var sonSoru = masaKullanici.Yarisma.FirstOrDefault()
                        .YarismaSoru.OrderByDescending(p => p.SiraNo)
                        .FirstOrDefault();

                    if (sonSoru != null)
                    {
                        if (sonSoru.Cevap != null)
                        {
                            model.OncekiSoru = sonSoru;
                            model.oncekiCevapDogrumu = sonSoru.Cevap.Dogrumu;
                        }
                    }

                }
                var aktifMasa = Utils.AktifMasa;
                ViewBag.seconds = (int)((aktifMasa.BaslangicTarihi.Value.AddMinutes(aktifMasa.SureDk) - DateTime.Now).TotalSeconds);
            }

            return View(model);
        }

        public ActionResult getMultiDevam()
        {
            if (Session[SessionNames.MASA] == null)
            {
                return Content("Soru hazırlanıyor...");
            }
            Guid masaId = Session[SessionNames.MASA].ToString().toGuid();
            Guid userId = User.Identity.GetUserId().toGuid();

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                var masa = ent.Masa.FirstOrDefault(p => p.Id == masaId);
                var masaKullanici = masa.MasaKullanici.Where(p => p.KullaniciId == userId).FirstOrDefault();
                var yarisma = masaKullanici.Yarisma.FirstOrDefault();

                if (!masa.BaslangicTarihi.HasValue)
                {
                    return Content("Masa Hazırlanıyor...~ ");
                }
                var fark1 = (masa.BaslangicTarihi.Value.AddMinutes(masa.SureDk) - DateTime.Now);
                var farkStr = string.Format("~{0}:{1}", fark1.Minutes, fark1.Seconds);

                if (masa.BitisTarihi.HasValue)
                {
                    yarisma.BitisTarihi = masa.BitisTarihi;
                    ent.SaveChanges();
                    return Content("2"+farkStr);
                }

                var sonSoru = ent.YarismaSoru
                    .Where(p => p.Yarisma.MasaKullaniciId == masaKullanici.Id)
                    .OrderByDescending(p => p.SiraNo)
                    .FirstOrDefault();

                if (sonSoru == null)
                {
                    return Content("Soru hazırlanıyor..." + farkStr);
                }

                if (sonSoru.SiraNo == masa.SoruSayisi)
                {
                    return Content("2" + farkStr);
                }

                if (sonSoru.Cevap != null)
                {
                    // tum  kullanicilar soruyu cevaplandi ise admin yeni sorulari olusturur
                    var cevaplanmayanSorular = ent.YarismaSoru
                        .Where(p => p.Yarisma.MasaKullanici.MasaId == masaId && p.SoruId == sonSoru.SoruId)
                        .Where(p => !p.CevapId.HasValue)
                        .ToList();

                    if (cevaplanmayanSorular.Count == 0)
                    {
                        var adminKullanici = masa.MasaKullanici.Where(p => p.Tip == (int)enmMasaKullaniciTipi.Yonetici).FirstOrDefault();
                        if (adminKullanici.KullaniciId == userId)
                        {
                            masayaSoruEkle(ent, masa);
                        }
                    }

                    return Content("Diğer kullanıcıların cevaplarını tamamlamaları bekleniyor" + farkStr);
                }

                if (sonSoru.GosterimTarihi <= DateTime.Now)
                {
                    return Content("1" + farkStr);
                }
                else
                {
                    return Content("Sıradaki soru geliyor, bekleyiniz..." + farkStr);
                }

                return Content("Belirsiz durum bekleniyor" + farkStr);

            }

        }


        public ActionResult MultiYaris()
        {

            MultiYarisViewModel model = new MultiYarisViewModel();

            if (Utils.AktifMasa == null)
            {
                return RedirectToAction("index");
            }

            Masa aktifMasa = Utils.AktifMasa;

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                Masa masa = ent.Masa.Where(p => p.Id == aktifMasa.Id).FirstOrDefault();

                Guid userId = User.Identity.GetUserId().toGuid();

                model.SiraNo = masa.MasaKullanici.FirstOrDefault()
                    .Yarisma.OrderBy(p => p.BaslangicTarihi).FirstOrDefault()
                    .YarismaSoru.OrderBy(p => p.SiraNo).LastOrDefault()
                    .SiraNo.Value;

                if (masa.BaslangicTarihi.Value.AddMinutes(masa.SureDk) < DateTime.Now)
                {
                    return RedirectToAction("MultiBitir");
                }

                model.Sorusu = masa.MasaKullanici.FirstOrDefault()
                    .Yarisma.OrderBy(p => p.BaslangicTarihi).FirstOrDefault()
                    .YarismaSoru.OrderBy(p => p.SiraNo).LastOrDefault()
                    .Soru.Sorusu;

                model.cevaplar = masa.MasaKullanici.FirstOrDefault()
                    .Yarisma.OrderBy(p => p.BaslangicTarihi).FirstOrDefault()
                    .YarismaSoru.OrderBy(p => p.SiraNo).LastOrDefault()
                    .YarismaSoruCevap
                    .Select(p => new CevapKisaModel
                    {
                        Cevabi = p.Cevap.Cevabi,
                        Id = p.Cevap.Id
                    })
                    .ToList();

                var oncekiSoru = ent.YarismaSoru
                    .Where(p => p.Yarisma.MasaKullanici.MasaId == masa.Id
                        && p.Yarisma.MasaKullanici.KullaniciId == userId
                        && p.SiraNo == model.SiraNo - 1)
                    .FirstOrDefault();

                if (oncekiSoru == null)
                {
                    model.OncekiDogrumu = 0;
                }
                else
                {
                    model.OncekiDogrumu = oncekiSoru.Cevap.Dogrumu ? 1 : 2;
                }

            }

            ViewBag.seconds = (int)((aktifMasa.BaslangicTarihi.Value.AddMinutes(aktifMasa.SureDk) - DateTime.Now).TotalSeconds);

            return View(model);
        }

        [HttpPost]
        public ActionResult MultiYaris(MultiYarisViewModel model)
        {
            if (model.cevapId == null || model.cevapId == Guid.Empty)
            {
                return RedirectToAction("MultiDevam");
            }

            var aktifMasa = Utils.AktifMasa;
            if (aktifMasa == null)
            {
                return RedirectToAction("index");
            }

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                Guid userId = User.Identity.GetUserId().toGuid();
                Masa masa = ent.Masa.Where(p => p.Id == aktifMasa.Id).FirstOrDefault();
                Yarisma yarisma = masa.MasaKullanici
                    .Where(p => p.KullaniciId == userId)
                    .FirstOrDefault()
                    .Yarisma.OrderBy(p => p.BaslangicTarihi).FirstOrDefault();
                YarismaSoru yarismaSoru = yarisma.YarismaSoru.OrderBy(p => p.SiraNo).LastOrDefault();
                Cevap cevap = ent.Cevap.Where(p => p.Id == model.cevapId).FirstOrDefault();

                if (yarismaSoru.SiraNo != model.SiraNo)
                {
                    return RedirectToAction("MultiDevam");
                }

                yarismaSoru.CevapId = model.cevapId;

                if (cevap.Dogrumu)
                {
                    // onceki dogru cevaplar kontrol edilir
                    var digerDogrular = ent.YarismaSoru
                        .Where(p => p.Yarisma.MasaKullanici.MasaId == masa.Id
                            && p.SoruId == yarismaSoru.SoruId
                            && p.Cevap.Dogrumu
                        ).Count();

                    yarismaSoru.Puan = Convert.ToInt32(YarismaDegerler.BaslangicPuani * Math.Pow(2, masa.KisiSayisi - digerDogrular - 1));
                }
                else
                {
                    yarismaSoru.Puan = YarismaDegerler.BaslangicPuani * (-1);
                }

                ent.SaveChanges();


                if (yarisma.YarismaSoru.Count() >= masa.Konu.Soru.Count())
                {
                    return RedirectToAction("MultiBitir");
                }

                //yarismayaSoruEkle(ent, yarisma);
                //ent.SaveChanges();

            }

            return RedirectToAction("MultiDevam");
        }

        public ActionResult MultiBitir()
        {
            MultiBitirViewModel model = new MultiBitirViewModel();

            if (Session[SessionNames.MASA] == null)
            {
                return Content("Bitmis");
            }


            Guid masaId = Session[SessionNames.MASA].ToString().toGuid();
            Guid userId = User.Identity.GetUserId().toGuid();

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                Masa masa = ent.Masa.Where(p => p.Id == masaId).FirstOrDefault();
                Yarisma yarisma = masa.MasaKullanici
                    .Where(p=>p.KullaniciId==userId)
                    .FirstOrDefault()
                    .Yarisma.FirstOrDefault();

                int? toplamPuan = ent.YarismaSoru
                    .Where(p => p.Yarisma.MasaKullanici.KullaniciId == userId)
                    .Where(p => p.Yarisma.BitisTarihi.HasValue)
                    .Sum(p => p.Puan);

                int? yarismaPuan = ent.YarismaSoru
                    .Where(p => p.Yarisma.MasaKullanici.KullaniciId == userId)
                    .Where(p => p.YarismaId==yarisma.Id)
                    .Sum(p => p.Puan);

                model.toplam = yarisma.SoruSayisi;
                model.cevaplanan = yarisma.YarismaSoru.Where(p=>p.CevapId.HasValue).Count();
                model.dogruSayisi = 0;
                model.yanlisSayisi = 0;
                model.yarismadanKazandiPuan = yarismaPuan.HasValue?yarismaPuan.Value:0;
                model.toplamPuan = toplamPuan.HasValue ? toplamPuan.Value : 0;

                foreach (var item in yarisma.YarismaSoru)
                {
                    if (item.CevapId.HasValue)
                    {
                        if (item.Cevap.Dogrumu)
                        {
                            model.dogruSayisi++;
                        }
                        else
                        {
                            model.yanlisSayisi++;
                        }
                    }
                }

                masa.BitisTarihi = DateTime.Now;
                yarisma.BitisTarihi = masa.BitisTarihi;
                ent.SaveChanges();

            }

            return View(model);
        }




        #region Private islemler

        private void yarismayaSoruEkle(BilgiYarismasiEntities2 ent, Yarisma yarisma)
        {

            var eskiSorular = yarisma.YarismaSoru
                .Select(p => p.SoruId)
                .ToList();

            YarismaSoru yarismaSoru = new YarismaSoru();
            yarismaSoru.Id = Guid.NewGuid();
            yarismaSoru.SiraNo = yarisma.YarismaSoru.Count() + 1;
            yarismaSoru.SoruId = yarisma.MasaKullanici.Masa.Konu.Soru
                .Where(p => !eskiSorular.Any(t => t == p.Id))
                .OrderBy(p => Guid.NewGuid()).FirstOrDefault().Id;
            yarismaSoru.YarismaId = yarisma.Id;

            if (yarisma.MasaKullanici.Masa.Tip == (int)EnmMasaTipi.MultiPlayerMasa)
            {
                yarismaSoru.GosterimTarihi = DateTime.Now.AddSeconds(2);
            }

            ent.YarismaSoru.Add(yarismaSoru);

            var mDogruCevap = yarismaSoru.Soru.Cevap.Where(p => p.Dogrumu).FirstOrDefault();

            //yarismaSoru.CevapId = mDogruCevap.Id;

            var cevaplar = yarismaSoru.Soru.Cevap
                .Where(p => !p.Dogrumu)
                .OrderBy(p => Guid.NewGuid())
                .Take(yarismaSoru.Soru.SecenekSayisi.HasValue ? yarismaSoru.Soru.SecenekSayisi.Value - 1 : 3)
                .ToList();

            cevaplar.Add(mDogruCevap);
            cevaplar = cevaplar.OrderBy(p => Guid.NewGuid()).ToList();

            foreach (var cevap in cevaplar)
            {
                YarismaSoruCevap yarismaSoruCevap = new YarismaSoruCevap();
                yarismaSoruCevap.CevapId = cevap.Id;
                yarismaSoruCevap.Id = Guid.NewGuid();
                yarismaSoruCevap.YarismaSoruId = yarismaSoru.Id;

                ent.YarismaSoruCevap.Add(yarismaSoruCevap);

            }

            yarisma.SoruSayisi++;
        }

        private void masayaSoruEkle(BilgiYarismasiEntities2 ent, Masa masa)
        {
            var masaKullanicilar = masa.MasaKullanici.Where(p => p.Tip != (int)enmMasaKullaniciTipi.Izleyici).ToList();

            var eskiSorular = masa.MasaKullanici.FirstOrDefault().Yarisma.FirstOrDefault().YarismaSoru
                .Select(p => p.SoruId)
                .ToList();

            var yeniSoruId = masa.Konu.Soru
                    .Where(p => !eskiSorular.Any(t => t == p.Id))
                    .OrderBy(p => Guid.NewGuid()).FirstOrDefault().Id;

            foreach (var mitem in masaKullanicilar)
            {
                Yarisma yarisma = mitem.Yarisma.FirstOrDefault();

                YarismaSoru yarismaSoru = new YarismaSoru();
                yarismaSoru.Id = Guid.NewGuid();
                yarismaSoru.SiraNo = yarisma.YarismaSoru.Count() + 1;
                yarismaSoru.SoruId = yeniSoruId;
                yarismaSoru.YarismaId = yarisma.Id;

                if (yarisma.MasaKullanici.Masa.Tip == (int)EnmMasaTipi.MultiPlayerMasa)
                {
                    yarismaSoru.GosterimTarihi = DateTime.Now.AddSeconds(2);
                }

                ent.YarismaSoru.Add(yarismaSoru);

                var mDogruCevap = yarismaSoru.Soru.Cevap.Where(p => p.Dogrumu).FirstOrDefault();

                //yarismaSoru.CevapId = mDogruCevap.Id;

                var cevaplar = yarismaSoru.Soru.Cevap
                    .Where(p => !p.Dogrumu)
                    .OrderBy(p => Guid.NewGuid())
                    .Take(yarismaSoru.Soru.SecenekSayisi.HasValue ? yarismaSoru.Soru.SecenekSayisi.Value - 1 : 3)
                    .ToList();

                cevaplar.Add(mDogruCevap);
                cevaplar = cevaplar.OrderBy(p => Guid.NewGuid()).ToList();

                foreach (var cevap in cevaplar)
                {
                    YarismaSoruCevap yarismaSoruCevap = new YarismaSoruCevap();
                    yarismaSoruCevap.CevapId = cevap.Id;
                    yarismaSoruCevap.Id = Guid.NewGuid();
                    yarismaSoruCevap.YarismaSoruId = yarismaSoru.Id;

                    ent.YarismaSoruCevap.Add(yarismaSoruCevap);

                }

                yarisma.SoruSayisi++;

                ent.SaveChanges();

            }


        }


        private void MasaTemizle()
        {
            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                // kullanilmayan masalar temizlenir
                var masalar = ent.Masa
                    .Where(p => p.MasaKullanici.Count() == 0);
                ent.Masa.RemoveRange(masalar);
                ent.SaveChanges();

                // biten masalar ve yarismalari guncellenir
                var mbiten = ent.Masa
                    .Where(p => p.BaslangicTarihi.HasValue && !p.BitisTarihi.HasValue)
                    .Where(p => p.MasaKullanici.Count() > 1)
                    .Where(p => p.MasaKullanici.Any(t => t.Yarisma.Count() > 0))
                    .ToList();
                foreach (var item in mbiten)
                {
                    DateTime bitisTarihi = item.BaslangicTarihi.Value.AddMinutes(item.SureDk);
                    if (bitisTarihi < DateTime.Now)
                    {
                        item.BitisTarihi = bitisTarihi;
                    }
                    var yarismalar = ent.Yarisma
                        .Where(p => p.MasaKullanici.MasaId == item.Id)
                        .ToList();
                    foreach (var yitem in yarismalar)
                    {
                        yitem.BitisTarihi = bitisTarihi;
                    }
                }
                ent.SaveChanges();
            }
        }

        private void MultiMasaKullaniciTemizle()
        {
            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                // eski kullanicilar silinir
                DateTime kayitKontrol = DateTime.Now.AddMinutes(-1);
                DateTime pingKontrol = DateTime.Now.AddSeconds(-3);

                var silinecekler = ent.MasaKullanici
                    .Where(p => p.Masa.Tip == (int)EnmMasaTipi.MultiPlayerMasa && !p.Masa.BaslangicTarihi.HasValue)
                    .Where(p => (!p.Ping.HasValue && p.KayitTarihi < kayitKontrol)
                        || (p.Ping.HasValue && p.Ping.Value < pingKontrol)
                    );

                ent.MasaKullanici.RemoveRange(silinecekler);
                ent.SaveChanges();
            }
        }

        #endregion

    }
}