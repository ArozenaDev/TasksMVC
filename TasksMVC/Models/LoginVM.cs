using System.ComponentModel.DataAnnotations;

namespace TasksMVC.Models
{
    public class LoginVM
    {
        [Required(ErrorMessage = "El campo{0} es obligatorio")]
        [EmailAddress(ErrorMessage = "El campo debe ser un correo electrónico válido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Recuérdame")]
        public bool RememberMe { get; set; }
    }
}
