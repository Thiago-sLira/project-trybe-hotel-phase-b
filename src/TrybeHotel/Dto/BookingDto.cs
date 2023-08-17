namespace TrybeHotel.Dto
{
    public class BookingDtoInsert
    {
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public string GuestQuant { get; set; }
        public int RoomId { get; set; }
    }

    public class BookingResponse
    {
        public int bookingId { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public string guestQuant { get; set; }
        public RoomDto room { get; set; }
    }
}