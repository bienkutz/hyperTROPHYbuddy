using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services.Interfaces;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Identity;

namespace hyperTROPHYbuddy.Controllers
{
    public class WorkoutPlanAssignmentsController : Controller
    {
        private readonly IWorkoutPlanAssignmentService _assignmentService;
        private readonly IWorkoutPlanService _workoutPlanService;
        private readonly IApplicationUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public WorkoutPlanAssignmentsController(
            IWorkoutPlanAssignmentService assignmentService,
            IWorkoutPlanService workoutPlanService,
            IApplicationUserService userService,
            UserManager<ApplicationUser> userManager)
        {
            _assignmentService = assignmentService;
            _workoutPlanService = workoutPlanService;
            _userService = userService;
            _userManager = userManager;
        }

        // GET: WorkoutPlanAssignments
        public async Task<IActionResult> Index()
        {
            var assignments = await _assignmentService.GetAllAsync();
            var currentUser = await _userManager.GetUserAsync(User);
            assignments = assignments.Where(a => a.AssignedByAdminId == currentUser.Id);

            return View(assignments);
        }

        // GET: WorkoutPlanAssignments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _assignmentService.GetByIdAsync(id.Value);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // GET: WorkoutPlanAssignments/Create
        public async Task<IActionResult> Create()
        {
            var users = await _userService.GetAllAsync();
            var currentuser = await _userManager.GetUserAsync(User);

            var workoutPlans = await _workoutPlanService.GetAllAsync();
            workoutPlans = workoutPlans.Where(wp => wp.CreatedByAdminId == currentuser.Id);

            // Use UserName for display, Id for value
            ViewData["AssignedToClientId"] = new SelectList(users, "Id", "UserName");
            ViewData["WorkoutPlanId"] = new SelectList(workoutPlans, "WorkoutPlanId", "Description");
            return View();
        }

        // POST: WorkoutPlanAssignments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkoutPlanAssignmentId,WorkoutPlanId,AssignedToClientId")] WorkoutPlanAssignment workoutPlanAssignment)
        {
            if (ModelState.IsValid)
            {
                // Check if the client already has a plan assigned
                var existingAssignments = await _assignmentService.GetByClientIdAsync(workoutPlanAssignment.AssignedToClientId);
                if (existingAssignments.Any())
                {
                    // Notify admin and do not create a new assignment
                    ModelState.AddModelError(string.Empty, "This client already has a workout plan assigned. Please remove the old assignment before adding a new one.");
                }
                else
                {
                    // Set the current logged-in user as the admin
                    var currentUser = await _userManager.GetUserAsync(User);
                    workoutPlanAssignment.AssignedByAdminId = currentUser.Id;

                    await _assignmentService.CreateAsync(workoutPlanAssignment);
                    return RedirectToAction(nameof(Index));
                }
            }
            var users = await _userService.GetAllAsync();
            var workoutPlans = await _workoutPlanService.GetAllAsync();

            ViewData["AssignedToClientId"] = new SelectList(users, "Id", "UserName", workoutPlanAssignment.AssignedToClientId);
            ViewData["WorkoutPlanId"] = new SelectList(workoutPlans, "WorkoutPlanId", "Description", workoutPlanAssignment.WorkoutPlanId);
            return View(workoutPlanAssignment);
        }
        /*
        // GET: WorkoutPlanAssignments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _assignmentService.GetByIdAsync(id.Value);
            if (assignment == null)
            {
                return NotFound();
            }
            var users = await _userService.GetAllAsync();
            var workoutPlans = await _workoutPlanService.GetAllAsync();
            ViewData["AssignedByAdminId"] = new SelectList(users, "Id", "Id", assignment.AssignedByAdminId);
            ViewData["AssignedToClientId"] = new SelectList(users, "Id", "Id", assignment.AssignedToClientId);
            ViewData["WorkoutPlanId"] = new SelectList(workoutPlans, "WorkoutPlanId", "Description", assignment.WorkoutPlanId);
            return View(assignment);
        }

        // POST: WorkoutPlanAssignments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkoutPlanAssignmentId,WorkoutPlanId,AssignedToClientId,AssignedByAdminId")] WorkoutPlanAssignment workoutPlanAssignment)
        {
            if (id != workoutPlanAssignment.WorkoutPlanAssignmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _assignmentService.UpdateAsync(workoutPlanAssignment);
                }
                catch
                {
                    if (await _assignmentService.GetByIdAsync(workoutPlanAssignment.WorkoutPlanAssignmentId) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var users = await _userService.GetAllAsync();
            var workoutPlans = await _workoutPlanService.GetAllAsync();
            ViewData["AssignedByAdminId"] = new SelectList(users, "Id", "Id", workoutPlanAssignment.AssignedByAdminId);
            ViewData["AssignedToClientId"] = new SelectList(users, "Id", "Id", workoutPlanAssignment.AssignedToClientId);
            ViewData["WorkoutPlanId"] = new SelectList(workoutPlans, "WorkoutPlanId", "Description", workoutPlanAssignment.WorkoutPlanId);
            return View(workoutPlanAssignment);
        }
        */
        // GET: WorkoutPlanAssignments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _assignmentService.GetByIdAsync(id.Value);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // POST: WorkoutPlanAssignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _assignmentService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: WorkoutPlanAssignments/MyPlan
        public async Task<IActionResult> MyPlan()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            // Get the latest assignment for this client
            var assignments = await _assignmentService.GetByClientIdAsync(user.Id);
            var assignment = assignments.OrderByDescending(a => a.WorkoutPlanAssignmentId).FirstOrDefault();

            if (assignment == null || assignment.WorkoutPlanId == null)
            {
                // Pass a flag to the Details view
                ViewBag.NoPlanAssigned = true;
                return View("~/Views/WorkoutPlans/Details.cshtml", null);
            }

            // Check if the user has been notified about this assignment
            if (user.LastNotifiedWorkoutPlanAssignmentId != assignment.WorkoutPlanAssignmentId)
            {
                ViewBag.ShowPlanAssignedNotification = true;
                // Update the user so they won't see the notification again until a new assignment
                user.LastNotifiedWorkoutPlanAssignmentId = assignment.WorkoutPlanAssignmentId;
                await _userManager.UpdateAsync(user);
            }
            else
            {
                ViewBag.ShowPlanAssignedNotification = false;
            }

            // Eagerly load the plan with its workouts and exercises
            var plan = await _workoutPlanService.GetByIdAsync(assignment.WorkoutPlanId.Value);

            ViewBag.AdminUserName = assignment.Admin?.UserName ?? "Unknown";
            ViewBag.NoPlanAssigned = false; // Ensure this is set

            return View("~/Views/WorkoutPlans/Details.cshtml", plan);
        
        
    }
    }
}
