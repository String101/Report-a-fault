using Report_a_Fault.Models;

namespace Report_a_Fault.ViewModel
{
    public class SearchFaultVM
    {
        public PaginatedList<Fault> ReportedFault {  get; set; }
        public string Search {  get; set; }
    }
}
