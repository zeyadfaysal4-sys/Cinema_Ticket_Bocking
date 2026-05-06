using Microsoft.AspNetCore.Identity;

namespace Cinema_Ticket_Bocking.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

}
