using Report_a_Fault.Models;

namespace Report_a_Fault.ViewModel
{
    public class SearchComputerVM
    {
        public PaginatedList<Computer> Computers { get; set; }
        public string Search {  get; set; }
        public string LabIdentityNumber { get; set; }
    }
}
