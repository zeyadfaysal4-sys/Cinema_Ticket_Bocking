using System.ComponentModel.DataAnnotations;

namespace Cinema_Ticket_Bocking.ViewModel
{
    public class NewPasswordVM
    {
        public int Id { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password) , Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public string  UserId { get; set; }

        public string Token { get; set; }

    }
}
