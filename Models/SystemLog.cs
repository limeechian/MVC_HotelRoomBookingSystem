
namespace HotelRoomBookingSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SystemLog
    {
        public long Id { get; set; }
        public System.DateTime LogDate { get; set; }
        public int Type { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Parameters { get; set; }
    }
}
