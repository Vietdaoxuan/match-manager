using matchManager.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace matchManager.Controllers
{
    public class TeamsController : Controller
    {
        private MatchManagementEntities db = new MatchManagementEntities();

        // GET: Teams
        public ActionResult allTeams()
        {
            return View(db.Teams.ToList());
        }

        public ActionResult Index(int? id)
        {
            var teams = db.Teams.Where(t => t.LeagueId == id);
            return View(teams.ToList());
        }

        // GET: Teams/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // GET: Teams/Create
        public ActionResult Create(int? id)
        {
            ViewBag.LeagueId = new SelectList(db.Leagues, "Id", "Shortname");
            return View();       
        }

        // POST: Teams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FullName,LeagueId,Description")] Team team, HttpPostedFileBase Picture)
        {
            if (ModelState.IsValid)
            {
                if (Picture == null)
                {
                    team.Picture = "Not Available";
                }
                else
                {
                    team.Picture = "Leagues_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + Path.GetExtension(Picture.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Pictures"), team.Picture);
                    Picture.SaveAs(path);
                }
                db.Teams.Add(team);
                db.SaveChanges();
                return RedirectToAction("Index",new { id=team.LeagueId});
            }
            ViewBag.LeagueId = new SelectList(db.Leagues, "Id", "Shortname", team.LeagueId);
            return View(team);
        }

        // GET: Teams/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            ViewBag.LeagueId = new SelectList(db.Leagues, "Id", "Shortname", team.LeagueId);
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ShortName,FullName,LeagueId,Description")] Team team, HttpPostedFileBase Picture)
        {
            
            if (ModelState.IsValid)
            {
                if (Picture == null)
                {
                    team.Picture = "Not Available";
                }
                else
                {
                    team.Picture = "Leagues_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + Path.GetExtension(Picture.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Pictures"), team.Picture);
                    Picture.SaveAs(path);                   
                }
                db.Teams.Attach(team);
                db.Entry(team).State = EntityState.Modified;               
                db.SaveChanges();
                return RedirectToAction("Index",new { id = team.LeagueId });

            }
            ViewBag.LeagueId = new SelectList(db.Leagues, "Id", "Shortname", team.LeagueId);
            return View(team);
        }

        // GET: Teams/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            Team team = db.Teams.Find(id);
            db.Teams.Remove(team);
            db.SaveChanges();
            return RedirectToAction("Index",new { id =team.LeagueId});
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
