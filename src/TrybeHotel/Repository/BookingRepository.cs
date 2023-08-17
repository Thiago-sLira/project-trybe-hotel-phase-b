using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class BookingRepository : IBookingRepository
    {
        protected readonly ITrybeHotelContext _context;
        public BookingRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public BookingResponse Add(BookingDtoInsert booking, string email)
        {
            var roomToBook = _context.Rooms.Find(booking.RoomId);

            var user = _context.Users.Find(email);

            if (roomToBook.Capacity >= int.Parse(booking.GuestQuant))
            {
                var bookingToSave = new Booking
                {
                    CheckIn = DateTime.Parse(booking.CheckIn),
                    CheckOut = DateTime.Parse(booking.CheckOut),
                    GuestQuant = int.Parse(booking.GuestQuant),
                    RoomId = booking.RoomId,
                    UserId = user.UserId
                };

                _context.Bookings.Add(bookingToSave);
                _context.SaveChanges();

                var content = from book in _context.Bookings
                              join room in _context.Rooms on book.RoomId equals room.RoomId
                              join hotel in _context.Hotels on room.HotelId equals hotel.HotelId
                              join city in _context.Cities on hotel.CityId equals city.CityId
                              select new BookingResponse
                              {
                                  bookingId = book.BookingId,
                                  CheckIn = book.CheckIn.ToString("yyyy-MM-dd"),
                                  CheckOut = book.CheckOut.ToString("yyyy-MM-dd"),
                                  guestQuant = book.GuestQuant.ToString(),
                                  room = new RoomDto
                                  {
                                      roomId = room.RoomId,
                                      name = room.Name,
                                      capacity = room.Capacity,
                                      image = room.Image,
                                      hotel = new HotelDto
                                      {
                                          hotelId = room.Hotel.HotelId,
                                          name = room.Hotel.Name,
                                          address = room.Hotel.Address,
                                          cityId = room.Hotel.CityId,
                                          cityName = city.Name
                                      }
                                  }
                              };
                return content.Last();

            }
            else
            {
                return null;
            }

        }

        public BookingResponse GetBooking(int bookingId, string email)
        {
            throw new NotImplementedException();
        }

        public Room GetRoomById(int RoomId)
        {
            throw new NotImplementedException();
        }

    }

}