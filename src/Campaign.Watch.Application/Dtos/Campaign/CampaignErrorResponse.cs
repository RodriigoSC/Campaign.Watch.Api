using System;

namespace Campaign.Watch.Application.Dtos.Campaign
{
    /// <summary>
    /// DTO para campanhas com erro de integração.
    /// Foca em identificar a campanha e o erro.
    /// </summary>
    public class CampaignErrorResponse
    {
        public string Id { get; set; }
        public string ClientName { get; set; }
        public long NumberId { get; set; }
        public string Name { get; set; }
        public string LastMessage { get; set; }
        public string LastExecutionWithIssueId { get; set; }
        public DateTime? LastCheckMonitoring { get; set; }
    }
}
