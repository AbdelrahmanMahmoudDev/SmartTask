using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTask.Bl.IServices;

namespace SmartTask.Web.Controllers
{
    public class BranchController : Controller
    {
        private readonly IBranchService branchService;

        public BranchController(IBranchService branchService)
        {
            this.branchService = branchService;
        }
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var branches = branchService.GetAllBranchAsync(page, pageSize);
            return View(branches);
        }


        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public IActionResult Branch()
        {

            return View();
        }


        [HttpPost]
        public IActionResult AddBranch()
        {
            return RedirectToAction("Index");
        }
    }
}
