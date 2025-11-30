// File: UserController.cs
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebBanSach.Helpers;
using WebBanSach.Models;

namespace WebBanSach.Controllers
{
    // Yêu cầu bắt buộc: Chỉ Admin mới được truy cập Controller này
    [AdminOnly]
    public class UserController : Controller
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        // GET: User/Index - Danh sách người dùng
        public ActionResult Index()
        {
            // Lấy danh sách người dùng, bao gồm thông tin Role
            var users = db.Users.Include(u => u.Role).OrderByDescending(u => u.UserID).ToList();
            return View(users);
        }

        // GET: User/Edit/5 - Chỉnh sửa thông tin và quyền
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Truyền danh sách Role để Admin chọn
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName", user.RoleID);
            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,FullName,Email,Phone,RoleID")] User user)
        {
            // Tìm user hiện tại trong DB
            var existingUser = db.Users.Find(user.UserID);

            if (existingUser == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                // Chỉ cập nhật các trường Admin được phép sửa (FullName, Email, Phone, RoleID)
                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.Phone = user.Phone;
                existingUser.RoleID = user.RoleID;

                db.Entry(existingUser).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = $"Đã cập nhật thông tin và quyền của tài khoản **{existingUser.Email}** thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName", user.RoleID);
            return View(user);
        }

        // GET: User/Delete/5 - Xác nhận xóa
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Include(u => u.Role).SingleOrDefault(u => u.UserID == id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Tránh Admin tự xóa tài khoản của mình (thêm logic kiểm tra nếu cần)
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Xóa người dùng
            db.Users.Remove(user);
            db.SaveChanges();
            TempData["Success"] = $"Đã xóa tài khoản **{user.Email}** thành công!";
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
        // File: UserController.cs (Thêm vào bên dưới các Action Admin)
        // ...

        // GET: User/Profile - Xem thông tin cá nhân
        [AllowAnonymous] // Tắt tạm [AdminOnly] để dùng cho khách hàng, sau đó kiểm tra Session
        public new ActionResult Profile()
        {
            var user = Session["User"] as User;

            // 🔐 Kiểm tra đăng nhập
            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Hoặc trang đăng nhập của bạn
            }

            // Lấy thông tin người dùng từ DB (để đảm bảo dữ liệu mới nhất)
            var dbUser = db.Users.Find(user.UserID);

            if (dbUser == null)
            {
                Session["User"] = null; // Xóa session nếu không tìm thấy User
                return RedirectToAction("Login", "Account");
            }

            return View(dbUser);
        }

        // POST: User/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult EditProfile([Bind(Include = "UserID,FullName,Email,Phone")] User user)
        {
            var sessionUser = Session["User"] as User;

            // 🔐 Kiểm tra đăng nhập và UserID có khớp không
            if (sessionUser == null || sessionUser.UserID != user.UserID)
            {
                return RedirectToAction("Login", "Account");
            }

            // 🚨 Bỏ qua validation cho PasswordHash, RoleID và CreatedAt
            // Chỉ cập nhật các trường người dùng được phép sửa (FullName, Phone)
            ModelState.Remove("PasswordHash");
            ModelState.Remove("RoleID");
            ModelState.Remove("CreatedAt");

            if (ModelState.IsValid)
            {
                var existingUser = db.Users.Find(user.UserID);
                if (existingUser != null)
                {
                    existingUser.FullName = user.FullName;
                    existingUser.Phone = user.Phone;
                    // Không cho phép sửa Email

                    db.Entry(existingUser).State = EntityState.Modified;
                    db.SaveChanges();

                    // Cập nhật lại Session sau khi lưu thành công
                    Session["User"] = existingUser;

                    TempData["ProfileSuccess"] = "Cập nhật thông tin cá nhân thành công!";
                    return RedirectToAction("Profile");
                }
            }

            TempData["ProfileError"] = "Có lỗi xảy ra khi cập nhật thông tin.";
            return View("Profile", user); // Trả về lại view Profile với dữ liệu đã nhập
        }


        // ...
    }
}