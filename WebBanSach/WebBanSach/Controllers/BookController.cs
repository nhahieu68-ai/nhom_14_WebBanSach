using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using WebBanSach.Models;
using WebBanSach.Models.ViewModels;

namespace WebBanSach.Controllers
{
    public class BookController : Controller
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        // ================================
        // KIỂM TRA ADMIN
        // ================================
        private bool IsAdmin()
        {
            var user = Session["User"] as WebBanSach.Models.User;
            return (user != null && user.RoleID == 2); // 2 = ADMIN
        }

        private ActionResult NoPermission()
        {
            return new HttpStatusCodeResult(403, "Bạn không có quyền truy cập.");
        }

        // ================================
        // GET: Book (có phân trang)
        // ================================
        public ActionResult Index(int page = 1)
        {
            int pageSize = 10;

            var books = db.Books
                .OrderBy(b => b.BookID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include("Authors")
                .Include("BookImages")
                .ToList();

            int totalBooks = db.Books.Count();

            ViewBag.TotalPages = Math.Ceiling((double)totalBooks / pageSize);
            ViewBag.CurrentPage = page;

            return View(books);
        }

        // ================================
        // CREATE (GET)
        // ================================
        public ActionResult Create()
        {
            if (!IsAdmin()) return NoPermission();

            ViewBag.CategoryList = db.Categories.ToList();
            return View();
        }

        // ================================
        // CREATE (POST)
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Book book, string AuthorName, string ImageUrl, int[] SelectedCategories)
        {
            if (!IsAdmin()) return NoPermission();

            if (ModelState.IsValid)
            {
                // Tác giả
                var author = db.Authors.FirstOrDefault(a => a.Name == AuthorName);
                if (author == null)
                {
                    author = new Author { Name = AuthorName };
                    db.Authors.Add(author);
                    db.SaveChanges();
                }
                book.Authors.Add(author);

                // Lưu sách
                db.Books.Add(book);
                db.SaveChanges();

                // Gắn category
                if (SelectedCategories != null)
                {
                    foreach (var catId in SelectedCategories)
                    {
                        var category = db.Categories.Find(catId);
                        if (category != null)
                            book.Categories.Add(category);
                    }
                }

                // Thêm ảnh
                if (!string.IsNullOrEmpty(ImageUrl))
                {
                    var img = new BookImage
                    {
                        BookID = book.BookID,
                        ImageUrl = ImageUrl
                    };
                    db.BookImages.Add(img);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryList = db.Categories.ToList();
            return View(book);
        }

        // ================================
        // EDIT (GET)
        // ================================
        public ActionResult Edit(int id)
        {
            if (!IsAdmin()) return NoPermission();

            var book = db.Books.Include("Categories").FirstOrDefault(b => b.BookID == id);

            if (book == null)
                return HttpNotFound();

            ViewBag.CategoryList = db.Categories.ToList();
            return View(book);
        }

        // ================================
        // EDIT (POST)
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Book book, string AuthorName, string ImageUrl, int[] SelectedCategories)
        {
            if (!IsAdmin()) return NoPermission();

            var dbBook = db.Books
                .Include("Categories")
                .Include("Authors")
                .FirstOrDefault(b => b.BookID == book.BookID);

            if (dbBook == null)
                return HttpNotFound();

            // Update thông tin chính
            dbBook.Title = book.Title;
            dbBook.Description = book.Description;
            dbBook.Price = book.Price;

            // Author
            dbBook.Authors.Clear();
            var author = db.Authors.FirstOrDefault(a => a.Name == AuthorName);

            if (author == null)
            {
                author = new Author { Name = AuthorName };
                db.Authors.Add(author);
                db.SaveChanges();
            }
            dbBook.Authors.Add(author);

            // Category
            dbBook.Categories.Clear();
            if (SelectedCategories != null)
            {
                foreach (var cid in SelectedCategories)
                {
                    var cat = db.Categories.Find(cid);
                    if (cat != null)
                        dbBook.Categories.Add(cat);
                }
            }

            // Image
            var img = db.BookImages.FirstOrDefault(i => i.BookID == dbBook.BookID);
            if (img == null)
            {
                img = new BookImage { BookID = dbBook.BookID };
                db.BookImages.Add(img);
            }
            img.ImageUrl = ImageUrl;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // ================================
        // DELETE (VIEW)
        // ================================
        public ActionResult Delete(int id)
        {
            if (!IsAdmin()) return NoPermission();

            var book = db.Books.Find(id);
            if (book == null) return HttpNotFound();

            return View(book);
        }

        // ================================
        // DELETE (POST)
        // ================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return NoPermission();

            var book = db.Books
                .Include(b => b.BookImages)
                .Include(b => b.Authors)
                .Include(b => b.Categories)
                .FirstOrDefault(b => b.BookID == id);

            if (book == null)
                return HttpNotFound();

            foreach (var img in book.BookImages.ToList())
                db.BookImages.Remove(img);

            book.Authors.Clear();
            book.Categories.Clear();

            var cartItems = db.CartItems.Where(c => c.BookID == id).ToList();
            foreach (var c in cartItems)
                db.CartItems.Remove(c);

            var orderItems = db.OrderItems.Where(o => o.BookID == id).ToList();
            foreach (var o in orderItems)
                db.OrderItems.Remove(o);

            db.SaveChanges();

            db.Books.Remove(book);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // ================================
        // BOOK DETAILS
        // ================================
        public ActionResult Details(int id)
        {
            var book = db.Books
                .Include("Authors")
                .Include("Categories")
                .Include("BookImages")
                .FirstOrDefault(b => b.BookID == id);

            if (book == null)
                return HttpNotFound();

            var reviews = db.Reviews
                .Where(r => r.BookID == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            List<Book> related = new List<Book>();

            if (book.Categories.Any())
            {
                var catIds = book.Categories.Select(c => c.CategoryID).ToList();

                related = db.Books
                    .Where(b => b.BookID != id &&
                                b.Categories.Any(c => catIds.Contains(c.CategoryID)))
                    .Take(6)
                    .ToList();
            }

            var model = new BookDetailViewModel
            {
                Book = book,
                Reviews = reviews,
                RelatedBooks = related
            };

            return View(model);
        }
        public ActionResult Search(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return RedirectToAction("Index");

            var books = db.Books
                .Where(b =>
                    b.Title.Contains(keyword) ||
                    b.Authors.Any(a => a.Name.Contains(keyword)) ||
                    b.Categories.Any(c => c.CategoryName.Contains(keyword))
                )
                .ToList();

            return View("SearchResults", books);
        }
        public ActionResult Category(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null) return HttpNotFound();

            var books = db.Books
                .Where(b => b.Categories.Any(c => c.CategoryID == id))
                .ToList();

            ViewBag.CategoryName = category.CategoryName;

            return View("Category", books);
        }




    }
}
