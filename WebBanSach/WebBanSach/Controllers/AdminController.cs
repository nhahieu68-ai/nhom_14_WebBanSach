using System.Web.Mvc;
using WebBanSach.Helpers;
using WebBanSach.Models;

namespace WebBanSach.Controllers
{
    [AdminOnly]   // 🔐 Chỉ admin được vào
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        private bool IsAdmin()
        {
            var user = Session["User"] as User;
            return (user != null && user.RoleID == 2);
        }
    }
}
