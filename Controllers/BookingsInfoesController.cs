using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HotelRoomBookingSystem.Models;
using Microsoft.Ajax.Utilities;

namespace HotelRoomBookingSystem.Controllers
{
    [StaffAuthorizationFilter]
    public class BookingsInfoesController : Controller
    {
        private readonly LEC2023Entities db = new LEC2023Entities();

        // GET: BookingsInfoes
        public ActionResult Index(string id)
        {
            
            short status = 1; // The value for the @Status parameter
            SqlParameter param = new SqlParameter("@Status", status);

            var bookings = db.Database.SqlQuery<BookingsInfo>("EXEC USP_BookingsInfo_GetByStatus @Status", param).ToList();

            if (bookings.Count == 0)
                ViewBag.BookingNotFound = "No Booking Found.";  // only when one line of expression, then no need to use square brackets for if statement


            if (!string.IsNullOrEmpty(id))
            {
                if (long.TryParse(id, out long numericId))
                {
                    var bookingInfoById = bookings.Where(b => b.Id == numericId).ToList();
                    if (bookingInfoById.Count == 0)
                        ViewBag.BookingNotFound = "Booking ID not found.";
                    return View(bookingInfoById);
                }
                else
                {
                    var bookingInfoById = bookings.Where(b => b.Id == numericId).ToList();
                    if (bookingInfoById.Count == 0)
                        ViewBag.BookingNotFound = "Error: Please enter a numeric ID.";
                    return View(bookingInfoById);
                }
            }
            return View(bookings);
        }


        // GET: BookingsInfoes/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookingsInfo bookingsInfo = db.BookingsInfoes.Find(id);
            if (bookingsInfo == null)
            {
                return HttpNotFound();
            }
            return View(bookingsInfo);
        }

        // GET: BookingsInfoes/Create
        [HttpGet]
        public ActionResult Create(long id)
        {  /* This is an HTTP GET action method for displaying the create view for a booking. 
              It takes a `long id` parameter, which represents the ClientId for whom the booking is being created. */
            
            ViewBag.ClientId = id; // It sets `ClientId` in Viewbag, which can be used in the view to prepopulate a field. 
            var bookingsInfo = new BookingsInfo { ClientId = id };  // It then creates a new `BookingsInfo` object with the given `ClientId`
            return View(bookingsInfo);  // and returns the Create view, passing this object.
        }

        // POST: BookingsInfoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BookingsInfo bookingsInfo, string RoomIds)
        {  /* This is an HTTP POST action method for handling the creation of a booking. It takes two parameters, which are `bookingsInfo` (represents the booking information), and `EoomIds` (a string containing comma-separated room IDs selected for the booking.) */ 
            
            // Check if ClientId is not provided, and try to get it from ViewBag
            if (bookingsInfo.ClientId == 0)
            {
                if (ViewBag.ClientId != null)
                {  
                    bookingsInfo.ClientId = ViewBag.ClientId;
                }
                else  // If `ClientId` is not found in ViewBag, it redirects to the ClientsInfoes/Index page. 
                {  
                    // Handle the case when clientId is not provided by redirecting to the ClientsInfoes/Index page
                    return RedirectToAction("Index", "ClientsInfoes");
                }
            }

            using (var dbContext = new LEC2023Entities())
            {
                if (Session["Username"] != null)  // Checks if the user is logged in, and proceeds to create the booking.
                {
                    long CreatedBy = (long)Session["UserId"];
                    long clientIdToUse = ViewBag.ClientId != null ? ViewBag.ClientId : bookingsInfo.ClientId;
                    /* - If `ViewBag.ClientId` is not `null`, `clientIdToUse` will be set to the value of `ViewBag.ClientId`.
                       - If `ViewBag.ClientId` is `null`, `clientIdToUse` will be set to the value of `bookingsInfo.ClientId`. */

                    // Process the selected room numbers
                    var roomNumbers = RoomIds.Split(','); // Split the comma-separated room IDs string into an array of room numbers.
                    var joinedRoomNumbers = string.Join(",", roomNumbers); // Joins the room IDs back into a single string with commas

                    try
                    {
                        // Call the stored procedure to insert the booking information into the database and pass the necessary parameters
                        dbContext.USP_BookingsInfo_BookingRoomsDetails_Insert(
                            clientIdToUse,
                            bookingsInfo.CheckInDate,
                            bookingsInfo.CheckOutDate,
                            joinedRoomNumbers, // Pass the joined room IDs here
                            CreatedBy
                        );

                        // Save the changes to the database context 
                        dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog(ex.Message, ex.StackTrace, ex.Source, 0);
                    }

                    // Set a success message in TempData for display in the redirected page
                    TempData["ConfirmationMessage"] = "Booking created successfully.";
                    /* After a successful booking, it sets a success message in TempData, which will be displayed on the redirected page. */

                    return RedirectToAction("Create");  // Redirect to the Create action after successful booking, which will show the Create view for another booking or display the success message.
                }

            }
            return View(bookingsInfo);
        }

