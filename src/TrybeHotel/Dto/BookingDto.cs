namespace TrybeHotel.Dto
{
    public class BookingDtoInsert
    {
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public int GuestQuant { get; set; }
        public int RoomId { get; set; }
    }

    public class BookingResponse
    {
        public int bookingId { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public int guestQuant { get; set; }
        public RoomDto room { get; set; }
    }
}