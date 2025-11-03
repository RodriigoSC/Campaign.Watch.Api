using AutoMapper;
using Campaign.Watch.Application.Dtos.Campaign;
using Campaign.Watch.Application.Dtos.Common; // <--- ADICIONADO
using Campaign.Watch.Application.Dtos.Execution; // <--- ADICIONADO
using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Extensions;

namespace Campaign.Watch.Application.Mappers.Campaign
{
    public class CampaignMonitoringProfile : Profile
    {
        public CampaignMonitoringProfile()
        {
            // ============================================
            // Mapeamento de CampaignEntity para Response
            // ============================================
            CreateMap<CampaignEntity, CampaignMonitoringResponse>()
                .ForMember(dest => dest.CampaignType,
                    opt => opt.MapFrom(src => src.CampaignType.GetDescription()))
                .ForMember(dest => dest.MonitoringStatus,
                    opt => opt.MapFrom(src => src.MonitoringStatus.GetDescription()))
                .ForMember(dest => dest.CampaignStatus,
                    opt => opt.MapFrom(src => src.StatusCampaign.GetDescription()))
                .ForMember(dest => dest.Metrics, opt => opt.Ignore()); // Calculado separadamente

            // ============================================
            // Mapeamento de ExecutionEntity para Response
            // ============================================
            CreateMap<ExecutionEntity, ExecutionMonitoringResponse>()
                .ForMember(dest => dest.CampaignMonitoringId,
                    opt => opt.MapFrom(src => src.CampaignMonitoringId.ToString()))
                .ForMember(dest => dest.HealthSummary, opt => opt.Ignore()); // Calculado separadamente

            // ============================================
            // Mapeamento de WorkflowStepEntity para Response
            // ============================================
            CreateMap<WorkflowStepEntity, WorkflowStepResponse>()
                .ForMember(dest => dest.HealthInfo, opt => opt.Ignore()); // Calculado separadamente

            // ============================================
            // Mapeamento de IntegrationData
            // ============================================
            CreateMap<ChannelIntegrationDataEntity, ChannelIntegrationDataResponse>();
            CreateMap<FileInfoDataEntity, FileInfoDataResponse>();
            CreateMap<LeadsDataEntity, LeadsDataResponse>();

            // ============================================
            // Mapeamentos de estruturas comuns (A CORREÇÃO ESTÁ AQUI)
            // ============================================
            // Isso informa ao AutoMapper como mapear as propriedades aninhadas
            // que nós movemos para a pasta /Common/
            CreateMap<MonitoringHealthStatus, MonitoringHealthStatusDto>();
            CreateMap<Scheduler, SchedulerResponse>();
        }
    }
}