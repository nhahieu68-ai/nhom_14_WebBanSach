// File: VoucherController.cs
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebBanSach.Helpers;
using WebBanSach.Models;
using System; // Cần dùng cho DateTime

namespace WebBanSach.Controllers
{
    // Yêu cầu bắt buộc: Chỉ Admin mới được truy cập Controller này
    [AdminOnly]
    public class VoucherController : Controller
    {
        private BookStoreDBEntities db = new BookStoreDBEntities();

        // GET: Voucher/Index - Danh sách mã giảm giá
        public ActionResult Index()
        {
            var coupons = db.Coupons.OrderByDescending(c => c.CouponID).ToList();
            return View(coupons);
        }

        // GET: Voucher/Create - Thêm mới
        public ActionResult Create()
        {
            return View();
        }

        // POST: Voucher/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Code,DiscountPercent,ExpiredAt")] Coupon coupon)
        {
            // Kiểm tra code đã tồn tại chưa
            if (db.Coupons.Any(c => c.Code == coupon.Code))
            {
                ModelState.AddModelError("Code", "Mã giảm giá này đã tồn tại.");
            }

            // Đảm bảo phần trăm giảm giá nằm trong khoảng hợp lý (ví dụ: 1 đến 100)
            if (coupon.DiscountPercent <= 0 || coupon.DiscountPercent > 100)
            {
                ModelState.AddModelError("DiscountPercent", "Phần trăm giảm giá phải từ 1 đến 100.");
            }

            if (ModelState.IsValid)
            {
                db.Coupons.Add(coupon);
                db.SaveChanges();
                TempData["Success"] = $"Đã thêm mã giảm giá **{coupon.Code}** thành công!";
                return RedirectToAction("Index");
            }

            return View(coupon);
        }

        // GET: Voucher/Edit/5 - Chỉnh sửa
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coupon coupon = db.Coupons.Find(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }
            return View(coupon);
        }

        // POST: Voucher/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CouponID,Code,DiscountPercent,ExpiredAt")] Coupon coupon)
        {
            // Kiểm tra code mới có bị trùng với code khác (ngoại trừ chính nó)
            if (db.Coupons.Any(c => c.Code == coupon.Code && c.CouponID != coupon.CouponID))
            {
                ModelState.AddModelError("Code", "Mã giảm giá này đã được sử dụng cho mã khác.");
            }

            if (ModelState.IsValid)
            {
                db.Entry(coupon).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = $"Đã cập nhật mã giảm giá **{coupon.Code}** thành công!";
                return RedirectToAction("Index");
            }
            return View(coupon);
        }

        // GET: Voucher/Delete/5 - Xác nhận xóa
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coupon coupon = db.Coupons.Find(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }
            return View(coupon);
        }

        // POST: Voucher/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Coupon coupon = db.Coupons.Find(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }

            db.Coupons.Remove(coupon);
            db.SaveChanges();
            TempData["Success"] = $"Đã xóa mã giảm giá **{coupon.Code}** thành công!";
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