using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using SmartTask.BL.Services;

namespace SmartTask.Web.Controllers
{
    public class OutLookController : Controller
    {
        IConfiguration _config;
        public OutLookController(IConfiguration config)
        {
            _config = config;   
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

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("access_token")) &&
                string.IsNullOrEmpty(HttpContext.Session.GetString("refresh_token")) &&
                string.IsNullOrEmpty(HttpContext.Session.GetString("expires_at")))
            {
                HttpContext.Session.SetString("access_token", accessToken);
                HttpContext.Session.SetString("refresh_token", refreshToken);
                HttpContext.Session.SetString("expires_at", expiresAt);
            }


            return RedirectToAction("cal");
        }


        [Authorize]
        public async Task<IActionResult> cal()
        {
            var accessToken = HttpContext.Session.GetString("access_token");
            var refreshToken = HttpContext.Session.GetString("refresh_token");
            var expiresAtStr = HttpContext.Session.GetString("expires_at");

            if (DateTime.TryParse(expiresAtStr, out var expiresAt))
            {
                if (DateTime.UtcNow > expiresAt)
                {
                    accessToken = await RefreshAccessTokenAsync(refreshToken);
                    HttpContext.Session.SetString("access_token", accessToken);
                    HttpContext.Session.SetString("expires_at", DateTime.UtcNow.AddMinutes(60).ToString());
                }
            }

            // Ensure the required NuGet packages are installed:  
            // 1. Microsoft.Graph  
            // 2. Microsoft.Graph.Auth  
            // 3. Microsoft.Identity.Client  

            var authProvider = new MyAccessTokenProvider(accessToken);

            // Create the GraphServiceClient using your custom provider
            var graphClient = new GraphServiceClient(authProvider);


            // Get the current date/time in ISO 8601 format (required by Microsoft Graph)
            var todayIso = DateTime.UtcNow.ToString("o"); // "o" = round-trip format like 2025-05-15T14:00:00.000Z
            
            // Get the events from the user's calendar
            var events = await graphClient.Me.Events.GetAsync(opt =>
            {

                //opt.QueryParameters.Top = 10;
                //opt.QueryParameters.Select = new[] { "subject", "start", "end" };
                opt.QueryParameters.Filter = $"start/dateTime ge '{todayIso}'";
                opt.QueryParameters.Orderby = new[] { "start/dateTime" };
            });

            return View(events.Value);
        }

        private async Task<string> RefreshAccessTokenAsync(string refreshToken)
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

            return tokenResult.access_token;
        }
    }
}
