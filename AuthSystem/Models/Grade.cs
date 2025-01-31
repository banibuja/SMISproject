﻿using System.ComponentModel.DataAnnotations;
using AuthSystem.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AuthSystem.Models
{
    public class Grade
    {
        [Key]
        public int Id { get; set; }
        public int Number { get; set; }
        [StringLength(1, ErrorMessage = "The letter must be 1 character long.")]
        public required string Letter { get; set; }
        public required string GradeStatus { get; set; }

        public required string StudentId { get; set; }
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }  // Assuming ApplicationUser uses string as the key

        public ApplicationUser? User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Automatically sets the current UTC time when a grade is added
    }
}
