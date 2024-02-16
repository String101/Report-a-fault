using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Report_a_Fault.Models;

namespace Report_a_Fault.ViewModel
{
    public class AssignVM
    {
        public AssignJob FaultAssign { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> EmployeeList { get; set; }
    }
}
