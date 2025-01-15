using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using AuthSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthSystem.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "nvarchar(15)")]
    public string? StudentId { get; set; }  



    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string FirstName { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string LastName { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(50)")]
    public string PersonalNumber { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string ParentName { get; set; }

    [PersonalData]
    [Column(TypeName = "datetime")]
    public DateTime? BirthDate { get; set; }


    [PersonalData]
    [Column(TypeName = "nvarchar(10)")]
    public string Gender { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string BirthPlace { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string State { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string Residence { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(200)")]
    public string Address { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(10)")]
    public string ZipCode { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(256)")]
    public string PrivateEmail { get; set; }


    [PersonalData]
    [Column(TypeName = "nvarchar(50)")]
    public string Nationality { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(50)")]
    public string Citizenship { get; set; }

    public int? DepartmentId { get; set; }

    [ForeignKey("DepartmentId")]
    public Department? Department { get; set; }



}
