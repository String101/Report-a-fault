using Report_a_Fault.Models;

namespace Report_a_Fault.ViewModel
{
    public class FaultUsersVM
    {
        public IEnumerable<Fault> Fault { get; set; }
        public ApplicationUser StudentAssistant { get; set; }
        public ApplicationUser? Intern { get;set; }
    }
}
