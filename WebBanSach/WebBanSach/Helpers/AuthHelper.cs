using System.Web;
using System.Web.Mvc;
using WebBanSach.Models;

namespace WebBanSach.Helpers
{
    public class AuthHelper
    {

            public static bool IsLoggedIn()
            {
                return HttpContext.Current.Session["User"] != null;
            }

            public static bool IsAdmin()
            {
                var user = HttpContext.Current.Session["User"] as User;
                return user != null && user.RoleID == 2; // 2 = ADMIN
            }
        }

        public class AdminOnly : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            return AuthHelper.IsAdmin();
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpStatusCodeResult(403, "Bạn không có quyền truy cập.");
        }
    }
}
