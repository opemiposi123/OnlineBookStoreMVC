using System.ComponentModel.DataAnnotations;

namespace OnlineBookStoreMVC.Models
{
    public class VerifyResetCodeModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Reset code is required")]
        public string Code { get; set; }
    }

}
