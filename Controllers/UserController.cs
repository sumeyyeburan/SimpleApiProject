using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SimpleApiProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    /// <summary>
    /// Accessible only by users with the "Admin" role.
    /// </summary>
    /// <returns>Content for Admins only</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult AdminOnly()
    {
        return Ok("Only Admin users can access this endpoint.");
    }

    /// <summary>
    /// Public endpoint accessible by anyone.
    /// </summary>
    /// <returns>Public message</returns>
    [AllowAnonymous]
    [HttpGet("public")]
    public IActionResult PublicAccess()
    {
        return Ok("This endpoint is open to everyone.");
    }

    /// <summary>
    /// Endpoint accessible by all authenticated users.
    /// </summary>
    /// <returns>Username info</returns>
    [Authorize]
    [HttpGet("authenticated")]
    public IActionResult AuthenticatedUser()
    {
        var username = User.Identity.Name; // Get username from token
        return Ok($"Authenticated user: {username}");
    }
}