using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using WebBanSach.Models;
using WebBanSach.Models.ViewModels;


namespace WebBanSach.Controllers
{
    public class HomeController : Controller
    {
        private readonly BookStoreDBEntities db = new BookStoreDBEntities();

        // GET: /
        public ActionResult Index()
        {
            // Lấy danh mục
            var categories = db.Categories.ToList();

            // Sách mới (100 cuốn)
            var newBooks = db.Books
                .OrderByDescending(b => b.BookID)
                .Take(100)
                .ToList();

            // Sách nổi bật 
            var popular = db.Books
                .OrderByDescending(b => b.BookID)
                .Take(100)
                .ToList();

            // Sách theo từng thể loại
            var booksByCat = new Dictionary<Category, List<Book>>();

            foreach (var cat in categories)
            {
                var list = db.Books
                    .Where(b => b.Categories.Any(c => c.CategoryID == cat.CategoryID))
                    .Take(100)
                    .ToList();

                if (list.Any())
                    booksByCat.Add(cat, list);
            }

            var model = new HomeViewModel
            {
                NewBooks = newBooks,
                PopularBooks = popular,
                Categories = categories,
                BooksByCategory = booksByCat
            };

            return View(model);
        }
        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(string name, string email, string message)
        {
            // Sau này nếu muốn lưu DB thì xử lý tại đây.
            TempData["Success"] = "Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi sớm nhất.";

            return RedirectToAction("Contact");
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