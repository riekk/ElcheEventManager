using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using ElcheEventManager.Models.db;
using ElcheEventManager.Models.util;

namespace ElcheEventManager.Controllers
{
    public class IntersectionsController : Controller
    {
        private EntitiesEM db = new EntitiesEM();

        // GET: Intersections
        public ActionResult Index()
        {
            var intersections = db.Intersections.Include(i => i.Event).Include(i => i.Material);
            return View(intersections.ToList());
        }

        // GET: Intersections/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Intersection intersection = db.Intersections.Find(id);
            if (intersection == null)
            {
                return HttpNotFound();
            }
            return View(intersection);
        }

        // GET: Intersections/Create
        public ActionResult Create()
        {
            //ViewBag.event_id = new SelectList(db.Events, "id", "name");
            ViewBag.material_id = new SelectList(db.Materials, "id", "name");
            return PartialView("_Create");
        }

        // POST: Intersections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,primary_street,secondary_street,quantity,latitude,longitude,material_id,event_id")] Intersection intersection)
        {
            try
            {
                if (ModelState.IsValid)
            {
                db.Intersections.Add(intersection);
                db.SaveChanges();
                return RedirectToAction("IntersectionsMap", new { id = intersection.event_id });
            }
                return PartialView("_Create", intersection);
            }
            catch (Exception ex)
            {
                // If an error occurs during creation, return an error response
                return Json(new { success = false, error = ex.Message });
            }
        }

        // GET: Intersections/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Intersection intersection = db.Intersections.Find(id);
            if (intersection == null)
            {
                return HttpNotFound();
            }
            ViewBag.event_id = new SelectList(db.Events, "id", "name", intersection.event_id);
            ViewBag.material_id = new SelectList(db.Materials, "id", "name", intersection.material_id);
            ViewBag.streets = new List<SelectListItem>
            {
                new SelectListItem { Value = intersection.primary_street, Text = intersection.primary_street },
                new SelectListItem { Value = intersection.secondary_street, Text = intersection.secondary_street }
            };
            return PartialView("_Edit", intersection);
        }

        // POST: Intersections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,primary_street,secondary_street,quantity,latitude,longitude,material_id,event_id")] Intersection intersection)
        {
            if (intersection.quantity == 0)
            {
                // Asigna un valor predeterminado
                intersection.quantity = 1;
            }
            if (ModelState.IsValid)
            {
                
                db.Entry(intersection).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IntersectionsMap", new { id = intersection.event_id });
            }
            ViewBag.event_id = new SelectList(db.Events, "id", "name", intersection.event_id);
            ViewBag.material_id = new SelectList(db.Materials, "id", "name", intersection.material_id);
            return PartialView("_Edit", intersection);
        }

        // GET: Intersections/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Intersection intersection = db.Intersections.Find(id);
            if (intersection == null)
            {
                return HttpNotFound();
            }
            return View(intersection);
        }

        // POST: Intersections/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Intersection intersection = db.Intersections.Find(id);
            db.Intersections.Remove(intersection);
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

        public ActionResult IntersectionsMap(int? id)
        {
            ViewBag.IsMapView = true;
            //var intersecciones = db.Intersections.Where(i => i.event_id == id);
            //return View(intersecciones.ToList());

            ViewBag.event_id = id;

            //ViewBag.event_id = new SelectList(db.Events, "id", "name");
            ViewBag.material_id = new SelectList(db.Materials, "id", "name");
            return View();
        }


        public ActionResult GetIntersections(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var intersections = db.Intersections.Where(i => i.event_id == id).ToList();
            if (intersections.Count() > 0)
            {
                return Json(new { success = true, intersections }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, message = "No hay datos disponibles." }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult FencesList(int id)
        {
            var intersections = db.Intersections
                .Where(i => i.event_id == id)
                .OrderBy(i => i.id)
                .ToList();

            //.Select(i => new IntersectionViewModel
            // {
            //     Id = i.id,
            //     PrimaryStreet = i.primary_street,
            //     SecondaryStreet = i.secondary_street,
            //     Quantity = i.quantity
            // })
            ViewBag.eventId = id;
            ViewBag.Title = "Listado de " + db.Events.Where(e => e.id == id).Select(e => e.name).FirstOrDefault();
            return View(intersections);
        }      

        [HttpPost]
        public ActionResult UpdateOrder(int eventId, List<RowData> ids)
        {
            
            var intersections = db.Intersections.Where(i => i.event_id == eventId).ToList();

            foreach (var item in ids)
            {                
                var intersection = intersections.FirstOrDefault(i => i.id == item.Id);
                if (intersection != null)
                {
                    intersection.position = item.NewOrder;
                }
            }

            db.SaveChanges();
            return Json("Ok");
        }

    }
}

