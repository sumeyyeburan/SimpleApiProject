using System.ComponentModel.DataAnnotations;

namespace SimpleApiProject.Models;

// Base entity class to be inherited by all database entities
public class BaseEntity
{
    [Key] // Primary key for the entity
    public Guid Id { get; set; }

    // User ID who created this entity
    public Guid CreatedBy { get; set; }

    // User ID who last updated this entity
    public Guid UpdatedBy { get; set; }

    // Timestamp when the entity was created, defaults to current UTC time
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Timestamp when the entity was last updated, defaults to current UTC time
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Status field to indicate entity state (e.g. active, deleted, etc.)
    public int Status { get; set; }
}
