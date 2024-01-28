using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Report_a_Fault.Models
{
    public class Computer_comp
    {
        [Key]
        public int Id { get; set; } 
        public required string Name { get; set; }
        [ForeignKey("Computer")]
        public string ComputerId { get; set; }
        [ValidateNever]
        public Computer Computer { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

       
    }
}
