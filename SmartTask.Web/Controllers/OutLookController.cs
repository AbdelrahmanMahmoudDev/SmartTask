using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using SmartTask.BL.Services;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using System.Security.Claims;

namespace SmartTask.Web.Controllers
{
    public class OutLookController : Controller
    {
        IConfiguration _config;
        private readonly IEventRepository eventRepository;
        public OutLookController(IConfiguration config,IEventRepository eventRepository)
        {
            _config = config;
            this.eventRepository = eventRepository;
        }
        [Authorize]
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


        [Authorize]
        public async Task<IActionResult> Cal(int size=100,int page=0)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allEvents = new List<Microsoft.Graph.Models.Event>();
            var s = (await eventRepository.GetByImportedByIdAsync(userId)).Skip(page*size).Take(size).ToList();
            return View(s);
        }
        [Authorize]
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
                var b = new Event()
                {
                    OutLooktTaskId = a.ICalUId,

                    Start = DateTime.Parse(a.Start.DateTime),

                    End = DateTime.Parse(a.End.DateTime),

                    Subject = a.Subject,

                    Attendees = string.Join(", ", a.Attendees.Select(att => att.EmailAddress?.Address)),

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
    }
}
