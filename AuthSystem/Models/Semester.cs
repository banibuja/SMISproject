using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Models
{
    public class Semester
    {
        [Key]
        public int SemesterId { get; set; } 

        [Required]
        [StringLength(100)]
        public string Name { get; set; } 
    }
}
