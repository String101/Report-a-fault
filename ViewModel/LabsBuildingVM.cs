using Report_a_Fault.Models;

namespace Report_a_Fault.ViewModel
{
    public class LabsBuildingVM
    {
        public IEnumerable<Labs> Lab { get; set; }
        public string BuildindId { get; set; }
    }
}
