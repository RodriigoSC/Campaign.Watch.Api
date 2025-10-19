using AutoMapper;
using Campaign.Watch.Application.Dtos.Campaign;
using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Extensions;

namespace Campaign.Watch.Application.Mappers.Campaign
{
    public class CampaignMonitoringProfile : Profile
    {
        public CampaignMonitoringProfile()
        {
            // Mapeamento de CampaignEntity para CampaignMonitoringResponse
            CreateMap<CampaignEntity, CampaignMonitoringResponse>()
                .ForMember(dest => dest.CampaignType, opt => opt.MapFrom(src => src.CampaignType.GetDescription()))
                .ForMember(dest => dest.MonitoringStatus, opt => opt.MapFrom(src => src.MonitoringStatus.GetDescription()))
                .ForMember(dest => dest.CampaignStatus, opt => opt.MapFrom(src => src.StatusCampaign.GetDescription()))
                .ForMember(dest => dest.Metrics, opt => opt.Ignore()); // Calculado separadamente

            // Mapeamento de Execution para ExecutionMonitoringResponse
            CreateMap<Execution, ExecutionMonitoringResponse>()
                .ForMember(dest => dest.CampaignMonitoringId, opt => opt.Ignore()) // Preenchido no serviço
                .ForMember(dest => dest.OriginalCampaignId, opt => opt.Ignore())
                .ForMember(dest => dest.OriginalExecutionId, opt => opt.MapFrom(src => src.ExecutionId))
                .ForMember(dest => dest.HealthSummary, opt => opt.Ignore()); // Calculado separadamente

            // Mapeamento de WorkflowStep para WorkflowStepResponse
            CreateMap<Workflows, WorkflowStepResponse>()
                .ForMember(dest => dest.OriginalStepId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.HealthInfo, opt => opt.Ignore()); // Calculado separadamente

            // Mapeamento de IntegrationData para Response
            CreateMap<IntegrationDataBase, ChannelIntegrationDataResponse>()
                .ForMember(dest => dest.ChannelName, opt => opt.MapFrom(src => src.ChannelName))
                .ForMember(dest => dest.IntegrationStatus, opt => opt.MapFrom(src => src.IntegrationStatus))
                .ForMember(dest => dest.TemplateId, opt => opt.Ignore())
                .ForMember(dest => dest.File, opt => opt.Ignore())
                .ForMember(dest => dest.Leads, opt => opt.Ignore());

            CreateMap<EmailIntegrationData, ChannelIntegrationDataResponse>()
                .IncludeBase<IntegrationDataBase, ChannelIntegrationDataResponse>()
                .ForMember(dest => dest.TemplateId, opt => opt.MapFrom(src => src.TemplateId))
                .ForMember(dest => dest.File, opt => opt.MapFrom(src => src.File))
                .ForMember(dest => dest.Leads, opt => opt.MapFrom(src => src.Leads));

            CreateMap<SmsIntegrationData, ChannelIntegrationDataResponse>()
                .IncludeBase<IntegrationDataBase, ChannelIntegrationDataResponse>()
                .ForMember(dest => dest.TemplateId, opt => opt.MapFrom(src => src.TemplateId))
                .ForMember(dest => dest.File, opt => opt.MapFrom(src => src.File))
                .ForMember(dest => dest.Leads, opt => opt.MapFrom(src => src.Leads));

            CreateMap<PushIntegrationData, ChannelIntegrationDataResponse>()
                .IncludeBase<IntegrationDataBase, ChannelIntegrationDataResponse>()
                .ForMember(dest => dest.TemplateId, opt => opt.MapFrom(src => src.TemplateId))
                .ForMember(dest => dest.File, opt => opt.MapFrom(src => src.File))
                .ForMember(dest => dest.Leads, opt => opt.MapFrom(src => src.Leads));

            CreateMap<WhatsAppIntegrationData, ChannelIntegrationDataResponse>()
                .IncludeBase<IntegrationDataBase, ChannelIntegrationDataResponse>()
                .ForMember(dest => dest.TemplateId, opt => opt.MapFrom(src => src.TemplateId))
                .ForMember(dest => dest.File, opt => opt.MapFrom(src => src.File))
                .ForMember(dest => dest.Leads, opt => opt.MapFrom(src => src.Leads));

            CreateMap<FileInfoData, FileInfoDataResponse>();
            CreateMap<LeadsData, LeadsDataResponse>();

            CreateMap<MonitoringHealthStatus, MonitoringHealthStatusDto>();
            CreateMap<Scheduler, SchedulerResponse>();
        }
    }
}
