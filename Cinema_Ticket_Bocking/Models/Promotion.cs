using Microsoft.EntityFrameworkCore;

namespace Cinema_Ticket_Bocking.Models
{
    [Index(nameof(Code), IsUnique = true)]
    public class Promotion
    {
        public int Id { get; set; }

        public string Code { get; set; }
        public int MaxUsage { get; set; }
        public int Discount { get; set; }
        public bool IsValid { get; set; }
        public DateTime ValidTo { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
