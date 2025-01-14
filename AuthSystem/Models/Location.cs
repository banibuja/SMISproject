using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; } 
        [Required]
        [StringLength(100)]
        public string Name { get; set; } 



    }
}

