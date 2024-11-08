using System.ComponentModel.DataAnnotations;

namespace TasksMVC.Models
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Error.Required")]
        [EmailAddress(ErrorMessage = "Error.Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Error.Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "RememberMe")]
        public bool RememberMe { get; set; }
    }
}
