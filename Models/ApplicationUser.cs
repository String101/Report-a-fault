using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Report_a_Fault.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Display(Name ="Email")]
        public string Email { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Surname")]
        public string Usersurname { get; set; }
        [Display(Name = "Role")]
        public string Role { get; set; }
        public string CampusId { get; set; }
        [ValidateNever]
        public Campus Campus { get; set; }

        public bool Enabled { get; set; } = true;
        public string? DepartmentId { get; set; }
        [ValidateNever]
        public Department Department { get; set; }
    }
}
