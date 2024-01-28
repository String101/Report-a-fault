using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Report_a_Fault.Models
{
    public class Fault
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        public string FaultDescription { get; set; }
        [ForeignKey("Computer")]
        public string computerId { get; set; }
        [ValidateNever]
        public Computer Computer { get; set; }

        
    }
}
