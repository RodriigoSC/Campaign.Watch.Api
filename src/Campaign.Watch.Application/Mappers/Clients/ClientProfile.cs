using AutoMapper;
using Campaign.Watch.Application.Dtos.Client;
using Campaign.Watch.Domain.Entities.Client;

namespace Campaign.Watch.Application.Mappers.Client
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<SaveClientRequest, ClientEntity>();

            CreateMap<ClientEntity, ClientResponse>();

            CreateMap<CampaignConfigDto, CampaignConfig>();
            CreateMap<CampaignConfig, CampaignConfigDto>();
        }
    }
}