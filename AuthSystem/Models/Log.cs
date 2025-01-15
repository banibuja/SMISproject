using AuthSystem.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthSystem.Models
{
    public class Log
    {
        public int Id { get; set; }
        public string Action { get; set; }  
        public string? Entity { get; set; }  
        public int? EntityId { get; set; }   
        public string UserId { get; set; }  
        public DateTime Timestamp { get; set; }  
        public string Details { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
