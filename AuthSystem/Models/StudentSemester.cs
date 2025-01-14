using AuthSystem.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthSystem.Models
{

    //qeto e bona i thirra krejt
    public class StudentSemester
    {
        [Key]
        public int StudentSemesterId { get; set; }

        public int SemesterId { get; set; }
        [ForeignKey("SemesterId")]
        public Semester? Semester { get; set; }

        public int LocationId { get; set; }
        [ForeignKey("LocationId")]
        public Location? Location { get; set; }

        public int ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public Schedule? Schedule { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
