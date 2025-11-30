using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models;
using WebBanSach.Models.ViewModels;

namespace WebBanSach.Controllers
{
    public class CartController : Controller
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        // Lấy giỏ hàng từ session
        private List<CartItemViewModel> GetCart()
        {
            var cart = Session["Cart"] as List<CartItemViewModel>;
            if (cart == null)
            {
                cart = new List<CartItemViewModel>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        // ================================
        // THÊM SÁCH VÀO GIỎ (có cập nhật badge)
        // ================================
        public ActionResult AddToCart(int id, int qty = 1)
        {
            var book = db.Books.Find(id);
            if (book == null) return HttpNotFound();

            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.BookID == id);
            if (item != null)
            {
                item.Quantity += qty;
            }
            else
            {
                cart.Add(new CartItemViewModel
                {
                    BookID = id,
                    Book = book,
                    Quantity = qty,
                    Price = book.Price
                });
            }

            // CẬP NHẬT SỐ LƯỢNG GIỎ HÀNG
            Session["CartCount"] = cart.Sum(x => x.Quantity);

            Session["Cart"] = cart;

            return RedirectToAction("Index");
        }

        // Hiển thị giỏ hàng
        public ActionResult Index()
        {
            var cart = GetCart();
            ViewBag.Total = cart.Sum(x => x.Total);

            // LUÔN đồng bộ CartCount
            Session["CartCount"] = cart.Sum(x => x.Quantity);

            return View(cart);
        }

        // ================================
        // XÓA ITEM
        // ================================
        public ActionResult Remove(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.BookID == id);

            if (item != null)
                cart.Remove(item);

            // CẬP NHẬT SỐ LƯỢNG GIỎ HÀNG
            Session["CartCount"] = cart.Sum(x => x.Quantity);

            return RedirectToAction("Index");
        }

        // ================================
        // CẬP NHẬT SỐ LƯỢNG
        // ================================
        [HttpPost]
        public ActionResult UpdateQuantity(int id, int qty)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.BookID == id);

            if (item != null && qty > 0)
                item.Quantity = qty;

            // CẬP NHẬT SỐ LƯỢNG GIỎ HÀNG
            Session["CartCount"] = cart.Sum(x => x.Quantity);

            return RedirectToAction("Index");
        }
    }
}
