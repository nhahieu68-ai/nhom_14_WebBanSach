using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using WebBanSach.Models;
using WebBanSach.Models.ViewModels;

namespace WebBanSach.Controllers
{
    public class WishlistController : Controller
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        private List<WishlistItemViewModel> GetWishlist()
        {
            var wishlist = Session["Wishlist"] as List<WishlistItemViewModel>;
            if (wishlist == null)
            {
                wishlist = new List<WishlistItemViewModel>();
                Session["Wishlist"] = wishlist;
            }
            return wishlist;
        }

        // ===========================
        // Thêm vào wishlist
        // ===========================
        public ActionResult Add(int id)
        {
            var book = db.Books.Find(id);
            if (book == null) return HttpNotFound();

            var wishlist = GetWishlist();

            if (!wishlist.Any(x => x.BookID == id))
            {
                wishlist.Add(new WishlistItemViewModel
                {
                    BookID = id,
                    Book = book
                });
            }

            Session["WishlistCount"] = wishlist.Count;

            return Redirect(Request.UrlReferrer.ToString());
        }

        // ===========================
        // Xóa item
        // ===========================
        public ActionResult Remove(int id)
        {
            var wishlist = GetWishlist();
            var item = wishlist.FirstOrDefault(x => x.BookID == id);

            if (item != null)
                wishlist.Remove(item);

            Session["WishlistCount"] = wishlist.Count;

            return RedirectToAction("Index");
        }

        // ===========================
        // Hiển thị wishlist
        // ===========================
        public ActionResult Index()
        {
            var wishlist = GetWishlist();
            Session["WishlistCount"] = wishlist.Count;
            return View(wishlist);
        }
    }
}
