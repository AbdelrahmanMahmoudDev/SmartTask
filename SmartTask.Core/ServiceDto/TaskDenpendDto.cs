using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.Core.Models.ServiceDto
{
    public class TaskDenpendDto
    {
        public int TaskId { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
