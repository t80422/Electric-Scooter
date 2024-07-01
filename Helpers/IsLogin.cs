using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Electric_Scooter.Helpers
{
    public class IsLogin : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Session["Username"] != null;
        }


        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                //Redirecting the user to the Login View of Account Controller  
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                     { "controller", "Account" },
                     { "action", "Login" }
                });
            }
        }
    }
}