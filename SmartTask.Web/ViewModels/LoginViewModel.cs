using System.ComponentModel.DataAnnotations;

namespace SmartTask.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [MinLength(3, ErrorMessage = "User Name Must be Between 3 to 10 Character")]
        [MaxLength(10, ErrorMessage = "The Name Must be Between 3 to 10 Character")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", ErrorMessage = "Password must be Minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
