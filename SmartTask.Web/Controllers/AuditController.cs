using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.Web.ViewModels;

namespace SmartTask.Web.Controllers
{
    public class AuditController : Controller
    {
        private readonly IAuditRepository _auditRepository;
        private readonly IUserLoginHistoryRepository _userLoginHistory;

        private readonly UserManager<ApplicationUser> _userManager;

        public AuditController(IAuditRepository auditRepository,IUserLoginHistoryRepository userLoginHistory,UserManager<ApplicationUser> userManager)
        {
            _auditRepository = auditRepository;
            _userLoginHistory = userLoginHistory;
           
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Users = await _userManager.Users.ToListAsync();
            var audits= _auditRepository.GetAllAudits();
            List<AuditViewModel> auditViewModels = new List<AuditViewModel>();
            foreach (var audit in audits)
            {
                AuditViewModel auditViewModel = new AuditViewModel
                {
                    Id = audit.Id,
                    TableName = audit.TableName,
                    Action = audit.Action,
                    Username = audit.Username,
                    Timestamp = audit.Timestamp,
                    OldValues=audit.OldValues,
                    NewValues = audit.NewValues,
                    AffectedColumns = audit.AffectedColumns,
                    UserId = audit.UserId

                };
                auditViewModels.Add(auditViewModel);
            }

            return View(auditViewModels);
        }
        public IActionResult FindAuditByUserId(string id)
        {
            var audits = _auditRepository.GetAuditsByUserId(id);
            List<AuditViewModel> auditViewModels = audits.Select(audit => new AuditViewModel
            {
                Id = audit.Id,
                TableName = audit.TableName,
                Action = audit.Action,
                Username = audit.Username,
                Timestamp = audit.Timestamp,
                OldValues = audit.OldValues,
                NewValues = audit.NewValues,
                AffectedColumns = audit.AffectedColumns,
                UserId = audit.UserId
            }).ToList();

            return PartialView("FindAuditByUserId",auditViewModels);
        }
        public IActionResult UserLog()
        {
            var users = _userLoginHistory.GetAllUserLoginHistories();
            List<UserLoginHistoryViewModel> userLoginHistoryViewModels = new List<UserLoginHistoryViewModel>();
            foreach (var user in users)
            {
                UserLoginHistoryViewModel userLoginHistoryViewModel = new UserLoginHistoryViewModel
                {
                    Id = user.Id,
                    UserId = user.UserId,
                    UserName = user.UserName,
                    LoginTime = user.LoginTime,
                    IPAddress = user.IPAddress,
                    UserAgent = user.UserAgent
                };
                userLoginHistoryViewModels.Add(userLoginHistoryViewModel);
            }
            return View(userLoginHistoryViewModels);
        }
    }
}
