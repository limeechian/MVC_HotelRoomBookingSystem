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
    public class ClientsInfoesController : Controller
    {
        private readonly LEC2023Entities db = new LEC2023Entities();

        // GET: ClientsInfoes by Status = 1
        public ActionResult Index(string id)
        {
            short status = 1;
            SqlParameter param = new SqlParameter("@Status", status);

            var clients = db.Database.SqlQuery<ClientsInfo>("EXEC USP_ClientsInfo_GetByStatus @Status", param).ToList();
            
            if (clients.Count == 0)
                ViewBag.ClientNotFound = "No Client Found.";

            if (!string.IsNullOrEmpty(id))
            {
                if (long.TryParse(id, out long numericId))
                {
                    var clientInfoById = clients.Where(c => c.Id == numericId).ToList();
                    if (clientInfoById.Count == 0)
                        ViewBag.ClientNotFound = "Client ID not found.";
                    return View(clientInfoById);
                }
                else
                {
                    var clientInfoById = clients.Where(c => c.Id == numericId).ToList();
                    if (clientInfoById.Count == 0)
                        ViewBag.ClientNotFound = "Error: Please enter a numeric ID.";
                    return View(clientInfoById);
                }
            }
            return View(clients);
        }

        #region Details
        // GET: ClientsInfoes/Details/5
        // [Route("ClientsInfoes/Details/{id}")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientsInfo clientsInfo = db.ClientsInfoes.Find(id);
            if (clientsInfo == null)
            {
                return HttpNotFound();
            }
                return View(clientsInfo);
        }
        #endregion


        // GET: ClientsInfoes/Create        
        public ActionResult Create()
        {
            return View();
        }


        // POST: ClientsInfoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClientsInfo clientsInfo)
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
                            // Call the stored procedure to insert a new client 
                            dbContext.USP_ClientsInfo_Insert(
                                clientsInfo.ClientName,
                                clientsInfo.ClientIcPassport,
                                clientsInfo.ClientPhoneNumber,
                                clientsInfo.ClientEmail,
                                clientsInfo.ClientGender,
                                clientsInfo.ClientBirthDate,
                                clientsInfo.ClientAddress,
                                CreatedBy
                            );
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLog(ex.Message, ex.StackTrace, ex.Source, 0);
                        }

                        // Set a success message in TempData for display in the redirected page
                        TempData["ConfirmationMessage"] = "Client created successfully.";

                        return RedirectToAction("Create");
                    }
                }
            }
            return View(clientsInfo);
        }

                        
        // GET: ClientsInfoes/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientsInfo clientsInfo = db.ClientsInfoes.Find(id);
            if (clientsInfo == null)
            {
                return HttpNotFound();
            }
            return View(clientsInfo);
        }

        // POST: ClientsInfoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClientsInfo clientsInfo)
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
                            // Call the stored procedure to update the client information
                            dbContext.USP_ClientsInfo_UpdateById(
                                clientsInfo.Id,
                                clientsInfo.ClientName,
                                clientsInfo.ClientIcPassport,
                                clientsInfo.ClientPhoneNumber,
                                clientsInfo.ClientEmail,
                                clientsInfo.ClientGender,
                                clientsInfo.ClientBirthDate,
                                clientsInfo.ClientAddress,
                                ModifiedBy
                            );
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLog(ex.Message, ex.StackTrace, ex.Source, 0);
                        }

                        // Set a success message in TempData for display in the redirected page
                        TempData["ConfirmationMessage"] = "Client information updated successfully.";

                        return RedirectToAction("Edit");
                        }
                    }
                }
            return View(clientsInfo);
        }

        // GET: ClientsInfoes/Delete/5
        [HttpGet]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientsInfo clientsInfo = db.ClientsInfoes.Find(id);
            if (clientsInfo == null)
            {
                return HttpNotFound();
            }
            return View(clientsInfo);
        }

        // POST: ClientsInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ClientsInfo clientsInfo = db.ClientsInfoes.Find(id);

            if (clientsInfo == null)
            {
                // Handle the case where the client record doesn't exist
                return Json(new { success = false, message = "Client not found." });
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
                            //dbContext.USP_ClientsInfo_DeleteById(id, ModifiedBy); // need to have .FirstOrDefault() to get result datatype from the model 'USP_ClientsInfo_DeleteById_Result'
                            
                            SqlParameter idParam = new SqlParameter("@ClientID", id);
                            SqlParameter modifiedByParam = new SqlParameter("@ModifiedBy", ModifiedBy);

                            // set variable for output parameter to capture the stored procedure return value
                            var returnCodeParam = new SqlParameter("@ReturnCode", SqlDbType.Int);
                            returnCodeParam.Direction = ParameterDirection.Output;

                            var returnMessageParam = new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 1000);
                            returnMessageParam.Direction = ParameterDirection.Output;

                            // assign thr sql query to a variable
                            string query = "EXEC USP_ClientsInfo_DeleteById @ClientID, @ModifiedBy";

                            // exec SP
                            var result = db.Database.SqlQuery<USP_ClientsInfo_DeleteById_Result>(query, idParam, modifiedByParam).FirstOrDefault();

                            // assign returned output param to variable
                            returnMessage = result.ReturnMessage;
                            returnCode = (int)result.ReturnCode;
                        }
                        catch (Exception ex)
                        {
                            //ModelState.AddModelError("", "An error occurred while deleting the client information: " + ex.Message);
                            // Handle exceptions and return an error response
                            Logger.WriteLog(ex.Message, ex.StackTrace, ex.Source, 0);
                            return Json(new { success = false, message = "Error deleting client: " + ex.Message });
                        }

                        // Return a success response
                        return Json(new { success = returnCode, message = returnMessage });
                    }
                }
            }
            // Redirect to the "Index" action if deletion is successful and not coming from the "Edit" page
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
