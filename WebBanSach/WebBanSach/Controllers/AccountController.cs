using System;
using System.Linq;
using System.Web.Mvc;
using WebBanSach.Models;

namespace WebBanSach.Controllers
{
    public class AccountController : Controller
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        // ========== REGISTER ==========
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string fullname, string email, string password, string phone)
        {
            var check = db.Users.FirstOrDefault(x => x.Email == email);
            if (check != null)
            {
                ViewBag.Error = "Email đã tồn tại!";
                return View();
            }

            User u = new User
            {
                FullName = fullname,
                Email = email,
                PasswordHash = password,   // ✔ LƯU PASSWORD ĐÚNG CHỖ
                Phone = phone,
                RoleID = 1                 // ✔ Customer = 1
            };

            db.Users.Add(u);
            db.SaveChanges();

            return RedirectToAction("Login");
        }

        // ========== LOGIN ==========
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var user = db.Users.FirstOrDefault(
                u => u.Email == email && u.PasswordHash == password // ✔ KIỂM TRA ĐÚNG
            );

            if (user == null)
            {
                ViewBag.Error = "Sai email hoặc mật khẩu!";
                return View();
            }

            Session["User"] = user;
            Session["RoleID"] = user.RoleID;

            return RedirectToAction("Index", "Home");
        }

        // ========== LOGOUT ==========
        public ActionResult Logout()
        {
            Session["User"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}
