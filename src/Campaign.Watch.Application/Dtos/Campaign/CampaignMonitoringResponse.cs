using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Campaign
{
    /// <summary>
    /// DTO completo para resposta de campanha monitorada, alinhado com CampaignModel do worker
    /// </summary>
    public class CampaignMonitoringResponse
    {
        public string Id { get; set; }
        public string ClientName { get; set; }
        public string IdCampaign { get; set; }
        public long NumberId { get; set; }
        public string Name { get; set; }
        public string CampaignType { get; set; }
        public string Description { get; set; }
        public string ProjectId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string CampaignStatus { get; set; }
        public string MonitoringStatus { get; set; }
        public DateTime? NextExecutionMonitoring { get; set; }
        public DateTime? LastCheckMonitoring { get; set; }
        public MonitoringHealthStatusDto HealthStatus { get; set; }
        public SchedulerResponse Scheduler { get; set; }

        // Novos campos do worker
        public int TotalExecutionsProcessed { get; set; }
        public int ExecutionsWithErrors { get; set; }
        public DateTime? FirstMonitoringAt { get; set; }
        public string MonitoringNotes { get; set; }

        // Métricas calculadas
        public CampaignMetricsDto Metrics { get; set; }
    }
}
