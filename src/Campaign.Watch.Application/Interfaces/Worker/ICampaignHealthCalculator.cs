using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Enums;
using System;

namespace Campaign.Watch.Application.Interfaces.Worker
{
    public interface ICampaignHealthCalculator
    {
        CampaignHealthResult Calculate(CampaignEntity campaign, DateTime now);
    }

    public record CampaignHealthResult(MonitoringHealthStatus HealthStatus,MonitoringStatus MonitoringStatus,CampaignType CampaignType,DateTime? NextExecutionTime);
}
