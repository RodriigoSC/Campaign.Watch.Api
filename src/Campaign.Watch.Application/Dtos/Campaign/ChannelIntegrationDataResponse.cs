using System;

namespace Campaign.Watch.Application.Dtos.Campaign
{
    /// <summary>
    /// DTO para dados de integração de canal
    /// </summary>
    public class ChannelIntegrationDataResponse
    {
        public string ChannelName { get; set; }
        public string IntegrationStatus { get; set; }
        public string TemplateId { get; set; }
        public FileInfoDataResponse File { get; set; }
        public LeadsDataResponse Leads { get; set; }
    }

    public class FileInfoDataResponse
    {
        public string Name { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public long Total { get; set; }
    }

    public class LeadsDataResponse
    {
        public int? Blocked { get; set; }
        public int? Deduplication { get; set; }
        public int? Error { get; set; }
        public int? Optout { get; set; }
        public int? Success { get; set; }
    }
}
