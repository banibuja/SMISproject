using AuthSystem.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthSystem.Models
{
    public class UserSubject
    {
        [Key]
        public int Id { get; set; }

        // Foreign Key to ApplicationUser
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }  // Assuming ApplicationUser uses string as the key

        public ApplicationUser? User { get; set; }

        // Foreign Key to Subject
        [ForeignKey("Subject")]
        public int SubjectId { get; set; }

        public Subject? Subject { get; set; }
    }
}