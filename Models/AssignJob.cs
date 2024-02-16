using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Report_a_Fault.Models
{
    public class AssignJob
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ValidateNever]
        public string Id { get; set; }
        public string InternId { get; set; }
        [ValidateNever]
        public ApplicationUser Intern { get; set; }
        public string? FaultId { get; set; }
        [ValidateNever]
        public Fault Fault { get; set; }
    }
}
