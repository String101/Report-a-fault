using System.ComponentModel.DataAnnotations;

namespace Report_a_Fault.ViewModel
{
    public class ResetPasswordViewM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string Token { get; set; }
    }
}
