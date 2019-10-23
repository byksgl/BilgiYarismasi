using BilgiYarismasi.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BilgiYarismasi.DAL;
using System.Runtime.Caching;

namespace BilgiYarismasi.Web.Controllers
{
    [Authorize]
    public class QuizzController : Controller
    {
        // GET: Quizz
        public ActionResult Index()
        {
            QuizIndexViewModel model = new QuizIndexViewModel();

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                model.konular = new LinkedList<Konu>();
                var elms = ent.Konu.OrderBy(p => p.Ad).ToList();
                foreach (var item in elms)
                {
                    if (model.konular.Count == 0)
                    {
                        model.konular.AddFirst(item);
                    }
                    else
                    {
                        model.konular.AddAfter(model.konular.Last, item);
                    }
                }
            }

            return View(model);
        }

        public RedirectToRouteResult Basla(Guid id)
        {
            Yarisma yarisma = new Yarisma();
            
            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {

                var user = ent.AspNetUsers
                    .Where(p => p.UserName == User.Identity.Name)
                    .FirstOrDefault();

                var konu = ent.Konu.Where(p => p.Id == id).FirstOrDefault();

                yarisma = new Yarisma();
                yarisma.BaslangicTarihi = DateTime.Now;
                yarisma.Id = Guid.NewGuid();
                //yarisma.KonuId = id;
                //yarisma.KullaniciId = new Guid(user.Id);
                yarisma.SoruSayisi = konu.SoruSayisi;
                yarisma.SureDk = konu.SureDk;

                ent.Yarisma.Add(yarisma);

                var sorular = ent.Soru
                    .Where(p => p.KonuId == konu.Id)
                    .OrderBy(p => Guid.NewGuid())
                    .Take(yarisma.SoruSayisi)
                    .ToList();

                YarismaS qzs = YarismaS.Instance;
                int soruSiraNo = 0;
                foreach (var soru in sorular)
                {
                    soruSiraNo++;

                    var mDogruCevap = ent.Cevap
                        .Where(p => p.SoruId == soru.Id && p.Dogrumu)
                        .FirstOrDefault();

                    var cevaplar = ent.Cevap
                        .Where(p => p.SoruId == soru.Id && !p.Dogrumu)
                        .OrderBy(p => Guid.NewGuid())
                        .Take(soru.SecenekSayisi.HasValue ? soru.SecenekSayisi.Value-1 : 3)
                        .ToList();

                    cevaplar.Add(mDogruCevap);
                    cevaplar = cevaplar.OrderBy(p => Guid.NewGuid()).ToList();

                    YarismaSoru qs = new YarismaSoru();
                    qs.Id = Guid.NewGuid();
                    qs.YarismaId = yarisma.Id;
                    qs.SoruId = soru.Id;
                    qs.SiraNo = soruSiraNo;

                    ent.YarismaSoru.Add(qs);

                    foreach (var cevap in cevaplar)
                    {
                        YarismaSoruCevap qsc = new YarismaSoruCevap();
                        qsc.CevapId = cevap.Id;
                        qsc.Id = Guid.NewGuid();
                        qsc.YarismaSoruId = qs.Id;

                        ent.YarismaSoruCevap.Add(qsc);

                    }

                }

                ent.SaveChanges();

                Session[SessionNames.QUIZ] = yarisma;

            }

            return RedirectToAction("Soru");
        }

        public ActionResult Soru(int? id)
        {
            SoruViewModel svm = new SoruViewModel();
            svm.SiraNo = id.HasValue ? id.Value : 1;

            Yarisma quiz = Session[SessionNames.QUIZ] as Yarisma;
            Konu konu = new Konu();

            if (quiz.BaslangicTarihi.AddMinutes(quiz.SureDk.Value) < DateTime.Now)
            {
                return RedirectToAction("Bitir");
            }

            using (BilgiYarismasiEntities2 ent=new BilgiYarismasiEntities2())
            {
                var qs = ent.YarismaSoru
                    .Where(p => p.YarismaId == quiz.Id && p.SiraNo == svm.SiraNo)
                    .FirstOrDefault();
                var soru = ent.Soru
                    .Where(p => p.Id == qs.SoruId)
                    .FirstOrDefault();

                svm.Sorusu = soru.Sorusu;

                var cevaplar = ent.YarismaSoruCevap
                    .Where(p => p.YarismaSoruId == qs.Id)
                    .ToList();

                foreach (var cevap in cevaplar)
                {
                    var cvp = ent.Cevap.Where(p => p.Id == cevap.CevapId).FirstOrDefault();
                    CevapKisaModel c1 = new CevapKisaModel();
                    c1.Id = cvp.Id;
                    c1.Cevabi = cvp.Cevabi;
                    svm.cevaplar.Add(c1);
                }

                konu = ent.Konu
                    .Where(p => p.Id == quiz.MasaKullaniciId)
                    .FirstOrDefault();

            }

            ViewBag.seconds = (int)((quiz.BaslangicTarihi.AddMinutes(konu.SureDk) - DateTime.Now).TotalSeconds);

            return View(svm);
        }

        [HttpPost]
        public ActionResult Soru(SoruViewModel model)
        {

            Yarisma quiz = Session[SessionNames.QUIZ] as Yarisma;

            if (quiz.BaslangicTarihi.AddMinutes(quiz.SureDk.Value) < DateTime.Now)
            {
                return RedirectToAction("Bitir");
            }

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                var qs = ent.YarismaSoru
                    .Where(p => p.YarismaId == quiz.Id && p.SiraNo == model.SiraNo)
                    .FirstOrDefault();

                qs.CevapId = model.cevapId;

                ent.SaveChanges();

            }

            if (model.SiraNo >= quiz.SoruSayisi)
            {
                return RedirectToAction("Bitir");
            }

            return RedirectToAction("Soru", new { id = model.SiraNo + 1 });
        }

        public ActionResult Bitir()
        {
            IBitis bvm = new BitirViewModel();

            Yarisma quiz = Session[SessionNames.QUIZ] as Yarisma;

            bvm.toplam = quiz.SoruSayisi;

            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                var q1 = ent.Yarisma.Where(p => p.Id == quiz.Id).FirstOrDefault();
                q1.BitisTarihi = DateTime.Now;

                ent.SaveChanges();

                Session[SessionNames.QUIZ] = null;

                var quizSorulari = ent.YarismaSoru
                    .Where(p => p.YarismaId == quiz.Id)
                    .ToList();

                bvm.cevaplanan = 0;
                bvm.dogruSayisi = 0;
                bvm.yanlisSayisi = 0;

                foreach (var quizSorusu in quizSorulari)
                {
                    if (quizSorusu.CevapId!=null)
                    {
                        bvm.cevaplanan++;

                        var cevap = ent.Cevap.Where(p => p.Id == quizSorusu.CevapId).FirstOrDefault();

                        if (cevap.Dogrumu)
                        {
                            bvm.dogruSayisi++;
                        }
                        else
                        {
                            bvm.yanlisSayisi++;
                        }

                    }
                }

            }

            return View(bvm);
        }
        
    }
}