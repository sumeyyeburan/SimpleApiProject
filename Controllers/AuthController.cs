using Microsoft.AspNetCore.Mvc;
using SimpleApiProject.Models;
using SimpleApiProject.Services;
using System.Collections.Generic;

namespace SimpleApiProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;

    // Inject JwtTokenService to generate JWT tokens
    public AuthController(JwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
    }

    // POST api/auth/login
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest loginRequest)
    {
        // Validate user credentials (this should be replaced with real DB or external service check)

        // Simple example validation for demonstration purposes
        if (!string.IsNullOrEmpty(loginRequest.Email) && !string.IsNullOrEmpty(loginRequest.Password))
        {
            if (loginRequest.Email.Trim().Equals("test@test.com") && loginRequest.Password.Trim().Equals("123"))
            {
                // Example user ID and roles; in real app, fetch from DB
                string userId = "1";
                var roles = new List<string> { "Admin", "User" };

                // Generate JWT token for authenticated user
                var token = _jwtTokenService.GenerateToken(userId, loginRequest.Email, roles);

                // Return the token in response
                return Ok(new { Token = token });
            }
            // Credentials don't match
            return Unauthorized("Invalid email or password.");
        }
        else
        {
            // Missing email or password in request
            return Unauthorized("Email and password cannot be empty.");
        }
    }
}
