using Cinema_Ticket_Bocking.Models;

namespace Cinema_Ticket_Bocking.ViewModel
{
    public class BookingVM
    {
        public Book Book { get; set; }
        public List<Movie> Movies { get; set; } = new();
        public List<Book> Bookings { get; set; } = new();
    }
}
