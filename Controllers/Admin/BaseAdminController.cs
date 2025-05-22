using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace hyperTROPHYbuddy.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public abstract class BaseAdminController : Controller
    {
        protected string GetAdminId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
