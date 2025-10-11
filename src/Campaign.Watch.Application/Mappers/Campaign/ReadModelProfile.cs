using AutoMapper;
using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Entities.Read.Campaign;
using Campaign.Watch.Domain.Enums;

namespace Campaign.Watch.Application.Mappers.Campaign
{
    public class ReadModelProfile : Profile
    {
        public ReadModelProfile()
        {
            // Mapeamentos de Dados de Leitura (Origem) para Domínio
            CreateMap<CampaignRead, CampaignEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IdCampaign, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StatusCampaign, opt => opt.MapFrom(src => (CampaignStatus)src.Status))
                .ForMember(dest => dest.CampaignType, opt => opt.MapFrom(src => (CampaignType)src.Type))
                .ForMember(dest => dest.Executions, opt => opt.Ignore());

            CreateMap<SchedulerReadModel, Scheduler>();

            CreateMap<ExecutionRead, Execution>()
                .ForMember(dest => dest.ExecutionId, opt => opt.MapFrom(src => src.ExecutionId.ToString()))
                .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.WorkflowExecution));

            CreateMap<WorkflowExecutionReadModel, Workflows>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.TotalUser, opt => opt.MapFrom(src => src.TotalUsers))
                .ForMember(dest => dest.ChannelName, opt => {
                    opt.MapFrom(src =>
                        (src.ExecutionData != null && src.ExecutionData.Contains("ChannelName"))
                            ? src.ExecutionData["ChannelName"].AsString
                            : null);
                });

        }
    }
}
