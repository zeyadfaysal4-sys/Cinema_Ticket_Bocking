using Cinema_Ticket_Bocking.Models;

namespace Cinema_Ticket_Bocking.ViewModel
{
    public class MovieVM
    {
        public Movie Movie { get; set; } 

        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Cinema> Cinemas { get; set; } = new List<Cinema>();
        public IEnumerable<Actors> Actors { get; set; } = new List<Actors>();

        public List<string> SupImages { get; set; } = new();
    }
}
