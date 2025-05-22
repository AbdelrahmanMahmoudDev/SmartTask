namespace SmartTask.Web.ViewModels.BranchVM
{
    public class AssignUsersToBranchViewModel
    {
        public int BranchId { get; set; }
        public List<string> SelectedUserIds { get; set; } = new List<string>();
    }
}
