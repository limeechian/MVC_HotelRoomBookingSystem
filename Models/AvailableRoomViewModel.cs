using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelRoomBookingSystem.Models
{

    public class AvailableRoomViewModel
    {
        public long RoomId { get; set; }
        public string RoomFloor { get; set; }
        public int RoomUnitPrice { get; set; }
    }
}
