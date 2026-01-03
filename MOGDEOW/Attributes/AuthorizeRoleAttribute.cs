using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace MOGDEOW.Attributes
{
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _roles = roles.Select(r => r.ToLower()).ToArray();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("UserRole")?.ToLower();

            if (string.IsNullOrEmpty(role) || !_roles.Contains(role))
            {
                // TempData cảnh báo không có quyền
                var controller = context.Controller as Controller;
                if (controller != null)
                {
                    controller.TempData["Error"] = "Không có quyền truy cập, chuyển về trang chủ!";
                }

                context.Result = new RedirectToActionResult("Index", "Home", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
