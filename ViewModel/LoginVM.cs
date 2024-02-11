using System.ComponentModel.DataAnnotations;

namespace Report_a_Fault.ViewModel
{
    public class LoginVM
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    
        public string? RedirectUrl { get; set; }
    }
}
