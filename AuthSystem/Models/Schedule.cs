using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; } 

        [Required]
        [StringLength(100)]
        public string Name { get; set; } 
    }
}
