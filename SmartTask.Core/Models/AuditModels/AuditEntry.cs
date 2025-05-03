

namespace SmartTask.Core.Models.AuditModels
{
    public class AuditEntry
    {
       
        public string UserId { get; set; }
        public string TableName { get; set; }
        public AuditAction Action { get; set; }
        public string Username { get; set; }
        public DateTime Timestamp { get; set; }=DateTime.UtcNow;
        public Dictionary<string, object> OldValues { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; set; } = new Dictionary<string, object>();
        public List<string> AffectedColumns { get; set; } = new List<string>();
        public Audit ToAudit()
        {
            Audit audit = new Audit();
            audit.UserId = UserId;
            audit.TableName = TableName;
            audit.Action = Action.ToString();
            audit.Username = Username;
            audit.Timestamp = Timestamp;
            audit.OldValues = string.Join(", ", OldValues.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
            audit.NewValues = string.Join(", ", NewValues.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
            audit.AffectedColumns = string.Join(", ", AffectedColumns);
            return audit;
        }
    }

}
