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
    [Authorize(Roles ="Admin")]
    public class CevapsController : Controller
    {
        private BilgiYarismasiEntities2 db = new BilgiYarismasiEntities2();

        // GET: Yonetim/Cevaps
        public ActionResult Index()
        {
            return View(db.Cevap.ToList());
        }

        // GET: Yonetim/Cevaps/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cevap cevap = db.Cevap.Find(id);
            if (cevap == null)
            {
                return HttpNotFound();
            }
            return View(cevap);
        }

        // GET: Yonetim/Cevaps/Create
        public ActionResult Create(string id)
        {
            if (string.IsNullOrEmpty(id)) return View();

            var cevap = new Cevap();
            cevap.SoruId = new Guid(id);

            return View(cevap);
        }

        // POST: Yonetim/Cevaps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SoruId,Dogrumu,Cevabi")] Cevap cevap)
        {
            if (ModelState.IsValid)
            {
                cevap.Id = Guid.NewGuid();
                db.Cevap.Add(cevap);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cevap);
        }

        // GET: Yonetim/Cevaps/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cevap cevap = db.Cevap.Find(id);
            if (cevap == null)
            {
                return HttpNotFound();
            }
            return View(cevap);
        }

        // POST: Yonetim/Cevaps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SoruId,Dogrumu,Cevabi")] Cevap cevap)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cevap).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cevap);
        }

        // GET: Yonetim/Cevaps/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cevap cevap = db.Cevap.Find(id);
            if (cevap == null)
            {
                return HttpNotFound();
            }
            return View(cevap);
        }

        // POST: Yonetim/Cevaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Cevap cevap = db.Cevap.Find(id);
            db.Cevap.Remove(cevap);
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
