using Cinema_Ticket_Bocking.Models;

namespace Cinema_Ticket_Bocking.ViewModel
{
    public class MovieWithRelatedVM
    {
        public Movie Movie { get; set; }

        public IEnumerable<Movie> RelatedMovies { get; set; }
    }
}
