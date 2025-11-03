using AutoMapper;
using Campaign.Watch.Application.Dtos.Alerts;
using Campaign.Watch.Domain.Entities.Alerts;
using Campaign.Watch.Domain.Enums.Alerts;
using MongoDB.Bson;
using System;

namespace Campaign.Watch.Application.Mappers.Alerts
{
    public class AlertProfile : Profile
    {
        public AlertProfile()
        {
            CreateMap<SaveAlertConfigurationRequest, AlertConfigurationEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.ClientId) ? (ObjectId?)null : ObjectId.Parse(src.ClientId)))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())

                .ForMember(dest => dest.Type, opt => opt.MapFrom(src =>
                    Enum.Parse<AlertChannelType>(src.Type, true))) 

                .ForMember(dest => dest.ConditionType, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.ConditionType)
                        ? (AlertConditionType?)null
                        : Enum.Parse<AlertConditionType>(src.ConditionType, true)))

                .ForMember(dest => dest.MinSeverity, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.MinSeverity)
                        ? (AlertSeverity?)null 
                        : Enum.Parse<AlertSeverity>(src.MinSeverity, true)));


            CreateMap<AlertConfigurationEntity, AlertConfigurationResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src =>
                    src.ClientId.HasValue ? src.ClientId.Value.ToString() : null))

                .ForMember(dest => dest.Type, opt => opt.MapFrom(src =>
                    src.Type.ToString()))

                .ForMember(dest => dest.ConditionType, opt => opt.MapFrom(src =>
                    src.ConditionType.HasValue ? src.ConditionType.Value.ToString() : null))

                .ForMember(dest => dest.MinSeverity, opt => opt.MapFrom(src =>
                    src.MinSeverity.HasValue ? src.MinSeverity.Value.ToString() : null));


            CreateMap<AlertHistoryEntity, AlertHistoryResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src =>
                    src.ClientId.HasValue ? src.ClientId.Value.ToString() : null))
                .ForMember(dest => dest.AlertConfigurationId, opt => opt.MapFrom(src => src.AlertConfigurationId.ToString()));
        }
    }
}