namespace hotelhub_backend.Models
{
    public class BookingRequest
    {
            public int UserId { get; set; }
            public int RoomId { get; set; }
            public int HotelId { get; set; }
            public int Rent { get; set; }
            public DateTime CheckIn { get; set; }
            public DateTime CheckOut { get; set; }
    }
}
