using Cinema_Ticket_Bocking.Models;

namespace Cinema_Ticket_Bocking.ViewModel
{
    public class MovieVM
    {
        public Movie Movie { get; set; } 

        public List<Category> Categories { get; set; } = new();
        public List<Cinema> Cinemas { get; set; } = new();
        public List<Actors> Actors { get; set; } = new();

        // لو محتاج صور رفع
        public List<string> SupImages { get; set; } = new();
    }
}
