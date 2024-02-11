using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Report_a_Fault.Models
{
    public class Department
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}
