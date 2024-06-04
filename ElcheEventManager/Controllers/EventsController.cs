using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ElcheEventManager.Models.db;

namespace ElcheEventManager.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private EntitiesEM db = new EntitiesEM();

        // GET: Events
        public ActionResult Index()
        {
            var events = db.Events.Include(e => e.Category).Include(e => e.Status);
            return View(events.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            ViewBag.category_id = new SelectList(db.Categories, "id", "name");
            ViewBag.status_id = new SelectList(db.Status, "id", "name");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,description,start_date,end_date,manager_name,manager_phone,manager_email,status_id,category_id")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.category_id = new SelectList(db.Categories, "id", "name", @event.category_id);
            ViewBag.status_id = new SelectList(db.Status, "id", "name", @event.status_id);
            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            ViewBag.category_id = new SelectList(db.Categories, "id", "name", @event.category_id);
            ViewBag.status_id = new SelectList(db.Status, "id", "name", @event.status_id);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,description,start_date,end_date,manager_name,manager_phone,manager_email,status_id,category_id")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.category_id = new SelectList(db.Categories, "id", "name", @event.category_id);
            ViewBag.status_id = new SelectList(db.Status, "id", "name", @event.status_id);
            return View(@event);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            db.Events.Remove(@event);
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

        public JsonResult GetEvents()
        {
            var events = db.Events
                .Select(e => new
                {
                    e.id,
                    e.name,
                    e.start_date,
                    status_name = e.Status.name,
                    category_name = e.Category.name
                })
                .ToList()
                .Select(e => new
                {
                    e.id,
                    e.name,
                    start_date = e.start_date.ToString("MM/dd/yyyy HH:mm"),
                    e.status_name,
                    e.category_name
                })
                .ToList();

            return Json(new { data = events }, JsonRequestBehavior.AllowGet);
        }
    }
}
