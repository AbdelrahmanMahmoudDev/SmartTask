namespace SmartTask.Api.DTOs
{
    public class SaveSelectedTasksDTO
    {
        public int SelectedTaskId { get; set; }
        public List<int> SelectedTaskIds { get; set; } = new();
    }
}