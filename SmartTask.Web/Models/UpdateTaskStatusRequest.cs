namespace SmartTask.Web.Models
{
    public class UpdateTaskStatusRequest
    {
        public int TaskId { get; set; }
        public int NewStatus { get; set; }
    }
}
