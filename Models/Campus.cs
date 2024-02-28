using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Report_a_Fault.Models
{
    public class Campus
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string CampusId { get; set; }
        public string CampusName { get; set; }
        public DateTime DateOpended { get; set; }
    }
}
