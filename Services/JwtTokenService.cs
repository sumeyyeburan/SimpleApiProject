using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace SimpleApiProject.Services;

public class JwtTokenService
{
    private readonly IConfiguration _configuration;

    // Inject IConfiguration to access JWT settings from configuration
    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Generate JWT token based on user info and roles
    public string GenerateToken(string userId, string email, List<string> roles)
    {
        // Create claims for the token (user identifier, email, and roles)
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, email)
            };

        // Add each role as a separate claim
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Create symmetric security key using the secret key from config
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        // Define signing credentials using HMAC SHA256 algorithm
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create the JWT token with issuer, audience, claims, expiry, and signing credentials
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
            signingCredentials: creds
        );

        // Serialize the token to a string and return it
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
