using Microsoft.EntityFrameworkCore;
using SmartTask.BL.IServices;
using SmartTask.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.BL.Services
{
    public class ProjectDeadlineExtendService : IProjectDeadlineExtendService
    {
        private readonly SmartTaskContext _context;
        public ProjectDeadlineExtendService(SmartTaskContext context)
        {
            _context = context;
        }

        public async Task ExtendProjectDeadlineToFitTasks(int project_id)
        {
            var AllTasks = await _context.Tasks.Where(x=>x.ProjectId == project_id).ToListAsync();

            if (!AllTasks.Any()) return;

            var maxEndDate = AllTasks.Max(x => x.EndDate);
            var curProject = await _context.Projects.FirstOrDefaultAsync(x => x.Id == project_id);

            if (curProject == null) return;
            if (curProject.EndDate >= maxEndDate) return;

            curProject.EndDate = maxEndDate;

            await _context.SaveChangesAsync();
        }
    }
}
