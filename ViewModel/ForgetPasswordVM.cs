using System.ComponentModel.DataAnnotations;

namespace Report_a_Fault.ViewModel
{
    public class ForgetPasswordVM
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }

}
