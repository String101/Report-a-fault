using Report_a_Fault.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Report_a_Fault.ViewModel
{
    public class CompLabVM
    {

      public Labs Labs {  get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CampusList { get; set; }
    }
}
