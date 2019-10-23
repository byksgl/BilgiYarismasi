using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BilgiYarismasi.DAL;
using BilgiYarismasi.Web.Areas.Yonetim.Models;
using AutoMapper;

namespace BilgiYarismasi.Web.Areas.Yonetim.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SorusController : Controller
    {
        private BilgiYarismasiEntities2 db = new BilgiYarismasiEntities2();

        public SorusController()
        {
            Mapper.CreateMap<Soru, SoruViewModel>();
            Mapper.CreateMap<SoruViewModel, Soru>();
        }

        // GET: Yonetim/Sorus
        public ActionResult Index()
        {
            return View(db.Soru.ToList());
        }

        // GET: Yonetim/Sorus/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Soru soru = db.Soru.Find(id);
            if (soru == null)
            {
                return HttpNotFound();
            }
            return View(soru);
        }

        // GET: Yonetim/Sorus/Create
        public ActionResult Create()
        {
            SoruViewModel model = new SoruViewModel();

            var lst = db.Konu.OrderBy(p => p.Ad).ToList();

            model.Konular = new SelectList(lst, "Id", "Ad");

            return View(model);
        }

        // POST: Yonetim/Sorus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,KonuId,Sorusu")] SoruViewModel pSoru)
        {
            Soru soru = Mapper.Map<Soru>(pSoru);
            //soru.KonuId = new Guid(pSoru.Konular.SelectedValue.ToString());

            if (ModelState.IsValid)
            {
                soru.Id = Guid.NewGuid();
                db.Soru.Add(soru);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(soru);
        }

        // GET: Yonetim/Sorus/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Soru soru = db.Soru.Find(id);
            if (soru == null)
            {
                return HttpNotFound();
            }
            return View(soru);
        }

        // POST: Yonetim/Sorus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,KonuId,Sorusu")] Soru soru)
        {
            if (ModelState.IsValid)
            {
                db.Entry(soru).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(soru);
        }

        // GET: Yonetim/Sorus/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Soru soru = db.Soru.Find(id);
            if (soru == null)
            {
                return HttpNotFound();
            }
            return View(soru);
        }

        // POST: Yonetim/Sorus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Soru soru = db.Soru.Find(id);
            db.Soru.Remove(soru);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
