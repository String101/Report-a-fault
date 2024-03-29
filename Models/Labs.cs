﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Report_a_Fault.Models
{
    public class Labs
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Lab Number")]
        public string Id { get; set; }
        public string LabNumber { get; set; }
        [ForeignKey("Campus")]
        public string BuildingId { get; set; }
        [ValidateNever]
        public Building Building { get; set; }
        

    }
}
