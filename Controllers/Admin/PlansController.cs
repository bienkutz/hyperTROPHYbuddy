using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace hyperTROPHYbuddy.Controllers.Admin
{
    public class PlansController : BaseAdminController
    {
        private readonly IPlanService _planService;
        private readonly IWorkoutService _workoutService;

        public PlansController(
            IPlanService planService,
            IWorkoutService workoutService)
        {
            _planService = planService;
            _workoutService = workoutService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm)
        {
            try
            {
                var plans = string.IsNullOrEmpty(searchTerm)
                    ? await _planService.GetPlansByAdmin(GetAdminId())
                    : await _planService.SearchPlans(searchTerm, GetAdminId());
                return View(plans);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            try
            {
                var workouts = await _workoutService.GetWorkoutsByAdmin(GetAdminId());
                var planTypes = await _planService.GetPlanTypes();

                ViewBag.Workouts = new MultiSelectList(workouts, "Id", "Name");
                ViewBag.PlanTypes = new SelectList(planTypes, "Id", "Name");
                return View(new WorkoutPlan());
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutPlan plan, List<int> selectedWorkoutIds)
        {
            if (!ModelState.IsValid)
            {
                var workouts = await _workoutService.GetWorkoutsByAdmin(GetAdminId());
                var planTypes = await _planService.GetPlanTypes();
                ViewBag.Workouts = new MultiSelectList(workouts, "Id", "Name", selectedWorkoutIds);
                ViewBag.PlanTypes = new SelectList(planTypes, "Id", "Name");
                return View(plan);
            }

            try
            {
                await _planService.CreatePlan(plan, selectedWorkoutIds, GetAdminId());
                TempData["Success"] = "Workout plan created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var workouts = await _workoutService.GetWorkoutsByAdmin(GetAdminId());
                var planTypes = await _planService.GetPlanTypes();
                ViewBag.Workouts = new MultiSelectList(workouts, "Id", "Name", selectedWorkoutIds);
                ViewBag.PlanTypes = new SelectList(planTypes, "Id", "Name");
                return View(plan);
            }
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var plan = await _planService.GetPlanById(id, GetAdminId());
                var workouts = await _workoutService.GetWorkoutsByAdmin(GetAdminId());
                var selectedWorkouts = plan.WorkoutPlanWorkouts.Select(wpw => wpw.WorkoutId).ToList();
                var planTypes = await _planService.GetPlanTypes();

                ViewBag.Workouts = new MultiSelectList(workouts, "Id", "Name", selectedWorkouts);
                ViewBag.PlanTypes = new SelectList(planTypes, "Id", "Name", plan.WorkoutPlanTypeId);
                return View(plan);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkoutPlan plan, List<int> selectedWorkoutIds)
        {
            if (id != plan.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                var workouts = await _workoutService.GetWorkoutsByAdmin(GetAdminId());
                var planTypes = await _planService.GetPlanTypes();
                ViewBag.Workouts = new MultiSelectList(workouts, "Id", "Name", selectedWorkoutIds);
                ViewBag.PlanTypes = new SelectList(planTypes, "Id", "Name");
                return View(plan);
            }

            try
            {
                await _planService.UpdatePlan(plan, selectedWorkoutIds, GetAdminId());
                TempData["Success"] = "Workout plan updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var workouts = await _workoutService.GetWorkoutsByAdmin(GetAdminId());
                var planTypes = await _planService.GetPlanTypes();
                ViewBag.Workouts = new MultiSelectList(workouts, "Id", "Name", selectedWorkoutIds);
                ViewBag.PlanTypes = new SelectList(planTypes, "Id", "Name");
                return View(plan);
            }
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var plan = await _planService.GetPlanById(id, GetAdminId());
                return View(plan);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _planService.DeletePlan(id, GetAdminId());
                TempData["Success"] = "Workout plan deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

