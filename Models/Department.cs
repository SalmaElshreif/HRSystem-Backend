using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GraduationProject.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public virtual List<Employee>? Employees {  get; set; }= new List<Employee>();
        [ForeignKey("company")]
        public int? Company_Id { get; set; }
        public virtual Company? company { get; set; }

    }
}
