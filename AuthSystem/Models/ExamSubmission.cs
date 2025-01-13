using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthSystem.Models
{
    public class ExamSubmission
    {
        [Key]
        public int ExamSubmissionId { get; set; }

        [Required]
        [StringLength(100)]
        public string ExamTitle { get; set; }

        [Required]
        public DateTime SubmissionDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }

        public Department? Department { get; set; }  
    }
}
