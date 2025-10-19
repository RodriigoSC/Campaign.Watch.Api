using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Campaign
{
    /// <summary>
    /// DTO para resposta de execução separada
    /// </summary>
    public class ExecutionMonitoringResponse
    {
        public string Id { get; set; }
        public string CampaignMonitoringId { get; set; }
        public string OriginalCampaignId { get; set; }
        public string OriginalExecutionId { get; set; }
        public string CampaignName { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? TotalDurationInSeconds { get; set; }
        public bool HasMonitoringErrors { get; set; }
        public List<WorkflowStepResponse> Steps { get; set; }

        // Diagnóstico da execução
        public ExecutionHealthSummaryDto HealthSummary { get; set; }
    }
}
