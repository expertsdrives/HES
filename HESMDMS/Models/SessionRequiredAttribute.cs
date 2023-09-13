using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HESMDMS.Models
{
    public class SessionRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sessionValue = HttpContext.Current.Session["Admin"];
            if (sessionValue != "Admin")
            {
                // Check if the session is available
                //if (HttpContext.Current.Session == null)
                //{
                    // Session is not available, redirect to an error page or perform some other action
                    filterContext.Result = new RedirectResult("~/Error/SessionError"); // Redirect to an error page
                    return;
                //}
            }

            base.OnActionExecuting(filterContext);
        }
    }
}