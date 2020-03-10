using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace matchManager.Controllers
{
    public class AdminsController : Controller
    {
        // GET: Admins
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(string Email, string Password)
        {

            if (Email == "Daoxuanviet99@gmail.com" || Password=="123456")
            {
                Session["Email"] = "Daoxuanviet99@gmail.com";
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }
    }
}