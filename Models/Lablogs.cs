using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Report_a_Fault.Models
{
    public class Lablogs
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
         public string Id { get; set; }= Guid.NewGuid().ToString();
        public int labNumber { get; set; }
        public int studentNumber { get; set; }
        public DateTime LogDate { get; set; }= DateTime.Now;
    }
}
