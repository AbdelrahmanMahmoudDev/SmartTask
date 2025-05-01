using System.ComponentModel.DataAnnotations;

namespace SmartTask.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [MinLength(3, ErrorMessage = "User Name Must be Between 3 to 10 Character")]
        [MaxLength(10, ErrorMessage = "The Name Must be Between 3 to 10 Character")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "The Name Must be Between 6 to 20 Character")]
        [MaxLength(20, ErrorMessage = "The Name Must be Between 6 to 20 Character")]
        [Display(Name = "Full Name")]
        public string FName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", ErrorMessage = "Password must be Minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character")]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Confirm Password Must Match Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmed Password")]
        public string ConfirmedPassowrd { get; set; }
        [Required]
        [Display(Name ="Phone Number")]
        [RegularExpression("^[\\+]?[(]?[0-9]{3}[)]?[-\\s\\.]?[0-9]{3}[-\\s\\.]?[0-9]{4,6}$", ErrorMessage = "Enter a Valid Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
