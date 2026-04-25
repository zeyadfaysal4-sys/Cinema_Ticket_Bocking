using Cinema_Ticket_Bocking.Models;

namespace Cinema_Ticket_Bocking.FilterMovieVm
{
    public class FilterMovieVM
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
        public bool Status { get; set; }
        public int CategoryId { get; set; }
        public int CinemaId { get; set; }
        public int ActorId { get; set; }
        public int Page { get; set; } = 1;
    }
}
