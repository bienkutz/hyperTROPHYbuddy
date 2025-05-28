using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hyperTROPHYbuddy.Controllers
{
    public class WorkoutPlansController : Controller
    {
        private readonly IWorkoutPlanService _workoutPlanService;
        private readonly IWorkoutPlanWorkoutService _workoutPlanWorkoutService;
        private readonly IWorkoutService _workoutService;
        private readonly UserManager<ApplicationUser> _userManager;

        public WorkoutPlansController(IWorkoutPlanService workoutPlanService,
                                      IWorkoutPlanWorkoutService workoutPlanWorkoutService,
                                      IWorkoutService workoutService,
                                      UserManager<ApplicationUser> userManager)
        {
            _workoutPlanService = workoutPlanService;
            _workoutPlanWorkoutService = workoutPlanWorkoutService;
            _workoutService = workoutService;
            _userManager = userManager;
        }

        // GET: WorkoutPlans
        public async Task<IActionResult> Index(WorkoutPlanType? type)
        {
            var allPlans = await _workoutPlanService.GetAllAsync();
            if (type.HasValue)
            {
                allPlans = allPlans.Where(p => p.Type == type.Value);
            }

            // Pass available types for dropdown
            ViewBag.Types = Enum.GetValues(typeof(WorkoutPlanType))
                                .Cast<WorkoutPlanType>()
                                .Select(t => new SelectListItem
                                {
                                    Value = ((int)t).ToString(),
                                    Text = t.ToString(),
                                    Selected = type.HasValue && t == type.Value
                                }).ToList();

            ViewBag.SelectedType = type;
            return View(allPlans);
        }

        // GET: WorkoutPlans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutPlan = await _workoutPlanService.GetByIdAsync(id.Value);
            if (workoutPlan == null)
            {
                return NotFound();
            }

            return View(workoutPlan);
        }

        // GET: WorkoutPlans/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var allWorkouts = await _workoutService.GetAllAsync();
            var userWorkouts = allWorkouts
                .Where(w => w.CreatedByAdminId == currentUser.Id)
                .Select(w => new { workoutId = w.WorkoutId, name = w.Name })
                .ToList();

            ViewBag.Workouts = userWorkouts;
            return View();
        }

        // POST: WorkoutPlans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutPlan workoutPlan, int[] WorkoutIds, int[] Days)
        {
            if (ModelState.IsValid)
            {
                // Save the plan and wait for the ID to be generated
                await _workoutPlanService.CreateAsync(workoutPlan);

                // Add each selected workout with its day
                for (int i = 0; i < WorkoutIds.Length; i++)
                {
                    var planWorkout = new WorkoutPlanWorkout
                    {
                        WorkoutPlanId = workoutPlan.WorkoutPlanId,
                        WorkoutId = WorkoutIds[i],
                        Day = Days[i]
                    };
                    await _workoutPlanWorkoutService.CreateAsync(planWorkout);
                }

                return RedirectToAction(nameof(Index));
            }
            var allWorkouts = await _workoutService.GetAllAsync();
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.Workouts = allWorkouts.Where(w => w.CreatedByAdminId == currentUser.Id)
                                          .Select(w => new { workoutId = w.WorkoutId, name = w.Name })
                                          .ToList();
            return View(workoutPlan);
        }

        // GET: WorkoutPlans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var workoutPlan = await _workoutPlanService.GetByIdAsync(id.Value);
            if (workoutPlan == null)
                return NotFound();

            // Get all workouts created by this admin
            var currentUser = await _userManager.GetUserAsync(User);
            var allWorkouts = (await _workoutService.GetAllAsync())
                .Where(w => w.CreatedByAdminId == currentUser.Id)
                .ToList();

            // Get selected workout IDs for this plan
            var selectedWorkoutIds = (await _workoutPlanWorkoutService.GetByPlanIdAsync(workoutPlan.WorkoutPlanId))
                .Select(pw => pw.WorkoutId)
                .ToList();

            // Get days for selected workouts
            var workoutDays = (await _workoutPlanWorkoutService.GetByPlanIdAsync(workoutPlan.WorkoutPlanId))
                .ToDictionary(pw => pw.WorkoutId, pw => pw.Day);

            ViewBag.Workouts = allWorkouts;
            ViewBag.SelectedWorkoutIds = selectedWorkoutIds;
            ViewBag.WorkoutDays = workoutDays;

            return View(workoutPlan);
        }

        // POST: WorkoutPlans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkoutPlanId,Name,Description,Type")] WorkoutPlan workoutPlan, int[] SelectedWorkoutIds, int[] Days)
        {
            if (id != workoutPlan.WorkoutPlanId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _workoutPlanService.UpdateAsync(workoutPlan);

                    // Remove all existing workouts for this plan
                    await _workoutPlanWorkoutService.DeleteAllForPlanAsync(workoutPlan.WorkoutPlanId);

                    // Add selected workouts with their days
                    if (SelectedWorkoutIds != null && SelectedWorkoutIds.Length > 0)
                    {
                        for (int i = 0; i < SelectedWorkoutIds.Length; i++)
                        {
                            var planWorkout = new WorkoutPlanWorkout
                            {
                                WorkoutPlanId = workoutPlan.WorkoutPlanId,
                                WorkoutId = SelectedWorkoutIds[i],
                                Day = Days[i]
                            };
                            await _workoutPlanWorkoutService.CreateAsync(planWorkout);
                        }
                    }
                }
                catch
                {
                    if (!WorkoutPlanExists(workoutPlan.WorkoutPlanId))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Repopulate workouts if model state is invalid
            var currentUser = await _userManager.GetUserAsync(User);
            var allWorkouts = (await _workoutService.GetAllAsync())
                .Where(w => w.CreatedByAdminId == currentUser.Id)
                .ToList();
            ViewBag.Workouts = allWorkouts;
            ViewBag.SelectedWorkoutIds = SelectedWorkoutIds?.ToList() ?? new List<int>();
            ViewBag.WorkoutDays = new Dictionary<int, int>();
            if (SelectedWorkoutIds != null && Days != null)
            {
                for (int i = 0; i < SelectedWorkoutIds.Length && i < Days.Length; i++)
                    ViewBag.WorkoutDays[SelectedWorkoutIds[i]] = Days[i];
            }
            return View(workoutPlan);
        }

        // GET: WorkoutPlans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutPlan = await _workoutPlanService.GetByIdAsync(id.Value);
            if (workoutPlan == null)
            {
                return NotFound();
            }

            return View(workoutPlan);
        }

        // POST: WorkoutPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workoutPlan = await _workoutPlanService.GetByIdAsync(id);
            if (workoutPlan != null)
            {
                _workoutPlanService.DeleteAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool WorkoutPlanExists(int id)
        {
            return _workoutPlanService.GetByIdAsync(id).Result != null;
        }
    }
}
