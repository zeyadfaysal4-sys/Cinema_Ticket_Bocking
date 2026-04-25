using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Cinema_Ticket_Bocking.Models
{
    [PrimaryKey(nameof(Id))]
    public class Category
    {
        public int Id { get; set; }
        [MinLength(2)]
        [MaxLength(20)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Status { get; set; }
    }
}
