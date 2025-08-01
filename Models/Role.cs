﻿using System.ComponentModel.DataAnnotations;

namespace SimpleApiProject.Models;

// Represents a Role entity, like Admin, User, etc.
public class Role : BaseEntity
{
    [Required]
    [MaxLength(50)] // Max length constraint for Role name
    public string Name { get; set; }

    // Navigation property for many-to-many relationship with Users
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
