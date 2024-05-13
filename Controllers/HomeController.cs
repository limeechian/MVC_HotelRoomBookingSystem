using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelRoomBookingSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NewClientRegistration()
        {
            //if (User.Identity.IsAuthenticated)
            if (Session["UserId"] != null) 
            {
                // User is logged in, so display the client registration form
                return RedirectToAction("Create", "ClientsInfoes");
            }
            else
            {
                // User is not logged in, so redirect to the Login page
                return RedirectToAction("Login", "StaffLogin", new { returnUrl = "/ClientsInfoes/Create" });
            }
        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}