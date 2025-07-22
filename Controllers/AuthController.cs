using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApiProject.Data;
using SimpleApiProject.Models;
using SimpleApiProject.Services;
using System.Collections.Generic;
using SimpleApiProject.DTOs;

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

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] DTOs.LoginDto LoginDto)
    {
        // Check for missing credentials
        if (string.IsNullOrEmpty(LoginDto.Email) || string.IsNullOrEmpty(LoginDto.Password))
            return Unauthorized("Kullanıcı adı veya şifre boş olamaz");

        // Retrieve user from database including related roles
        var user = await _context.Users
            .Include(u => u.UserRoles)          // Include UserRoles relationship
            .ThenInclude(ur => ur.Role)         // Then include the Role linked to each UserRole
            .FirstOrDefaultAsync(u => u.Email == LoginDto.Email); // Find user by email

        // If user not found or password is incorrect, deny access
        if (user == null || !BCrypt.Net.BCrypt.Verify(LoginDto.Password, user.PasswordHash))
            return Unauthorized("Email veya şifre hatalı");

        // Convert user's roles to a list of role names
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

        // Generate JWT token with user ID, email, and roles
        var token = _jwtTokenService.GenerateToken(user.Id.ToString(), user.Email, roles);

        // Return token in the response
        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto RegisterDto)
    {
        if (string.IsNullOrEmpty(RegisterDto.Email) || string.IsNullOrEmpty(RegisterDto.Password))
            return BadRequest("Email and password cannot be empty.");

        // Check if the user already exists
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == RegisterDto.Email);
        if (existingUser != null)
            return BadRequest("This email is already registered.");

        // Hash the password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(RegisterDto.Password);

        // Create a new user
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = RegisterDto.Email,
            PasswordHash = hashedPassword,

            UserName = RegisterDto.UserName
        };

        // Add the new user to the database
        _context.Users.Add(newUser);

        // Assign a role to the new user (for example "User")
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