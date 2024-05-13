using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelRoomBookingSystem.Models;

namespace HotelRoomBookingSystem.Controllers
{
    public class StaffLoginController : Controller
    {
        private readonly LEC2023Entities db = new LEC2023Entities();

        // GET: StaffLogin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(StaffsInfo loginModel, string returnUrl)
        {
            // Check if there is a staff with the provided username and password
            bool isValid = db.StaffsInfoes.Any(s => s.Username == loginModel.Username && s.Password == loginModel.Password);

            if (isValid)
            {
                // Authentication successful

                // Store the id in the session
                var staffId = db.StaffsInfoes.Where(s => s.Username == loginModel.Username).Select(s => s.Id).SingleOrDefault();

                Session["UserId"] = staffId;
                
                // Store the username in the session
                Session["Username"] = loginModel.Username;
                
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    // Check if the returnUrl points to the Create action of ClientsInfoes
                    if (returnUrl.Contains("/ClientsInfoes/Create"))
                    {
                        // Redirect to the Create Client page
                        return RedirectToAction("Create", "ClientsInfoes");
                    }
                    else
                    {
                        // Redirect to the returnUrl
                        return Redirect(returnUrl);
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home"); // Redirect to the home page or wherever you want
                }

            }
            else
            {
            // Authentication failed
                
                ViewBag.ErrorMessage = "Invalid username or password. Please try again.";
               
                return View(loginModel);
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}