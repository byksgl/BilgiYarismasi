using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BilgiYarismasi.DAL;

namespace BilgiYarismasi.Web.Areas.Yonetim.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KonusController : Controller
    {
        private BilgiYarismasiEntities2 db = new BilgiYarismasiEntities2();

        // GET: Yonetim/Konus
        public ActionResult Index()
        {
            return View(db.Konu.ToList());
        }

        // GET: Yonetim/Konus/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Konu konu = db.Konu.Find(id);
            if (konu == null)
            {
                return HttpNotFound();
            }
            return View(konu);
        }

        // GET: Yonetim/Konus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Yonetim/Konus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Ad,SoruSayisi,SureDk")] Konu konu)
        {
            if (ModelState.IsValid)
            {
                konu.Id = Guid.NewGuid();
                db.Konu.Add(konu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(konu);
        }

        // GET: Yonetim/Konus/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Konu konu = db.Konu.Find(id);
            if (konu == null)
            {
                return HttpNotFound();
            }
            return View(konu);
        }

        // POST: Yonetim/Konus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Ad,SoruSayisi,SureDk")] Konu konu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(konu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(konu);
        }

        // GET: Yonetim/Konus/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Konu konu = db.Konu.Find(id);
            if (konu == null)
            {
                return HttpNotFound();
            }
            return View(konu);
        }

        // POST: Yonetim/Konus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Konu konu = db.Konu.Find(id);
            db.Konu.Remove(konu);
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
