
namespace HotelRoomBookingSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class RoomsInfo
    {
        [Display(Name = "Room No.")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Please select a floor")]
        [Display(Name = "Floor")]
        public string RoomFloor { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Unit Price (RM)")]
        public Nullable<int> RoomUnitPrice { get; set; }

        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedAt { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public short Status { get; set; }
    }
}
