using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTask.Core.Models;
using SmartTask.DataAccess.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTask.Web.Controllers
{
    public class TaskController : Controller
    {
        private readonly SmartTaskContext _context;

        public TaskController(SmartTaskContext context)
        {
            _context = context;
        }

        // الأكشن الأول: هيجيب التاسكات الخاصة بيوزر معين في مشروع محدد
        public async Task<IActionResult> TasksForUserInProject(int projectId, string userId)
        {
            var tasks = await _context.Tasks
                .Where(t => t.ProjectId == projectId && t.AssignedToId == userId)
                .ToListAsync();
            return View("Tasks",tasks);
        }

        // الأكشن الثاني: هيجيب كل التاسكات الخاصة بمشروع معين
        public async Task<IActionResult> TasksForProject(int projectId)
        {
            var tasks = await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();

            return View("Tasks", tasks);
        }

        // الأكشن الثاني: هيجيب كل التاسكات الخاصة بيوزر معين
        public async Task<IActionResult> TasksForUser(string userid)
        {
            var tasks = await _context.Tasks
                .Where(t => t.AssignedToId == userid)
                .ToListAsync();

            return View("Tasks", tasks);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}