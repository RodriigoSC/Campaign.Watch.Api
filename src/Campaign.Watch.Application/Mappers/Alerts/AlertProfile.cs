using AutoMapper;
using Campaign.Watch.Application.Dtos.Alerts;
using Campaign.Watch.Domain.Entities.Alerts;
using MongoDB.Bson;

namespace Campaign.Watch.Application.Mappers.Alerts
{
    public class AlertProfile : Profile
    {
        public AlertProfile()
        {

            // Request (DTO) para Entidade
            CreateMap<SaveAlertConfigurationRequest, AlertConfigurationEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.ClientId) ? (ObjectId?)null : ObjectId.Parse(src.ClientId)))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            // Entidade para Response (DTO)
            CreateMap<AlertConfigurationEntity, AlertConfigurationResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src =>
                    src.ClientId.HasValue ? src.ClientId.Value.ToString() : null));


            // Entidade para Response (DTO)
            CreateMap<AlertHistoryEntity, AlertHistoryResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src =>
                    src.ClientId.HasValue ? src.ClientId.Value.ToString() : null))
                .ForMember(dest => dest.AlertConfigurationId, opt => opt.MapFrom(src => src.AlertConfigurationId.ToString()));
        }
    }
}
