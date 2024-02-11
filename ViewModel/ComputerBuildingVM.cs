using Report_a_Fault.Models;

namespace Report_a_Fault.ViewModel
{
    public class ComputerBuildingVM
    {
        public IEnumerable<Computer> Computers { get; set; }
        public string BuildingId { get; set; }
    }
}
