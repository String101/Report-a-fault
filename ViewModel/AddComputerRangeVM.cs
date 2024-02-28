using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Report_a_Fault.Models;

namespace Report_a_Fault.ViewModel
{
    public class AddComputerRangeVM
    {
        public int  NumberOfComputer { get; set; }
        public string LabId { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> BuildingList { get; set; }
    }
}
