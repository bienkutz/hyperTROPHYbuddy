using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace hyperTROPHYbuddy.Controllers.Admin
{
    public class PlansController : Controller
    {
        private readonly IPlanService _planService;
        private readonly IWorkoutService _workoutService;

        public PlansController(IPlanService planService, IWorkoutService workoutService)
        {
            _planService = planService;
            _workoutService = workoutService;
        }

        // GET: Admin/Plans
        public async Task<IActionResult> Index()
        {
            var adminId = "hardcoded-admin-id";
            var plans = await _planService.GetPlansByAdmin(adminId);
            return View(plans);
        }

        // GET: Admin/Plans/Create
        public async Task<IActionResult> Create()
        {
            var adminId = "hardcoded-admin-id";
            var workouts = await _workoutService.GetWorkoutsByAdmin(adminId);
            var planTypes = await _planService.GetPlanTypes();

            ViewBag.Workouts = new MultiSelectList(workouts, "Id", "Name");
            ViewBag.PlanTypes = new SelectList(planTypes, "Id", "Name");
            return View();
        }

        // POST: Admin/Plans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutPlan plan, List<int> SelectedWorkoutIds)
        {
            if (ModelState.IsValid)
            {
                plan.AdminId = "hardcoded-admin-id";
                await _planService.CreatePlan(plan, SelectedWorkoutIds);
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        // GET: Admin/Plans/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var adminId = "hardcoded-admin-id";
            var plan = await _planService.GetPlanById(id, adminId);
            if (plan == null)
            {
                return NotFound();
            }

            var workouts = await _workoutService.GetWorkoutsByAdmin(adminId);
            var selectedWorkouts = plan.WorkoutPlanWorkouts.Select(wpw => wpw.WorkoutId).ToList();
            var planTypes = await _planService.GetPlanTypes();

            ViewBag.Workouts = new MultiSelectList(workouts, "Id", "Name", selectedWorkouts);
            ViewBag.PlanTypes = new SelectList(planTypes, "Id", "Name", plan.WorkoutPlanTypeId);
            return View(plan);
        }

        // POST: Admin/Plans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkoutPlan plan, List<int> SelectedWorkoutIds)
        {
            if (id != plan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                plan.AdminId = "hardcoded-admin-id";
                await _planService.UpdatePlan(plan, SelectedWorkoutIds);
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        // GET: Admin/Plans/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var adminId = "hardcoded-admin-id";
            var plan = await _planService.GetPlanById(id, adminId);
            if (plan == null)
            {
                return NotFound();
            }
            return View(plan);
        }

        // POST: Admin/Plans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adminId = "hardcoded-admin-id";
            await _planService.DeletePlan(id, adminId);
            return RedirectToAction(nameof(Index));
        }
    }
}
