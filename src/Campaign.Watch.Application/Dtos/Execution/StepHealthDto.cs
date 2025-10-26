using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Execution
{
    /// <summary>
    /// Informações de saúde do step
    /// </summary>
    public class StepHealthDto
    {
        public string Severity { get; set; }
        public string DiagnosticType { get; set; }
        public string Message { get; set; }
        public DateTime? DetectedAt { get; set; }
        public Dictionary<string, object> AdditionalData { get; set; }
    }
}
