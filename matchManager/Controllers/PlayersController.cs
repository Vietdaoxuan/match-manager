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

namespace matchManager.Controllers
{
    public class PlayersController : Controller
    {
        private MatchManagementEntities db = new MatchManagementEntities();

        // GET: Players
        public ActionResult allPlayers()
        {
            return View(db.Players.ToList());
        }

        public ActionResult Index(int? id)
        {
            var players = db.Players.Where(p => p.TeamID == id);
            return View(players.ToList());
        }

        // GET: Players/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // GET: Players/Create
        public ActionResult Create()
        {
            ViewBag.TeamID = new SelectList(db.Teams, "Id", "ShortName");
            return View();
        }

        // POST: Players/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FullName,Age,TeamID,Nationality,Position")] Player player, HttpPostedFileBase Picture)
        {
            if (ModelState.IsValid)
            {
                if (Picture == null)
                {
                    player.Picture = "Not Available";
                }
                else
                {
                    player.Picture = "Players_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + Path.GetExtension(Picture.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Pictures/Players"), player.Picture);
                    Picture.SaveAs(path);
                }
                db.Players.Add(player);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(player);
        }

            // GET: Players/Edit/5
            public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            ViewBag.TeamID = new SelectList(db.Teams, "Id", "ShortName", player.TeamID);
            return View(player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FullName,Age,TeamID,Nationality,Position,Picture")] Player player)
        {
            if (ModelState.IsValid)
            {
                db.Entry(player).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index",new { id = player.TeamID });
            }
            ViewBag.TeamID = new SelectList(db.Teams, "Id", "ShortName", player.TeamID);
            return View(player);
        }

        // GET: Players/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Player player = db.Players.Find(id);
            db.Players.Remove(player);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = player.TeamID });
            
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
