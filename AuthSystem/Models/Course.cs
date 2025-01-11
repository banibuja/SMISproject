using AuthSystem.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthSystem.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }

        public Department? Department { get; set; }

        public ICollection<ApplicationUser>? Users { get; set; }
    }
}
