using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApiProject.Data;
using SimpleApiProject.DTOs;
using SimpleApiProject.Models;
using SimpleApiProject.Services;
using System.Collections.Generic;

namespace SimpleApiProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly AppDbContext _context;

    // Inject JwtTokenService and DbContext into the controller
    public AuthController(JwtTokenService jwtTokenService, AppDbContext context)
    {
        _jwtTokenService = jwtTokenService;
        _context = context;
    }

    /// <summary>
    /// Logs in a user and returns a JWT token.
    /// </summary>
    /// <param name="LoginDto">Email and password credentials</param>
    /// <returns>JWT token</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] DTOs.LoginDto LoginDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(new { errors });
        }

        // Check for missing credentials
        if (string.IsNullOrEmpty(LoginDto.Email) || string.IsNullOrEmpty(LoginDto.Password))
            return Unauthorized("Username or password cannot be empty.");

        // Retrieve user from database including related roles
        var user = await _context.Users
            .Include(u => u.UserRoles)          // Include UserRoles relationship
            .ThenInclude(ur => ur.Role)         // Then include the Role linked to each UserRole
            .FirstOrDefaultAsync(u => u.Email == LoginDto.Email && u.Status == BaseEntity.EntityStatus.Active); // Find user by email

        // If user not found or password is incorrect, deny access
        if (user == null || !BCrypt.Net.BCrypt.Verify(LoginDto.Password, user.PasswordHash))
            return Unauthorized("Incorrect email or password.");

        // Convert user's roles to a list of role names
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

        // Generate JWT token with user ID, email, and roles
        var token = _jwtTokenService.GenerateToken(user.Id.ToString(), user.Email, roles);

        // Return token in the response
        return Ok(new { Token = token });
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="RegisterDto">Email, password and username information</param>
    /// <returns>Registration result message</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto RegisterDto)
    {
        // Validate the incoming model
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(new { errors });
        }

        // Basic input validation
        if (string.IsNullOrEmpty(RegisterDto.Email) || string.IsNullOrEmpty(RegisterDto.Password))
            return BadRequest("Email and password cannot be empty.");

        // Check if the user already exists
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == RegisterDto.Email);
        if (existingUser != null)
            return BadRequest(new { message = "This email is already registered." });


        // Hash the password using BCrypt
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(RegisterDto.Password);

        // Create new user entity
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = RegisterDto.Email.ToLower(),  // Normalize email
            PasswordHash = hashedPassword,
            UserName = RegisterDto.UserName,
            Status = BaseEntity.EntityStatus.Active  // Set user as active
        };

        // Add the new user to the database
        _context.Users.Add(newUser);

        // Get or create the "User" role
        var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
        if (userRole == null)
        {
            // If the role doesn't exist, add it (optional)
            userRole = new Role { Id = Guid.NewGuid(), Name = "User" };
            _context.Roles.Add(userRole);
        }

        // Create the relation between user and role
        var newUserRole = new UserRole
        {
            UserId = newUser.Id,
            RoleId = userRole.Id
        };
        _context.UserRoles.Add(newUserRole);

        // Save changes to the database
        await _context.SaveChangesAsync();

        return Ok("Registration successful.");
    }

}