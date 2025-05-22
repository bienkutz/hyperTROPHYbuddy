using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace hyperTROPHYbuddy.Controllers.Client
{
    [Authorize(Roles = "Client")]
    [Route("Client/[controller]")]
    public abstract class BaseClientController : Controller
    {
        protected string GetClientId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
