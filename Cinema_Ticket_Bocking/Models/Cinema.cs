using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Cinema_Ticket_Bocking.Models
{
    [PrimaryKey(nameof(Id))]
    public class Cinema
    {
        public int Id { get; set; }
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Img { get; set; } = "default.png";
        public bool Status { get; set; }
    }
}
