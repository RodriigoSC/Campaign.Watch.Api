using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Execution
{
    /// <summary>
    /// Resumo de saúde da execução
    /// </summary>
    public class ExecutionHealthSummaryDto
    {
        public string OverallHealth { get; set; }
        public int TotalSteps { get; set; }
        public int HealthySteps { get; set; }
        public int StepsWithWarnings { get; set; }
        public int StepsWithErrors { get; set; }
        public int CriticalSteps { get; set; }
        public List<string> MainIssues { get; set; }
    }
}
