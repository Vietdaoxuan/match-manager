using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using matchManager.Models;

namespace matchManager.Controllers
{
    public class MatchesController : Controller
    {
        private MatchManagementEntities db = new MatchManagementEntities();

        // GET: Matches
        public ActionResult MatchesForTeams(int? id)
        {
            var matches = db.Matches.Where(x => x.HomeTeamID == id || x.VisitingTeamID == id);
            return View(matches.ToList());
        }

        public ActionResult Index(int? id)
        {
            var matches = db.Matches.Where(x => x.LeagueId == id);
            return View(matches.ToList());
        }

        // GET: Matches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Match match = db.Matches.Find(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        // GET: Matches/Create
        public ActionResult Create()
        {
            ViewBag.LeagueId = new SelectList(db.Leagues, "Id", "Shortname");
            ViewBag.HomeTeamID = new SelectList(db.Teams, "Id", "FullName");
            ViewBag.SeasonId = new SelectList(db.Seasons, "Id", "Id");
            ViewBag.StadiumId = new SelectList(db.Stadiums, "Id", "FullName");
            ViewBag.VisitingTeamID = new SelectList(db.Teams, "Id", "FullName");
            return View();
        }

        // POST: Matches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HomeTeamID,VisitingTeamID,Round,StadiumId,LeagueId,SeasonId,HomeTeamGoal,VisitTeamGoal,Referee")] Match match)
        {
            if (ModelState.IsValid)
            {
                db.Matches.Add(match);
                db.SaveChanges();
                return RedirectToAction("MatchesForTeams", new { id = match.HomeTeamID,match.VisitingTeamID});
            }

            ViewBag.LeagueId = new SelectList(db.Leagues, "Id", "Shortname", match.LeagueId);
            ViewBag.HomeTeamID = new SelectList(db.Teams, "Id", "FullName", match.HomeTeamID);
            ViewBag.SeasonId = new SelectList(db.Seasons, "Id", "Id", match.SeasonId);
            ViewBag.StadiumId = new SelectList(db.Stadiums, "Id", "FullName", match.StadiumId);
            ViewBag.VisitingTeamID = new SelectList(db.Teams, "Id", "FullName", match.VisitingTeamID);
            return View(match);
        }

        // GET: Matches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Match match = db.Matches.Find(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.LeagueId = new SelectList(db.Leagues, "Id", "Shortname", match.LeagueId);
            ViewBag.HomeTeamID = new SelectList(db.Teams, "Id", "FullName", match.HomeTeamID);
            ViewBag.SeasonId = new SelectList(db.Seasons, "Id", "Id", match.SeasonId);
            ViewBag.StadiumId = new SelectList(db.Stadiums, "Id", "FullName", match.StadiumId);
            ViewBag.VisitingTeamID = new SelectList(db.Teams, "Id", "FullName", match.VisitingTeamID);
            return View(match);
        }

        // POST: Matches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HomeTeamID,VisitingTeamID,Round,StadiumId,LeagueId,SeasonId,HomeTeamGoal,VisitTeamGoal,Referee")] Match match)
        {
            if (ModelState.IsValid)
            {
                db.Entry(match).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MatchesForTeams", new { id = match.VisitingTeamID});
            }
            
            ViewBag.LeagueId = new SelectList(db.Leagues, "Id", "Shortname", match.LeagueId);
            ViewBag.HomeTeamID = new SelectList(db.Teams, "Id", "FullName", match.HomeTeamID);
            ViewBag.SeasonId = new SelectList(db.Seasons, "Id", "Id", match.SeasonId);
            ViewBag.StadiumId = new SelectList(db.Stadiums, "Id", "FullName", match.StadiumId);
            ViewBag.VisitingTeamID = new SelectList(db.Teams, "Id", "FullName", match.VisitingTeamID);
            return View(match);
        }

        // GET: Matches/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Match match = db.Matches.Find(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        // POST: Matches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Match match = db.Matches.Find(id);
            db.Matches.Remove(match);
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
