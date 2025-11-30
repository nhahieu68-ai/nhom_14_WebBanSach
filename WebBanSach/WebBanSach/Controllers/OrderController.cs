using System;
using System.Linq;
using System.Web.Mvc;
using WebBanSach.Models;
using WebBanSach.Models.ViewModels;
using System.Collections.Generic;

namespace WebBanSach.Controllers
{
    public class OrderController : Controller
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        // GET: Checkout
        public ActionResult Checkout()
        {
            var cart = Session["Cart"] as List<CartItemViewModel>;
            if (cart == null || cart.Count == 0)
                return RedirectToAction("Index", "Cart");

            ViewBag.Total = cart.Sum(x => x.Total);
            ViewBag.Discount = Session["Discount"] ?? 0;

            return View(cart);
        }

        // POST: Checkout
        [HttpPost]
        public ActionResult Checkout(string shippingMethod, string paymentMethod)
        {
            var cart = Session["Cart"] as List<CartItemViewModel>;
            if (cart == null || cart.Count == 0)
                return RedirectToAction("Index", "Cart");

            int discount = (int)(Session["Discount"] ?? 0);
            int shipFee = shippingMethod == "pickup" ? 0 : 25000;

            // Tạo đơn hàng
            Order order = new Order
            {
                UserID = (Session["User"] as User)?.UserID,
                TotalAmount = cart.Sum(x => x.Total) + shipFee - discount,
            };

            db.Orders.Add(order);
            db.SaveChanges(); // orderID sinh ở đây

            // Thêm Payment
            var payment = new Payment
            {
                OrderID = order.OrderID,
                Method = $"{paymentMethod} | {shippingMethod}",
                Amount = order.TotalAmount,
                PaidAt = DateTime.Now
            };

            db.Payments.Add(payment);
            db.SaveChanges();

            // Thêm danh sách OrderItems
            foreach (var item in cart)
            {
                db.OrderItems.Add(new OrderItem
                {
                    OrderID = order.OrderID,
                    BookID = item.BookID,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                });
            }

            db.SaveChanges();

            // Reset session
            Session["Cart"] = null;
            Session["Discount"] = null;

            return RedirectToAction("Success");
        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult History()
        {
            var user = Session["User"] as User;
            if (user == null)
                return RedirectToAction("Login", "Account");

            var orders = db.Orders
                           .Where(o => o.UserID == user.UserID)
                           .OrderByDescending(o => o.OrderID)
                           .ToList();

            return View(orders);
        }

        public ActionResult Details(int id)
        {
            var order = db.Orders.Find(id);
            if (order == null) return HttpNotFound();

            var items = db.OrderItems.Where(i => i.OrderID == id).ToList();
            ViewBag.Items = items;

            return View(order);
        }

        [HttpPost]
        public ActionResult ApplyPromo(string code)
        {
            int discount = 0;

            switch (code.ToUpper())
            {
                case "WHISPER10":
                    discount = 10000;
                    break;

                case "FREESHIP":
                    discount = 25000;
                    break;
            }

            Session["Discount"] = discount;
            return RedirectToAction("Checkout");
        }
    }
}
