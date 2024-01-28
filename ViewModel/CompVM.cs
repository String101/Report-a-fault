using Report_a_Fault.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Report_a_Fault.ViewModel
{
    public class CompVM
    {
        public Computer_comp ComputerComp { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> ComputerList { get; set; }
    }
}
