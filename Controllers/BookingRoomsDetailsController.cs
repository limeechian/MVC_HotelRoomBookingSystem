using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HotelRoomBookingSystem.Models;

namespace HotelRoomBookingSystem.Controllers
{
    [StaffAuthorizationFilter]
    public class BookingRoomsDetailsController : Controller
    {
        private readonly LEC2023Entities db = new LEC2023Entities();

        // GET: BookingRoomsDetails
        public ActionResult Index(string id)
        {
            short status = 1;
            SqlParameter param = new SqlParameter("@Status", status);

            var bookingRooms = db.Database.SqlQuery<BookingRoomsDetail>("EXEC USP_BookingRoomsDetails_GetByStatus @Status", param).ToList();

            if (bookingRooms.Count == 0)
                ViewBag.BookingRoomNotFound = "No Booking Found.";


            if (!string.IsNullOrEmpty(id))
            {
                if (long.TryParse(id, out long numericId))
                {
                    var bookingRoomInfoById = bookingRooms.Where(br => br.BookingId == numericId).ToList();
                    if (bookingRoomInfoById.Count == 0)
                        ViewBag.BookingRoomNotFound = "Booking ID not found.";
                    return View(bookingRoomInfoById);
                }
                else
                {
                    var bookingRoomInfoById = bookingRooms.Where(br => br.BookingId == numericId).ToList();
                    if (bookingRoomInfoById.Count == 0)
                        ViewBag.BookingRoomNotFound = "Error: Please enter a numeric ID.";
                    return View(bookingRoomInfoById);
                }
            }
            return View(bookingRooms);
        }
    


        // GET: BookingRoomsDetails/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookingRoomsDetail bookingRoomsDetail = db.BookingRoomsDetails.Find(id);
            if (bookingRoomsDetail == null)
            {
                return HttpNotFound();
            }
            return View(bookingRoomsDetail);
        }

        // GET: BookingRoomsDetails/Create
        public ActionResult Create()
        {
            ViewBag.BookingId = new SelectList(db.BookingsInfoes, "Id", "Id");
            return View();
        }

        // POST: BookingRoomsDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BookingRoomsDetail bookingRoomsDetail)
        {
            if (ModelState.IsValid)
            {
                db.BookingRoomsDetails.Add(bookingRoomsDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BookingId = new SelectList(db.BookingsInfoes, "Id", "Id", bookingRoomsDetail.BookingId);
            return View(bookingRoomsDetail);
        }

        // GET: BookingRoomsDetails/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookingRoomsDetail bookingRoomsDetail = db.BookingRoomsDetails.Find(id);
            if (bookingRoomsDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.BookingId = new SelectList(db.BookingsInfoes, "Id", "Id", bookingRoomsDetail.BookingId);
            return View(bookingRoomsDetail);
        }

        // POST: BookingRoomsDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BookingRoomsDetail bookingRoomsDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bookingRoomsDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BookingId = new SelectList(db.BookingsInfoes, "Id", "Id", bookingRoomsDetail.BookingId);
            return View(bookingRoomsDetail);
        }

        // GET: BookingRoomsDetails/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookingRoomsDetail bookingRoomsDetail = db.BookingRoomsDetails.Find(id);
            if (bookingRoomsDetail == null)
            {
                return HttpNotFound();
            }
            return View(bookingRoomsDetail);
        }

        // POST: BookingRoomsDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            BookingRoomsDetail bookingRoomsDetail = db.BookingRoomsDetails.Find(id);
            db.BookingRoomsDetails.Remove(bookingRoomsDetail);
            db.SaveChanges();
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
