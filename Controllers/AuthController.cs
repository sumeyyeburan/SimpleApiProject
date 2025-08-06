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
    /// <param name= "LoginDto" > Email and password credentials</param>
    /// <returns>JWT token</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto LoginDto)
    {
        if (string.IsNullOrEmpty(LoginDto.Email) && string.IsNullOrEmpty(LoginDto.UserName))
            return BadRequest(new { message = "Email or username is required." });

        if (string.IsNullOrEmpty(LoginDto.Password))
            return BadRequest(new { message = "Password is required." });

        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u =>
                (!string.IsNullOrEmpty(LoginDto.Email) && u.Email.ToLower() == LoginDto.Email.ToLower()) ||
                (!string.IsNullOrEmpty(LoginDto.UserName) && u.UserName.ToLower() == LoginDto.UserName.ToLower())
            );

        if (user == null || !BCrypt.Net.BCrypt.Verify(LoginDto.Password, user.PasswordHash))
            return Unauthorized("Incorrect email/username or password.");

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var token = _jwtTokenService.GenerateToken(user.Id.ToString(), user.Email, roles);

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

        var existingUserByEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == RegisterDto.Email.ToLower());
        if (existingUserByEmail != null)
            return BadRequest(new { message = "This email is already registered." });

        var existingUserByUsername = await _context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == RegisterDto.UserName.ToLower());
        if (existingUserByUsername != null)
            return BadRequest(new { message = "This username is already taken." });


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