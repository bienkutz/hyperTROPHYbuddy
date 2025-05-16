using hyperTROPHYbuddy.Services;
using Microsoft.AspNetCore.Mvc;

namespace hyperTROPHYbuddy.Controllers.Admin
{
    public class AssignController : Controller
    {
        private readonly IPlanService _planService;

        public AssignController(IPlanService planService)
        {
            _planService = planService;
        }

        // GET: Admin/Assign
        public async Task<IActionResult> Index()
        {
            var adminId = "hardcoded-admin-id";
            var plans = await _planService.GetPlansByAdmin(adminId);
            return View(plans);
        }

        // GET: Admin/Assign/AssignToUser/5
        public async Task<IActionResult> AssignToClient(int planId)
        {
            var adminId = "hardcoded-admin-id";
            var plan = await _planService.GetPlanById(planId, adminId);
            if (plan == null)
            {
                return NotFound();
            }

            ViewBag.PlanId = planId;
            return View();
        }

        // POST: Admin/Assign/AssignToUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignToClient(int planId, string clientId)
        {
            /*var adminId = "hardcoded-admin-id";
            try
            {
                await _planService.AssignPlanToUser(planId, userId, adminId);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.PlanId = planId;
                return View();
            }*/
            return RedirectToAction(nameof(Index));//temporary
        }
    }
}
