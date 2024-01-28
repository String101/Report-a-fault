using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Report_a_Fault.Models
{
    public class Computer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ValidateNever]
        public string Id { get; set; }
        [Display(Name = "Computer Number")]
        [MinLength(2)]
        public string ComputerNumber { get; set; }
        [ForeignKey("Lab")]
        public string LabId { get; set; }
        [ValidateNever]
        public Labs Lab { get; set; }
        [ValidateNever]
        public IEnumerable<Computer_comp> Computer_Comps { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public string ComputerComponentStatus { get; set; } = SD.StatusHealty;
    }
}
