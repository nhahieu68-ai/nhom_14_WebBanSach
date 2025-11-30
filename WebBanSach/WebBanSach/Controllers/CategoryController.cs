// File: CategoryController.cs
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
    public class CategoryController : Controller
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        // GET: Category/Index - Danh sách danh mục
        public ActionResult Index()
        {
            var categories = db.Categories.ToList();
            return View(categories);
        }

        // GET: Category/Create - Thêm mới
        public ActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                TempData["Success"] = $"Đã thêm danh mục **{category.CategoryName}** thành công!";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Category/Edit/5 - Chỉnh sửa
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = $"Đã cập nhật danh mục **{category.CategoryName}** thành công!";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Category/Delete/5 - Xác nhận xóa
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);

            // Xóa danh mục
            db.Categories.Remove(category);
            db.SaveChanges();
            TempData["Success"] = $"Đã xóa danh mục **{category.CategoryName}** thành công!";
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