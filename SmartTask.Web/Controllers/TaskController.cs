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

        public async Task<IActionResult> Loadnodes()
        {
            Dictionary<int, List<int>> graph = new Dictionary<int, List<int>>();
           await _context.TaskDependencies.ForEachAsync(t =>
            {
                if (graph.ContainsKey(t.SuccessorId))
                {
                    graph[t.SuccessorId].Add(t.PredecessorId);
                }
                else
                {
                    graph[t.SuccessorId] = new List<int>();
                    graph[t.SuccessorId].Add(t.PredecessorId);
                }
            });
            var visited = new HashSet<int>();
            DFS(22, graph, visited);

            var allNodes = _context.Tasks.Select(x=>x.Id);
            var notReachable = allNodes.Except(visited);
            graph = new Dictionary<int, List<int>>();
            await _context.TaskDependencies.ForEachAsync(t =>
            {
                if (graph.ContainsKey(t.PredecessorId))
                {
                    graph[t.PredecessorId].Add(t.SuccessorId);
                }
                else
                {
                    graph[t.PredecessorId] = new List<int>();
                    graph[t.PredecessorId].Add(t.SuccessorId);
                }
            });

            visited = new HashSet<int>();
            DFS(22, graph, visited);
            notReachable = notReachable.Except(visited);
            return Ok(notReachable);
        }


        static void DFS(int node, Dictionary<int, List<int>> graph, HashSet<int> visited)
        {
            if (visited.Contains(node))
                return;

            visited.Add(node);

            if (graph.ContainsKey(node))
            {
                foreach (var neighbor in graph[node])
                {
                    DFS(neighbor, graph, visited);
                }
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}