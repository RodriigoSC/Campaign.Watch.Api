namespace Campaign.Watch.Application.Dtos.Campaign
{
    /// <summary>
    /// Representa a contagem de campanhas para um status específico.
    /// </summary>
    public class CampaignStatusCountResponse
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }
}
