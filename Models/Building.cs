using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Report_a_Fault.Models
{
    public class Building
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string BuildingId { get; set; }
        public string BuildingName { get; set;}
        public string CampusId { get; set; }
        [ValidateNever]
        public Campus Campus { get; set;}
    }
}
