using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using matchManager.Models;
using matchManager.Models.ViewModels;
using matchManager.Utilities;

namespace matchManager.Controllers
{
    public class LoginController : Controller
    {
        private MatchManagementEntities db = new MatchManagementEntities();

        // GET: Login
        public ActionResult Index()
        {
            int a;
            return View();
        }

        [HttpPost]
        public JsonResult Login(Account account)
        {
            account.Password = Common.MD5Encrypt(account.Password);
            Admin admin = db.Admins.Where(x => x.User_Email == account.Email && x.User_Password == account.Password).FirstOrDefault();
            if (admin != null)
            {
                Session["Email"] = admin.User_Email;
                Session["UserName"] = admin.UserName;
                Session["UserId"] = admin.Id;
                Session["Picture"] = admin.Picture;
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }
                            
        public ActionResult LogOut()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index");
        }

        // GET: Login/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // GET: Login/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Login/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserName,User_Email,User_Password")] Admin admin, HttpPostedFileBase Picture)
        {
            if (ModelState.IsValid)
            {
                if (Picture == null)
                {
                    admin.Picture = "male-shadow-circle-128.png";
                }
                else
                {
                    admin.Picture = "Admins_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + Path.GetExtension(Picture.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Pictures/Admins"), admin.Picture);
                    Picture.SaveAs(path);
                }
                admin.User_Password = Common.MD5Encrypt(admin.User_Password);
                db.Admins.Add(admin);
                db.SaveChanges();
                return RedirectToAction("Index","Leagues");
            }

            return View(admin);
        }

        public JsonResult CheckAccount(string userdata)
        {
            System.Threading.Thread.Sleep(200);
            var searchdata = db.Admins.Where(x => x.UserName == userdata).FirstOrDefault();
            if (searchdata != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        public JsonResult CheckEmail(string userdata)
        {
            System.Threading.Thread.Sleep(200);
            var searchdata = db.Admins.Where(x => x.User_Email == userdata).FirstOrDefault();
            if (searchdata != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        [NonAction]
        public void SendEmail(string email, string ResetCode)
        {
            //string content = System.IO.File.ReadAllText(Server.MapPath(@"~/EmailMessage/Text.cshtml"));
            //var verifyUrl = "/Login/ResetPassword/" + ResetCode;
            //var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            //content = content.Replace("{{Content}}", link);
            //Common.SendMail(email, "Reset Password", "Admin", content);

            var verifyUrl = "/Login/ResetPassword/" + ResetCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("daoxuanviet99@gmail.com", "Dao Xuan Viet");
            var toEmail = new MailAddress(email);
            string fromEmailPassword = "viet1999";
            string subject = "Reset Passwords";
            string body = "<p>Bấm vào đây để reset lại password:</p> <a href=" + link + ">Đặt lại mật khẩu</a>";
            var smtp = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        public ActionResult ForgotPassword()
        {
            return View();
        } 

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            var account = db.Admins.Where(x => x.User_Email == email).FirstOrDefault();
            string message = "";
            if (account != null)
            {
                //Send Email for reset password
                string resetCode = Guid.NewGuid().ToString();
                SendEmail(account.User_Email, resetCode);
                account.ResetPasswordCode = resetCode;
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
                message = "Link reset password đã được gửi đến email của bạn";
            }
            else
            {
                message = "Tài khoản không tồn tại";
            }
            ViewBag.message = message;
            return View();
        }

        public ActionResult ResetPassword(string id)
        {
            var user = db.Admins.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
            if(user != null)
            {
                ResetPassword model = new ResetPassword();
                model.ResetCode = id;
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPassword model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                var user = db.Admins.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                if (user != null)
                {
                    Admin admin = db.Admins.Where(x => x.User_Password == model.NewPassword).FirstOrDefault();
                    admin.User_Password = Common.MD5Encrypt(admin.User_Password);                   
                    user.ResetPasswordCode = "";
                    db.Configuration.ValidateOnSaveEnabled = false;

                    db.SaveChanges();
                    message = "Đổi mật khẩu thành công";
                }
            }
            else
            {
                message = "Không hợp lệ";
            }
            ViewBag.message = message;
            return RedirectToAction("Index");
        }
        // GET: Login/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // POST: Login/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserName,User_Email,User_Password")] Admin admin, HttpPostedFileBase Picture)
        {
            if (ModelState.IsValid)
            {
                if (Picture == null)
                {
                    admin.Picture = "male-shadow-circle-128.png";
                }
                else
                {
                    admin.Picture = "Admins_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + Path.GetExtension(Picture.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Pictures/Admins"), admin.Picture);
                    Picture.SaveAs(path);
                }                
                db.Admins.Attach(admin);
                db.Entry(admin).State = EntityState.Modified;
                Session["Picture"] = admin.Picture;
                db.SaveChanges();
                return RedirectToAction("Index","Leagues");
            }
            return View(admin);
        }

        // GET: Login/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // POST: Login/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Admin admin = db.Admins.Find(id);
            db.Admins.Remove(admin);
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
