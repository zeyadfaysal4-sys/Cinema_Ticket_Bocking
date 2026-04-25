using System.ComponentModel.DataAnnotations;

namespace Cinema_Ticket_Bocking.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [MinLength(2)]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }      
        public decimal Price { get; set; }
       public bool Status { get; set; }
        public string MainImg { get; set; }
        public List<string> SupImages { get; set; } = new List<string>();
        public DateTime Date { get; set; }
        public List<Actors> Actors { get; set; } = new();
        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
