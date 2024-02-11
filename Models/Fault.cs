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

        public string StudentEmail { get; set; }

        public string? InternEmail { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime DateReported { get; set; }= DateTime.Now;

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? DateUpdate { get; set; }


    }
}
