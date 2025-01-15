using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AuthSystem.Areas.Identity.Data;

namespace AuthSystem.Models
{
    public class Exam
    {
        [Key]
        public int Id { get; set; }


        [ForeignKey("UserSubject")]
        public int UserSubjectId { get; set; }
        public UserSubject? UserSubject { get; set; }


        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

    }
}