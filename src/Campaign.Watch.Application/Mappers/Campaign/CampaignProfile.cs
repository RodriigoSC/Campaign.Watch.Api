using AutoMapper;
using Campaign.Watch.Application.Dtos.Campaign;
using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Enums;
using Campaign.Watch.Domain.Extensions;
using MongoDB.Bson;

namespace Campaign.Watch.Application.Mappers.Campaign
{
    public class CampaignProfile : Profile
    {
        public CampaignProfile()
        {
            // Mapeamentos de Entidade de Domínio para DTOs (Saída)
            CreateMap<CampaignEntity, CampaignDetailResponse>()
             .ForMember(dest => dest.CampaignType, opt => opt.MapFrom(src => src.CampaignType.GetDescription()))
             .ForMember(dest => dest.MonitoringStatus, opt => opt.MapFrom(src => src.MonitoringStatus.GetDescription()))
             .ForMember(dest => dest.CampaignStatus, opt => opt.MapFrom(src => src.StatusCampaign.GetDescription()));

            CreateMap<CampaignEntity, CampaignStatusResponse>()
             .ForMember(dest => dest.CampaignType, opt => opt.MapFrom(src => src.CampaignType.GetDescription()))
             .ForMember(dest => dest.MonitoringStatus, opt => opt.MapFrom(src => src.MonitoringStatus.GetDescription()))
             .ForMember(dest => dest.CampaignStatus, opt => opt.MapFrom(src => src.StatusCampaign.GetDescription()));

            CreateMap<CampaignEntity, CampaignErrorResponse>()
                .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src => src.HealthStatus.LastMessage))
                .ForMember(dest => dest.LastExecutionWithIssueId, opt => opt.MapFrom(src => src.HealthStatus.LastExecutionWithIssueId));

            CreateMap<CampaignEntity, CampaignDelayedResponse>()
                .ForMember(dest => dest.MonitoringStatus, opt => opt.MapFrom(src => src.MonitoringStatus.GetDescription()))
                .ForMember(dest => dest.StartDateTime, opt => opt.MapFrom(src => src.Scheduler.StartDateTime));

            CreateMap<CampaignStatusCount, CampaignStatusCountResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.GetDescription()));

            CreateMap<CampaignMonitoringStatusCount, CampaignStatusCountResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.GetDescription()));

            CreateMap<MonitoringHealthStatus, MonitoringHealthStatusDto>();
            CreateMap<Scheduler, SchedulerResponse>();
            CreateMap<Execution, ExecutionResponse>();
            CreateMap<Workflows, WorkflowResponse>();
            CreateMap<FileInfoData, FileInfoDataResponse>();
            CreateMap<LeadsData, LeadsDataResponse>();

            // Mapeamentos de DTOs para Entidade de Domínio (Entrada)
            CreateMap<CampaignDetailResponse, CampaignEntity>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Id) ? ObjectId.Parse(src.Id) : ObjectId.Empty))
                 .ForMember(dest => dest.CampaignType, opt => opt.MapFrom(src => src.CampaignType.ToEnumByDescription<CampaignType>()))
                 .ForMember(dest => dest.MonitoringStatus, opt => opt.MapFrom(src => src.MonitoringStatus.ToEnumByDescription<MonitoringStatus>()))
                 .ForMember(dest => dest.StatusCampaign, opt => opt.MapFrom(src => src.CampaignStatus.ToEnumByDescription<CampaignStatus>()));

            CreateMap<MonitoringHealthStatusDto, MonitoringHealthStatus>();
            CreateMap<SchedulerResponse, Scheduler>();
            CreateMap<ExecutionResponse, Execution>();
            CreateMap<WorkflowResponse, Workflows>();
            CreateMap<FileInfoDataResponse, FileInfoData>();
            CreateMap<LeadsDataResponse, LeadsData>();
        }
    }
}