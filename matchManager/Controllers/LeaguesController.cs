using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using matchManager.Models;
using PagedList;

namespace matchManager.Controllers
{
    public class LeaguesController : Controller
    {
        private MatchManagementEntities db = new MatchManagementEntities();

        // GET: Leagues
        public ActionResult Index(int PageNumber = 1, int PageSize = 3)
        {
            return View(db.Leagues.ToList().ToPagedList(PageNumber, PageSize));
        }

        // GET: Leagues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = db.Leagues.Find(id);
            if (league == null)
            {
                return HttpNotFound();
            }
            return View(league);
        }

        // GET: Leagues/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Leagues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Shortname,FullName,Description")] League league,HttpPostedFileBase Picture)
        {
            if (ModelState.IsValid)
            {
                if (Picture == null)
                {
                    league.Picture = "Not Available";
                }
                else
                {
                    league.Picture = "Leagues_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + Path.GetExtension(Picture.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Pictures/Leagues"), league.Picture);
                    Picture.SaveAs(path);
                }
                db.Leagues.Add(league);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(league);
        }

        // GET: Leagues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = db.Leagues.Find(id);
            if (league == null)
            {
                return HttpNotFound();
            }
            return View(league);
        }

        // POST: Leagues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Shortname,FullName,Description")] League league, HttpPostedFileBase Picture)
        {
            if (ModelState.IsValid)
            {
                if (Picture == null)
                {
                    league.Picture = "Not Available";
                }
                else
                {
                    league.Picture = "Leagues_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + Path.GetExtension(Picture.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Pictures/Leagues"), league.Picture);
                    Picture.SaveAs(path);
                }
                db.Leagues.Attach(league);
                db.Entry(league).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(league);
        }

        // GET: Leagues/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = db.Leagues.Find(Int32.Parse(id));
            if (league == null)
            {
                return HttpNotFound();
            }
            return View(league);
        }

        // POST: Leagues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            League league = db.Leagues.Find(Int32.Parse(id));
            db.Leagues.Remove(league);
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
