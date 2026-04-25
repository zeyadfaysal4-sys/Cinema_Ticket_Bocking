using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Cinema_Ticket_Bocking.Models
{
    [PrimaryKey(nameof(Id))]
    public class Actors
    {
        public int Id { get; set; }
        [MinLength(2)]
        [MaxLength(30)]
        public string Name { get; set; }
        public string Img { get; set; } = "default.png";

    }
}
