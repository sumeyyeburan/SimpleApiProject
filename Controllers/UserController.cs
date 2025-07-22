using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SimpleApiProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    // This endpoint is only available to those with the "Admin" role.
    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult AdminOnly()
    {
        return Ok("Sadece Admin kullanıcılar buraya erişebilir.");
    }

    //  This endpoint is open to everyone
    [AllowAnonymous]
    [HttpGet("public")]
    public IActionResult PublicAccess()
    {
        return Ok("Bu endpoint herkese açık.");
    }

    // This endpoint is open to any logged in user (no matter the role).
    [Authorize]
    [HttpGet("authenticated")]
    public IActionResult AuthenticatedUser()
    {
        var username = User.Identity.Name; // Get username from token
        return Ok($"Giriş yapmış kullanıcı: {username}");
    }
}