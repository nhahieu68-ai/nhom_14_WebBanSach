
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebBanSach.Helpers;
using WebBanSach.Models;

namespace WebBanSach.Controllers
{
    // Đảm bảo chỉ có Admin (RoleID = 2) mới truy cập được Controller này
    // Giả định bạn đã có một Attribute [AdminOnly] tương tự như trong AdminController.cs
    [AdminOnly]
    public class AdminOrderController : Controller
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        // GET: /AdminOrder/Index
        // Hiển thị danh sách tất cả đơn hàng
        public ActionResult Index()
        {
            // Lấy danh sách đơn hàng, bao gồm thông tin khách hàng (User) để hiển thị tên
            var orders = db.Orders.Include(o => o.User).OrderByDescending(o => o.CreatedAt).ToList();
            return View(orders);
        }

        // GET: /AdminOrder/Details/{id}
        // Xem chi tiết đơn hàng
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy chi tiết đơn hàng, bao gồm User và các OrderItems (có Book)
            Order order = db.Orders
                            .Include(o => o.User)
                            .Include(o => o.OrderItems.Select(oi => oi.Book))
                            .SingleOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            // Lấy thông tin thanh toán (nếu có)
            ViewBag.Payment = db.Payments.FirstOrDefault(p => p.OrderID == id);

            return View(order);
        }

        // POST: /AdminOrder/UpdateStatus - Cập nhật trạng thái
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int id, string status)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            // Danh sách trạng thái hợp lệ
            string[] validStatuses = { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
            if (!validStatuses.Contains(status))
            {
                // Trạng thái không hợp lệ, có thể báo lỗi cho người dùng
                TempData["Error"] = "Trạng thái đơn hàng không hợp lệ.";
                return RedirectToAction("Details", new { id = id });
            }

            order.Status = status;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Success"] = $"Đã cập nhật trạng thái đơn hàng #{id} thành **{status}**.";

            return RedirectToAction("Details", new { id = id });
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