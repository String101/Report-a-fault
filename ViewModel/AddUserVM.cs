using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Report_a_Fault.ViewModel
{
    public class AddUserVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        public string Role { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CampusList { get; set; }

        public string CampusId { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> DepartmentList { get; set; }

        public string DepartmentId { get; set; }
        [ValidateNever]
        public string RedirectUrl { get; set; }
    }
}
