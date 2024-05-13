using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HotelRoomBookingSystem.Models;

namespace HotelRoomBookingSystem.Controllers
{
    [StaffAuthorizationFilter]
    public class RoomsInfoesController : Controller
    {
        private readonly LEC2023Entities db = new LEC2023Entities();

        // GET: RoomsInfoes
        public ActionResult Index(string id)
        {
            short status = 1; // The value for the @Status parameter
            SqlParameter param = new SqlParameter("@Status", status);

            var rooms = db.Database.SqlQuery<RoomsInfo>("EXEC USP_RoomsInfo_GetByStatus @Status", param).ToList();
           
            if (!string.IsNullOrEmpty(id))
            {
                // Expect an input to be numeric, but you want to handle cases where it might not be, without causing an exception.
                if (long.TryParse(id, out long numericId))  // `if` statement checks the result of the `long.TryParse` succeeds in parsing `id` into a `long`, the condition will be `true`, and the code inside the `if` block will be executed.
                /* Checking whether the `id` variable, which is a string, can be successfully parsed into a `long` (a numeric data type) using the `long.TryParse` method.
                   - a method call to `long.TryParse`, which attempts to parse the `id` variable (which is a string) into a `long`.
                     This method returns a `bool` indicating whether the parsing was successful. If it succeeds, it assigns the parsed `long` value to the `numericId` variable.
                   Checking if the `id` is a valid numeric string that can be converteed to a `long`. 
                   If it is, it enters the `if` block, and you can work with the `numericId` variable, which holds the parsed numeric value of `id`. */
                {
                    var roomInfoById = rooms.Where(r => r.Id == numericId).ToList();
                    if (roomInfoById.Count == 0)
                        ViewBag.RoomNotFound = "Room No. not found.";
                    return View(roomInfoById);
                }
                else  // If the parsing failes, the code inside the `else` block will be executed
                {
                    var roomInfoById = rooms.Where(r => r.Id == numericId).ToList();
                    if (roomInfoById.Count == 0)
                        ViewBag.RoomNotFound = "Error: Please enter a numeric ID.";
                    return View(roomInfoById);
                }
            }
            return View(rooms);
        }
    
        // GET: RoomsInfoes/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomsInfo roomsInfo = db.RoomsInfoes.Find(id);
            if (roomsInfo == null)
            {
                return HttpNotFound();
            }
            return View(roomsInfo);
        }

        // GET: RoomsInfoes/Create
        public ActionResult Create()
        {
            using (var dbContext = new LEC2023Entities())
            {
                // Get the latest room ID from the RoomsInfo table
                int latestRoomId = (int)dbContext.RoomsInfoes.OrderByDescending(r => r.Id).Select(r => r.Id).FirstOrDefault();

                // Calculate the next room ID
                int nextRoomId = latestRoomId + 1;

                // Pass the previewed room ID to the view
                ViewBag.PreviewedRoomId = nextRoomId;

                return View();
            }
        }

        // POST: RoomsInfoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoomsInfo roomsInfo)
        {
            if (ModelState.IsValid)
            {
                using (var dbContext = new LEC2023Entities())
                {
                    if (Session["Username"] != null)
                    {
                        long CreatedBy = (long)Session["UserId"];

                        try
                        {
                            // Call the stored procedure to insert a new room
                            dbContext.USP_RoomsInfo_Insert(
                                roomsInfo.RoomFloor,
                                roomsInfo.RoomUnitPrice,
                                CreatedBy
                                );

                            // Set a success message in TempData for display in the redirected page
                            TempData["ConfirmationMessage"] = "Room created successfully.";

                            // Save the changes to the database context
                            dbContext.SaveChanges();

                        } 
                        catch (Exception ex)
                        {
                            Logger.WriteLog(ex.Message, ex.StackTrace, ex.Source, 0);
                        }

                        // Set a success message in TempData for display in the redirected page
                        //TempData["ConfirmationMessage"] = "Room created successfully.";

                        return RedirectToAction("Create");
                    }
                }              
            }
            return View(roomsInfo);
        }

        // GET: RoomsInfoes/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomsInfo roomsInfo = db.RoomsInfoes.Find(id);
            if (roomsInfo == null)
            {
                return HttpNotFound();
            }
            return View(roomsInfo);
        }

        // POST: RoomsInfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RoomsInfo roomsInfo)
        {
            if (ModelState.IsValid)
            {
                using (var dbContext = new LEC2023Entities())
                {
                    if (Session["Username"] != null)
                    {
                        long ModifiedBy = (long)Session["UserId"];

                        try
                        {
                            // Call the stored procedure to update the room information
                            dbContext.USP_RoomsInfo_UpdateById(
                                (int?)roomsInfo.Id,
                                roomsInfo.RoomFloor,
                                roomsInfo.RoomUnitPrice,
                                ModifiedBy
                            );

                            dbContext.Entry(roomsInfo).State = EntityState.Modified;
                            // Save the changes to the database context
                            dbContext.SaveChanges();

                        } 
                        catch (Exception ex)
                        {
                            Logger.WriteLog(ex.Message, ex.StackTrace, ex.Source, 0);
                            //ModelState.AddModelError("", "An error occurred while updating the room information.");
                        }

                        // Set a success message in TempData for display in the redirected page
                        TempData["ConfirmationMessage"] = "Room information updated successfully.";

                        return RedirectToAction("Edit");
                    }
                }
            }
            return View(roomsInfo);
        }

        // GET: RoomsInfoes/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomsInfo roomsInfo = db.RoomsInfoes.Find(id);
            if (roomsInfo == null)
            {
                return HttpNotFound();
            }
            return View(roomsInfo);
        }

        // POST: RoomsInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            RoomsInfo roomsInfo = db.RoomsInfoes.Find(id);

            if (roomsInfo == null)
            {
                // Handle the case where the room record doesn't exist
                return Json(new { success = false, message = "Room not found." });
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
                            // Call the stored procedure and pass the necessary parameter
                            //dbContext.USP_RoomsInfo_DeleteById(id, ModifiedBy);

                            SqlParameter idParam = new SqlParameter("@RoomID", id);
                            SqlParameter modifiedByParam = new SqlParameter("@ModifiedBy", ModifiedBy);

                            // set variable for output parameter to capture the stored procedure return value
                            var returnCodeParam = new SqlParameter("@ReturnCode", SqlDbType.Int);
                            returnCodeParam.Direction = ParameterDirection.Output;

                            var returnMessageParam = new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 1000);
                            returnMessageParam.Direction = ParameterDirection.Output;

                            // assign thr sql query to a variable
                            string query = "EXEC USP_RoomsInfo_DeleteById @RoomID, @ModifiedBy";

                            // exec SP
                            var result = db.Database.SqlQuery<USP_RoomsInfo_DeleteById_Result>(query, idParam, modifiedByParam).FirstOrDefault();


                            // assign returned output param to variable
                            returnMessage = result.ReturnMessage;
                            returnCode = (int)result.ReturnCode;

                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLog(ex.Message, ex.StackTrace, ex.Source, 0);
                            // Handle exceptions and return an error response
                            return Json(new { success = false, message = "Error deleting room: " + ex.Message });
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
