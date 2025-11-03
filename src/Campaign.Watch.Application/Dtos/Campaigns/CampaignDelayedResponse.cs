using System;

namespace Campaign.Watch.Application.Dtos.Campaign
{
    /// <summary>
    /// DTO para campanhas com execução atrasada.
    /// Foca em informações de agendamento.
    /// </summary>
    public class CampaignDelayedResponse
    {
        public string Id { get; set; }
        public string ClientName { get; set; }
        public long NumberId { get; set; }
        public string Name { get; set; }
        public string MonitoringStatus { get; set; }
        public DateTime? NextExecutionMonitoring { get; set; }
        public DateTime StartDateTime { get; set; }
    }
}
