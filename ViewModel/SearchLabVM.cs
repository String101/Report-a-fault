using Report_a_Fault.Models;

namespace Report_a_Fault.ViewModel
{
    public class SearchLabVM
    {
        public IEnumerable<Labs> AllBuildingLabs { get; set; }
        public string Search {  get; set; }
        public string labNumber { get; set; }
    }
}
