using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hyperTROPHYbuddy.Services
{
    public class WorkoutPlanAssignmentService : IWorkoutPlanAssignmentService
    {
        private readonly ApplicationDbContext _context;

        public WorkoutPlanAssignmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkoutPlanAssignment>> GetAllAsync()
        {
            return await _context.WorkoutPlanAssignments
                .Include(w => w.Admin)
                .Include(w => w.Client)
                .Include(w => w.WorkoutPlan)
                .ToListAsync();
        }

        public async Task<WorkoutPlanAssignment?> GetByIdAsync(int id)
        {
            return await _context.WorkoutPlanAssignments
                .Include(w => w.Admin)
                .Include(w => w.Client)
                .Include(w => w.WorkoutPlan)
                .FirstOrDefaultAsync(w => w.WorkoutPlanAssignmentId == id);
        }

        public async Task<IEnumerable<WorkoutPlanAssignment>> GetByClientIdAsync(string clientId)
        {
            return await _context.WorkoutPlanAssignments
                .Include(w => w.Admin)
                .Include(w => w.Client)
                .Include(w => w.WorkoutPlan)
                .Where(w => w.AssignedToClientId == clientId)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkoutPlanAssignment>> GetByAdminIdAsync(string adminId)
        {
            return await _context.WorkoutPlanAssignments
                .Include(w => w.Admin)
                .Include(w => w.Client)
                .Include(w => w.WorkoutPlan)
                .Where(w => w.AssignedByAdminId == adminId)
                .ToListAsync();
        }

        public async Task CreateAsync(WorkoutPlanAssignment assignment)
        {
            _context.WorkoutPlanAssignments.Add(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WorkoutPlanAssignment assignment)
        {
            _context.WorkoutPlanAssignments.Update(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.WorkoutPlanAssignments.FindAsync(id);
            if (entity != null)
            {
                _context.WorkoutPlanAssignments.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}