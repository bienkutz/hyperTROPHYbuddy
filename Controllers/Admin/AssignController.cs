using hyperTROPHYbuddy.Services;
using Microsoft.AspNetCore.Mvc;

namespace hyperTROPHYbuddy.Controllers.Admin
{
    public class AssignController : BaseAdminController
    {
        private readonly IPlanService _planService;
        private readonly IClientService _clientService;

        public AssignController(
            IPlanService planService,
            IClientService clientService)
        {
            _planService = planService;
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var assignments = await _planService.GetAssignedPlans(GetAdminId());
                return View(assignments);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
        }

        [HttpGet("AssignToClient/{planId}")]
        public async Task<IActionResult> AssignToClient(int planId)
        {
            try
            {
                var plan = await _planService.GetPlanById(planId, GetAdminId());
                if (plan == null)
                    return NotFound();

                ViewBag.PlanId = planId;
                ViewBag.PlanName = plan.Name;
                ViewBag.PlanType = plan.WorkoutPlanType.Name;
                ViewBag.WorkoutCount = plan.WorkoutPlanWorkouts.Count;

                return View("AssignToUser");
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("AssignToClient/{planId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignToClient(int planId, string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                ModelState.AddModelError("", "Client ID is required");
                ViewBag.PlanId = planId;
                var plan = await _planService.GetPlanById(planId, GetAdminId());
                ViewBag.PlanName = plan.Name;
                ViewBag.PlanType = plan.WorkoutPlanType.Name;
                ViewBag.WorkoutCount = plan.WorkoutPlanWorkouts.Count;
                return View("AssignToUser");
            }

            try
            {
                await _planService.AssignPlanToClient(planId, clientId, GetAdminId());
                TempData["Success"] = "Plan assigned successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.PlanId = planId;
                var plan = await _planService.GetPlanById(planId, GetAdminId());
                ViewBag.PlanName = plan.Name;
                ViewBag.PlanType = plan.WorkoutPlanType.Name;
                ViewBag.WorkoutCount = plan.WorkoutPlanWorkouts.Count;
                return View("AssignToUser");
            }
        }
    }
}

