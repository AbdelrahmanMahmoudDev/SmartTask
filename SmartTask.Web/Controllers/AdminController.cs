using Microsoft.AspNetCore.Mvc;
using SmartTask.Core.IRepositories;
using SmartTask.DataAccess.Repositories;

namespace SmartTask.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserRepository _userRepository;

        public AdminController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public IActionResult Index(string id)
        {
          
        var User =_userRepository.GetByIdAsync(id);
            if (User == null)
            {
                return NotFound();
            }
            return View(User);
        }
    }
}