        // GET: BookingsInfoes/GetAvailableRooms
        public ActionResult GetAvailableRooms(DateTime? checkInDate, DateTime? checkOutDate)
        {
            // Manually check for validation
            // Validate check-in and check-out dates
            if (!checkInDate.HasValue || !checkOutDate.HasValue)
            {
                return Json(new { error = "Please enter both check-in and check-out dates." }, JsonRequestBehavior.AllowGet);
            }

            if (checkInDate >= checkOutDate)
            {
                return Json(new { error = "Check-out date must be after check-in date." }, JsonRequestBehavior.AllowGet);
            }

            using (var dbContext = new LEC2023Entities())
            {

                // Retrieve available rooms
                var availableRooms = dbContext.USP_BookingsInfo_BookingRoomsDetails_GetByCheckInCheckOutDate(checkInDate, checkOutDate).Select(
                    ar => new AvailableRoomViewModel
                    {
                        RoomId = ar.AvailableRoomIds,
                        RoomFloor = ar.RoomFloor,
                        RoomUnitPrice = (int)ar.RoomUnitPrice
                    }).ToList();

                // Check if rooms are available and pass them to the view
                if (availableRooms.Count > 0)
                {
                    ViewBag.AvailableRooms = availableRooms; // Pass available rooms to the view
                    return Json(availableRooms, JsonRequestBehavior.AllowGet);
                    //return View(availableRooms);
                }
                else
                {
                    //ModelState.AddModelError("", "No available rooms for the selected dates.");
                }

                return Json(availableRooms, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: BookingsInfoes/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookingsInfo bookingsInfo = db.BookingsInfoes.Find(id);
            if (bookingsInfo == null)
            {
                return HttpNotFound();
            }
            return View(bookingsInfo);
        }

        // POST: BookingsInfoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BookingsInfo bookingsInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bookingsInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bookingsInfo);
        }

        // GET: BookingsInfoes/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookingsInfo bookingsInfo = db.BookingsInfoes.Find(id);
            if (bookingsInfo == null)
            {
                return HttpNotFound();
            }
            return View(bookingsInfo);
        }

        // POST: BookingsInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            BookingsInfo bookingsInfo = db.BookingsInfoes.Find(id);

            if (bookingsInfo == null)
            {
                // Handle the case where the booking record doesn't exist
                return Json(new { success = false, message = "Booking not found." });
            }

            if (ModelState.IsValid)
            {
                using (var dbContext = new LEC2023Entities())
                {
                    if (Session["Username"] != null)
                    {
                        long ModifiedBy = (long)Session["UserId"];
                        int returnCode;
                        string returnMessage;

                        try
                        {
                            // Call the stored procedure and pass the necessary parameters
                            //dbContext.USP_BookingsInfo_BookingRoomsDetails_DeleteById(id, ModifiedBy);

                            SqlParameter idParam = new SqlParameter("@BookingID", id);
                            SqlParameter modifiedByParam = new SqlParameter("@ModifiedBy", ModifiedBy);

                            // set variable for output parameter to capture the stored procedure return value
                            var returnCodeParam = new SqlParameter("@ReturnCode", SqlDbType.Int);
                            returnCodeParam.Direction = ParameterDirection.Output;

                            var returnMessageParam = new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 1000);
                            returnMessageParam.Direction = ParameterDirection.Output;

                            // assign thr sql query to a variable
                            string query = "EXEC USP_BookingsInfo_BookingRoomsDetails_DeleteById @BookingID, @ModifiedBy";

                            // exec SP
                            var result = db.Database.SqlQuery<USP_BookingsInfo_BookingRoomsDetails_DeleteById_Result>(query, idParam, modifiedByParam).FirstOrDefault();


                            // assign returned output param to variable
                            returnMessage = result.ReturnMessage;
                            returnCode = (int)result.ReturnCode;

                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLog(ex.Message, ex.StackTrace, ex.Source, 0);

                            //ModelState.AddModelError("", "An error occurred while deleting the booking information: " + ex.Message);
                            // Handle exceptions and return an error response
                            return Json(new { success = false, message = "Error deleting booking: " + ex.Message });
                        }

                        // Return a success response
                        return Json(new { success = returnCode, message = returnMessage });

                    }
                }
            }
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
