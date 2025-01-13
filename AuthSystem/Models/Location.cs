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

        [Required]
        [StringLength(200)]
        public string Address { get; set; } 

        [StringLength(50)]
        public string City { get; set; } 

        [StringLength(50)]
        public string State { get; set; } 
    }
}

