
namespace HotelRoomBookingSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class ClientsInfo
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Name")]
        public string ClientName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "IC/Passport")]
        [RegularExpression(@"^[0-9-]*$", ErrorMessage = "Please enter a valid value containing only numeric digits and '-' character.")]
        public string ClientIcPassport { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Phone No.")]
        [RegularExpression(@"^[0-9-]*$", ErrorMessage = "Please enter a valid value containing only numeric digits and '-' character.")]
        public string ClientPhoneNumber { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Email")]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$", ErrorMessage = "Please enter a valid email address.")]
        public string ClientEmail { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Gender")]
        public string ClientGender { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Birth Date")]
        [DisplayFormat(DataFormatString = "{0:M/d/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ClientBirthDate { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Address")]
        public string ClientAddress { get; set; }

        public Nullable<System.DateTime> CreatedAt { get; set; }

        public Nullable<long> CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedAt { get; set; }

        public Nullable<long> ModifiedBy { get; set; }

        public short Status { get; set; }
    }
}
