using Cinema_Ticket_Bocking.ViewModel;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Ticket_Bocking.Models
{
    [PrimaryKey(nameof(MovieId), nameof(ApplicationUserId))]

    public class Cart
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }

    }
}
