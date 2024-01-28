using Report_a_Fault.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Report_a_Fault.ViewModel
{
    public class ComputerVM
    {
        public Computer Computer { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> LabList { get; set; }
    }
}
