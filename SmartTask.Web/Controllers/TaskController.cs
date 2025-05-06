using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTask.Core.Models;
using SmartTask.DataAccess.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartTask.Web.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly SmartTaskContext _context;

        public TaskController(SmartTaskContext context)
        {
            _context = context;
        }

        // الأكشن الأول: هيجيب التاسكات الخاصة بيوزر معين في مشروع محدد
        public async Task<IActionResult> TasksForUserInProject(int projectId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tasks = await _context.Tasks
                .Where(t => t.ProjectId == projectId && t.AssignedToId == userId)
                .ToListAsync();
            return View("Tasks",tasks);
        }

        // الأكشن الثاني: هيجيب كل التاسكات الخاصة بمشروع معين
        public async Task<IActionResult> TasksForProject(int projectId)
        {
            var tasks = await _context.Tasks
                //.Where(t => t.ProjectId == projectId)
                .ToListAsync();

            return View("Tasks", tasks);
        }

        // الأكشن الثاني: هيجيب كل التاسكات الخاصة بيوزر معين
        public async Task<IActionResult> TasksForUser()
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tasks = await _context.Tasks
                .Where(t => t.AssignedToId == userId)
                .ToListAsync();

            return View("Tasks", tasks);
        }
        [HttpGet]
        public  int Depend(int taskid)
        {
            var num =  _context.TaskDependencies
                .Where(t => t.PredecessorId == taskid)
                .ToList().Count();

            return num;
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteTask(int taskid)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.Id == taskid);
            if (task.Description=="")
            {
                return BadRequest("task cann't deleted it is started");
            }
            var TaskDependencies = await _context.TaskDependencies
                .Where(t => t.PredecessorId == taskid|| t.SuccessorId==taskid)
                .ToListAsync();
            _context.TaskDependencies.RemoveRange(TaskDependencies);
            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return Ok();

            //< button onclick = "deleteTask(5)" > Delete Task </ button >

//< script src = "https://code.jquery.com/jquery-3.6.0.min.js" ></ script >
//< script >
//function deleteTask(taskId) {
//    $.ajax({
//                url: '/YourController/DeleteTask',
//        type: 'POST',
//        data: { taskid: taskId },
//        success: function() {
//                        alert('Task deleted successfully!');
//                        location.reload(); // Reload or update the UI
//                    },
//        error: function(xhr) {
//                        alert('Error: ' + xhr.responseText);
//                    }
//                });
//            }
//</ script >
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}