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

            var user = _context.Users.Where(u => u.Email == email).FirstOrDefault();

            // if (roomToBook == null || user == null)
            //     return null;

            if (roomToBook.Capacity >= booking.GuestQuant)
            {
                var bookingToSave = new Booking
                {
                    CheckIn = DateTime.Parse(booking.CheckIn),
                    CheckOut = DateTime.Parse(booking.CheckOut),
                    GuestQuant = booking.GuestQuant,
                    RoomId = booking.RoomId,
                    UserId = user.UserId
                };

                _context.Bookings.Add(bookingToSave);
                _context.SaveChanges();

                var content = from book in _context.Bookings
                                  //   join room in _context.Rooms on book.RoomId equals room.RoomId
                                  //   join hotel in _context.Hotels on room.HotelId equals hotel.HotelId
                                  //   join city in _context.Cities on hotel.CityId equals city.CityId
                              orderby book.BookingId
                              select new BookingResponse
                              {
                                  bookingId = book.BookingId,
                                  CheckIn = book.CheckIn.ToString("yyyy-MM-dd"),
                                  CheckOut = book.CheckOut.ToString("yyyy-MM-dd"),
                                  guestQuant = book.GuestQuant,
                                  room = new RoomDto
                                  {
                                      roomId = book.Room.RoomId,
                                      name = book.Room.Name,
                                      capacity = book.Room.Capacity,
                                      image = book.Room.Image,
                                      hotel = new HotelDto
                                      {
                                          hotelId = book.Room.Hotel.HotelId,
                                          name = book.Room.Hotel.Name,
                                          address = book.Room.Hotel.Address,
                                          cityId = book.Room.Hotel.CityId,
                                          cityName = book.Room.Hotel.City.Name
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