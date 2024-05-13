
namespace HotelRoomBookingSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class BookingsInfo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BookingsInfo()
        {
            this.BookingRoomsDetails = new HashSet<BookingRoomsDetail>();
        }

        public long Id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Client Id")]
        public long ClientId { get; set; }

        [Display(Name = "Booking Date")]
        [DisplayFormat(DataFormatString = "{0:M/d/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> BookingDate { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Check In Date")]
        [DisplayFormat(DataFormatString = "{0:M/d/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> CheckInDate { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Check Out Date")]
        [DisplayFormat(DataFormatString = "{0:M/d/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> CheckOutDate { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedAt { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public short Status { get; set; }


        // Include properties from BookingRoomsDetails
        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Room(s) No.")]
        [RegularExpression(@"^\d+(,\d+)*$", ErrorMessage = "Please enter a list of numeric values separated by commas.")]
        public string RoomIds { get; set; }

        [Display(Name = "Room No.")]
        public long RoomId { get; set; }

        [Display(Name = "Floor")]
        public string RoomFloor { get; set; }

        [Display(Name = "Unit Price (RM)")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Please enter a valid integer value.")]
        public Nullable<int> RoomUnitPrice { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BookingRoomsDetail> BookingRoomsDetails { get; set; }
    }
}
