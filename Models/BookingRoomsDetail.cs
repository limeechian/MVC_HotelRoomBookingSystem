
namespace HotelRoomBookingSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class BookingRoomsDetail
    {
        public long Id { get; set; }
     
        [Display(Name = "Id")]
        public long BookingId { get; set; }

        [Display(Name = "Room No.")]
        public long RoomId { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedAt { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public short Status { get; set; }


        public virtual BookingsInfo BookingsInfo { get; set; }

     
    }
}
