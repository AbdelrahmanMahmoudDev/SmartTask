namespace SmartTask.Web.Models
{
    public class JqueryDatatableParam
    {
        // The draw counter that this object is a response to - from the draw parameter sent as part of the data request
        public int draw { get; set; }

        // Global search value
        public string search { get; set; }

        // Global search regex status
        public bool regex { get; set; }

        // Number of records that the table can display in the current draw
        public int length { get; set; }

        // First record that should be displayed (start point)
        public int start { get; set; }

        // Number of columns being displayed
        public int columns { get; set; }

        // Column to which ordering should be applied
        public int order_column { get; set; }

        // Direction to be ordered - "asc" or "desc"
        public string order_dir { get; set; }
    }
}
