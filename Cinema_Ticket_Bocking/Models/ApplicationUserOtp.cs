namespace Cinema_Ticket_Bocking.Models
{
    public class ApplicationUserOtp
    {
        public string Id { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string OTP { get; set; }

        public bool IsValied { get; set; }

        public DateTime ValiedTo { get; set; }
        public DateTime CreatedAt { get; set; }

        public ApplicationUserOtp() 
        {
            
        }
        public ApplicationUserOtp(string ApplicationUserId, string OTP)
        {
            this.ApplicationUserId = ApplicationUserId;
            this.OTP = OTP;
            Id = Guid.NewGuid().ToString();
            IsValied = true;
            ValiedTo = DateTime.UtcNow.AddMinutes(20);
            CreatedAt = DateTime.UtcNow;





        }
    }
}
