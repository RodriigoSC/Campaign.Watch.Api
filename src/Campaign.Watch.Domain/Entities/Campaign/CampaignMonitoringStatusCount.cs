using Campaign.Watch.Domain.Enums;

namespace Campaign.Watch.Domain.Entities.Campaign
{
    /// <summary>
    /// Modelo auxiliar para armazenar o resultado da agregação de contagem de status monitoramento.
    /// </summary>
    public class CampaignMonitoringStatusCount
    {
        public MonitoringStatus Status { get; set; }
        public int Count { get; set; }
    }
}
