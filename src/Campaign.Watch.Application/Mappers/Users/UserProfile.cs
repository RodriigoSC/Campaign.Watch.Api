using AutoMapper;
using Campaign.Watch.Application.Dtos.Users;
using Campaign.Watch.Domain.Entities.Users;

namespace Campaign.Watch.Application.Mappers.Users
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserEntity, UserInfo>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

            CreateMap<UserEntity, SettingsDto>()
                .ForPath(dest => dest.Profile.Name, opt => opt.MapFrom(src => src.Name))
                .ForPath(dest => dest.Profile.Email, opt => opt.MapFrom(src => src.Email))
                .ForPath(dest => dest.Profile.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForPath(dest => dest.Profile.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.Settings))
                .ForMember(dest => dest.General, opt => opt.MapFrom(src => src.Settings));

            CreateMap<UserSettings, SystemSettingsDto>();
            CreateMap<UserSettings, GeneralSettingsDto>();

            CreateMap<UserEntity, UserSummaryResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
        }
    }
}
