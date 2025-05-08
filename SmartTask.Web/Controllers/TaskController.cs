using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTask.Core.Models;
using SmartTask.Core.ViewModels;
using SmartTask.DataAccess.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartTask.Web.Controllers
{
    //[Authorize]
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
        public IActionResult Details(int id)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            return PartialView("_DetailsPartial",task);
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

        }

          
        public async Task<IActionResult> Loadnodes(int id)
        {
            Dictionary<int, List<int>> graph = new Dictionary<int, List<int>>();

            var visited = new HashSet<int>();
            var allNodes = _context.Tasks;

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

            DFS(id, graph, visited);
            var notReachable = allNodes.ToList().ExceptBy(visited, e => e.Id);
            var a = _context.TaskDependencies.Where(x => x.SuccessorId == id).Select(e => e.PredecessorId).ToList();
            List<TaskDendenciesViewModel> taskviewdep = new List<TaskDendenciesViewModel>();
            foreach (var n in notReachable)
            {
                if (a.Contains(n.Id))
                {
                    taskviewdep.Add(new TaskDendenciesViewModel { Id = n.Id, Name = n.Title, IsSelected = true });
                }
                else
                {
                    taskviewdep.Add(new TaskDendenciesViewModel { Id = n.Id, Name = n.Title, IsSelected = false });
                }
            }
            return PartialView("_TaskDend", taskviewdep);
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
        [HttpPost]
        public IActionResult SaveSelectedTasks(int SelectedTaskId, List<int> selectedTaskIds)
        {
            var taskDependencies = _context.TaskDependencies
                                   .Where(td => td.SuccessorId == SelectedTaskId) 
                                   .ToList();
            foreach (var selectedTaskId in selectedTaskIds)
            {
                var existingDependency = taskDependencies
                    .FirstOrDefault(td => td.PredecessorId == selectedTaskId);

                if (existingDependency == null)
                {
                    _context.TaskDependencies.Add(new TaskDependency
                    {
                        SuccessorId = SelectedTaskId, 
                        PredecessorId = selectedTaskId 
                    });
                }
            }

            foreach (var taskDependency in taskDependencies)
            {
                if (!selectedTaskIds.Contains(taskDependency.PredecessorId))
                {
                    _context.TaskDependencies.Remove(taskDependency); 
                }
            }

            _context.SaveChanges();
            return Json(new { success = true, selected = selectedTaskIds.ToString() });
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}