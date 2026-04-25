namespace Cinema_Ticket_Bocking.Models
{
    public class Book
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int NumberOfTickets { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime ShowTime { get; set; }
        public bool Status { get; set; }
    }
}
