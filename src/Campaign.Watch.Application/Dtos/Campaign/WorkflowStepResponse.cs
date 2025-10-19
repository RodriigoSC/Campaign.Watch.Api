using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Campaign
{
    /// <summary>
    /// DTO para step de workflow com informações detalhadas
    /// </summary>
    public class WorkflowStepResponse
    {
        public string OriginalStepId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public long TotalUser { get; set; }
        public long TotalExecutionTime { get; set; }
        public string Error { get; set; }
        public string ChannelName { get; set; }
        public string MonitoringNotes { get; set; }
        public ChannelIntegrationDataResponse IntegrationData { get; set; }

        // Diagnóstico do step
        public StepHealthDto HealthInfo { get; set; }
    }
}
