﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitTrackerAPI.Models.UserInfo;

[Table("UserProfiles", Schema = "Core")]
public class UserProfile
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "varchar(100)")]
    public string FirstName { get; set; }

    [Required]
    [Column(TypeName = "varchar(100)")]
    public string LastName { get; set; }

    [Required]
    [Column(TypeName = "varchar(20)")]
    public string PhoneNumber { get; set; }

    [Required]
    [Column(TypeName = "decimal(5, 2)")] // Permite valores como 85.50
    public decimal Weight { get; set; }

    [Required]
    [Column(TypeName = "decimal(5, 2)")]
    public decimal Height { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateOfBirth { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Llave Foránea 
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; }
}