using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SimpleApiProject.Controllers
{
    // Route pattern: api/user
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // This controller will handle all HTTP requests related to users.
    }
}
