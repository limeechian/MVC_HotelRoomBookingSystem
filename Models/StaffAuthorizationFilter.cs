using System.Web.Mvc;

namespace HotelRoomBookingSystem.Models
{
    // This class is a custom action filter that checks if the user is logged in by looking for the "UserId" in the session. 
    // If the user is not logged in, it redirects them to the staff login page.
    // This filter is applied to controllers or actions to restrict access to logged-in users.
    public class StaffAuthorizationFilter : ActionFilterAttribute
    { /* `StaffAuthorizationFilter` class is defined that derives from `ActionFilterAttribute`
         Means that this class is a custom action filter that can apply to controller actions */

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        { /* `OnActionExecuting` method is declared, this method is called before an action method in a controller is executed.
             This method is part of the `ActionFilterAttribute` base class and is overriden here to implement custom logic */
            
            var loggedIn = filterContext.HttpContext.Session["UserId"] != null;
            /* `UserId` stored in the user's session is retrieved. The `Session` object is a server-side storage mechanism that can hold data specific to a user's session.
               The `["UserId"]` syntax accesses the value stored with the key "UserId" in the session. If the value is not null, it indicates that the user is logged in. */

            if (!loggedIn)
            { /* Checks whether the user is not logged in. It negates the `loggedIn` variable. If `loggedIn` is `false`, means the user is not logged in. */
                
                filterContext.Result = new RedirectResult("~/StaffLogin/Login");
                /* If the user is not logged in, the `Result` property of the `filterContext` is set to a new instance of `RedirectResult`.
                   Means the action execution is interrupted, and the user is redirected to the login page. */
            }

            base.OnActionExecuting(filterContext);
            /* Calls the base class implementation of `OnActionExecuting` to ensure that any additional behavior defined in the base class is executed.
               In this case, it helps maintain the normal flow of action execution after the custom logic. */
        }
    }
}



using System.Web.Mvc;

namespace HotelRoomBookingSystem.Controllers
{
    public class StaffAuthorizationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        { 
            var loggedIn = filterContext.HttpContext.Session["UserId"] != null;
            
            if (!loggedIn)
            {
                filterContext.Result = new RedirectResult("~/StaffLogin/Login");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}