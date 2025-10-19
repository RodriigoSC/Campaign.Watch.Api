using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Campaign
{
    /// <summary>
    /// Problema diagnosticado
    /// </summary>
    public class DiagnosticIssueDto
    {
        public string Type { get; set; }
        public string Severity { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime DetectedAt { get; set; }
    }
}
