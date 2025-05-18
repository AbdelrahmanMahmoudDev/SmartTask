using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using SmartTask.BL.IServices;
using SmartTask.BL.Services;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.DataAccess.Repositories;
using SmartTask.Web.ViewModels;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartTask.Web.Controllers
{
    public class OutLookController : Controller
    {
        IConfiguration _config;
        private readonly IEventRepository eventRepository;
        private readonly IProjectRepository projectRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IAssignTaskRepository _assignTaskRepository;

        public OutLookController(IConfiguration config,IEventRepository eventRepository
            ,IProjectRepository project,ITaskRepository taskRepository
            ,IAssignTaskRepository assignTaskRepository)
        {
            _config = config;
            this.eventRepository = eventRepository;
            projectRepository = project;
            _taskRepository=taskRepository;
            _assignTaskRepository=assignTaskRepository;
        }
        //[Authorize]
        public IActionResult Index()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("OutlookCallback")
            };

            // Add `prompt=login` to force the user to sign in again
            props.Items["LoginHint"] = ""; // optional
            props.Items["prompt"] = "login";

            return Challenge(props, "Outlook");
        }

        public async Task<IActionResult> OutlookCallback()
        {
            var accessToken = await HttpContext.GetTokenAsync("Outlook", "access_token");
            var refreshToken = await HttpContext.GetTokenAsync("Outlook", "refresh_token");
            var expiresAt = await HttpContext.GetTokenAsync("Outlook", "expires_at");

            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("access_token")) &&
            //    string.IsNullOrEmpty(HttpContext.Session.GetString("refresh_token")) &&
            //    string.IsNullOrEmpty(HttpContext.Session.GetString("expires_at")))
            {
                HttpContext.Session.SetString("access_token", accessToken);
                HttpContext.Session.SetString("refresh_token", refreshToken);
                HttpContext.Session.SetString("expires_at", expiresAt);
            }


            return RedirectToAction("Cal");
        }


        //[Authorize]
        public async Task<IActionResult> Cal(int size=100,int page=0)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allEvents = new List<Microsoft.Graph.Models.Event>();
            var s = (await eventRepository.GetByImportedByIdAsync(userId)).Skip(page*size).Take(size).ToList();
            return View(s);
        }
        //[Authorize]
        public async Task<IActionResult> syncoutlook()
        {
            var accessToken = HttpContext.Session.GetString("access_token");
            var refreshToken = HttpContext.Session.GetString("refresh_token");
            var expiresAtStr = HttpContext.Session.GetString("expires_at");
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("access_token")) ||
                string.IsNullOrEmpty(HttpContext.Session.GetString("refresh_token")))
            {
                return RedirectToAction("Index");
            }
            if (DateTime.TryParse(expiresAtStr, out var expiresAt))
            {
                if (DateTime.UtcNow > expiresAt)
                {
                    var a = await RefreshAccessTokenAsync(refreshToken);
                    HttpContext.Session.SetString("access_token", a.access_token);
                    var at = DateTime.UtcNow.AddSeconds(a.expires_in);
                    HttpContext.Session.SetString("expires_at", at.ToString("o"));
                    HttpContext.Session.SetString("refresh_token", a.refresh_token);
                }
            }
            var authProvider = new MyAccessTokenProvider(accessToken);
            var graphClient = new GraphServiceClient(authProvider);

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allEvents = new List<Microsoft.Graph.Models.Event>();
            var s = (await eventRepository.GetByImportedByIdAsync(userId)).Select(x => x.OutLooktTaskId).ToList();
            var page = await graphClient.Me.Events.GetAsync(opt =>
            {
                opt.QueryParameters.Orderby = new[] { "start/dateTime" };
            });
            int i = 10;

            while (page != null)
            {
                allEvents.AddRange(page.Value.Where(x => !s.Contains(x.ICalUId)));
                if (page.OdataNextLink != null)
                {
                    page = await graphClient.Me.Events.GetAsync(requestConfig =>
                    {
                        requestConfig.QueryParameters.Skip = i;
                        requestConfig.QueryParameters
                        .Orderby = new[] { "start/dateTime" };
                    });
                    i += 10;
                }
                else
                {
                    break;
                }
            }
            foreach (var a in allEvents)
            {
                var b = new Core.Models.Event()
                {
                    OutLooktTaskId = a.ICalUId,

                    Start = DateTime.Parse(a.Start.DateTime),

                    End = DateTime.Parse(a.End.DateTime),

                    Subject = a.Subject,

                    Attendees = Regex.Replace(a.Body.Content, "<.*?>", string.Empty)+"\n "+ string.Join(", ", a.Attendees.Select(att => att.EmailAddress?.Address)),

                    ImportedById = userId

                };
                await eventRepository.AddAsync(b);
            }
            return RedirectToAction("Cal");
        }

        private async Task<OAuthResponse> RefreshAccessTokenAsync(string refreshToken)
        {
            var http = new HttpClient();
            var parameters = new Dictionary<string, string>
        {
            { "client_id", _config["AzureAd:ClientId"] },
            { "client_secret", _config["AzureAd:ClientSecret"] },
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken },
            { "scope", "https://graph.microsoft.com/.default offline_access" }
        };

            var res = await http.PostAsync($"https://login.microsoftonline.com/{_config["AzureAd:TenantId"]}/oauth2/v2.0/token", new FormUrlEncodedContent(parameters));
            var tokenResult = await res.Content.ReadFromJsonAsync<OAuthResponse>();

            return tokenResult;
        }
        public async Task<IActionResult> LoadAddTaskPartial(int eventId)
        {
            var projects = await projectRepository.GetAllAsyncWithoutInclude();
            //var theEvent = await eventRepository.GetByIdAsync(eventId);
            var model = new AddEventAsTaskViewModel
            {
                EventId = eventId,
                //Start=theEvent.Start,
                //End=theEvent.End,
                Projects = projects.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList()
            };
            return PartialView("_AddTaskPartial", model);
        }
        [HttpGet]
        public async Task<IActionResult> GetUsersForProject(int projectId)
        {
            var users = projectRepository.GetMembers(projectId);
            return Json(users.Select(u => new { u.Id, u.FullName }));
        }

        public async Task<JsonResult> ValidateStartDate(DateTime Start, int ProjectId)
        {
            var project = await projectRepository.GetByIdAsync(ProjectId);

            if (project == null)
                return Json("Invalid project.");

            if (Start < project.StartDate || Start > project.EndDate)
                return Json($"Start must be between {project.StartDate:d} and {project.EndDate:d}");

            return Json(true);
        }

        public async Task<JsonResult> ValidateEndDate(DateTime End, DateTime Start, int ProjectId)
        {
            var project = await projectRepository.GetByIdAsync(ProjectId);

            if (project == null)
                return Json("Invalid project.");

            if (End < project.StartDate || End > project.EndDate)
                return Json($"End must be between {project.StartDate:d} and {project.EndDate:d}");

            if (End < Start)
                return Json("End date must be after start date.");

            return Json(true);
        }

        [HttpPost]
        public async Task<IActionResult> AddTaskFromEvent(AddEventAsTaskViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var allErrors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

                string errorMessage = string.Join(" | ", allErrors);
                return BadRequest(errorMessage);
            }
            var ev = await eventRepository.GetByIdAsync(model.EventId);
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Core.Models.Task task = new Core.Models.Task
            {
                Title = ev.Subject,
                Description = ev.Attendees,
                StartDate = model.Start,
                EndDate = model.End,
                Status = Core.Models.Enums.Status.Todo,
                Priority = model.Priority,
                CreatedById = userId,
                CreatedAt = DateTime.Now,
                ProjectId = model.ProjectId.Value
            };
            await _taskRepository.AddAsync(task);
            await _assignTaskRepository.AssignTasksToUserByIds(model.UserIds, task, User);
            ev.CreatedAt = DateTime.Now;
            ev.TaskId = task.Id;
            await eventRepository.UpdateAsync(ev);
            var projectname = (await projectRepository.GetByIdAsync(model.ProjectId.Value)).Name;
            Console.WriteLine(model.EventId+" "+model.ProjectId+" "+model.UserIds.ToString()+" "+model.Start);
            return Ok(new { projectName = projectname, eventId = model.EventId });
        }

    }
}
