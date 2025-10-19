using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Campaign
{
    /// <summary>
    /// Métricas calculadas da campanha
    /// </summary>
    public class CampaignMetricsDto
    {
        public int TotalExecutions { get; set; }
        public int CompletedExecutions { get; set; }
        public int FailedExecutions { get; set; }
        public int InProgressExecutions { get; set; }
        public double SuccessRate { get; set; }
        public TimeSpan? AverageExecutionTime { get; set; }
        public DateTime? LastExecutionDate { get; set; }
        public DateTime? NextScheduledExecution { get; set; }
    }
}
