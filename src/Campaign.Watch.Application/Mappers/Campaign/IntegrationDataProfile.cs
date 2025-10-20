using AutoMapper;
using Campaign.Watch.Application.Dtos.Campaign;
using Campaign.Watch.Domain.Entities.Campaign;


namespace Campaign.Watch.Application.Mappers.Campaign
{
    public class IntegrationDataProfile : Profile
    {
        public IntegrationDataProfile()
        {
            // Mapeamentos Polimórficos de Dados de Integração (Domínio -> DTO)
            CreateMap<IntegrationDataBase, IntegrationDataResponseBase>()
                .Include<EmailIntegrationData, EmailIntegrationDataResponse>()
                .Include<SmsIntegrationData, SmsIntegrationDataResponse>()
                .Include<PushIntegrationData, PushIntegrationDataResponse>()
                .Include<WhatsAppIntegrationData, WhatsAppIntegrationDataResponse>();

            CreateMap<EmailIntegrationData, EmailIntegrationDataResponse>();
            CreateMap<SmsIntegrationData, SmsIntegrationDataResponse>();
            CreateMap<PushIntegrationData, PushIntegrationDataResponse>();
            CreateMap<WhatsAppIntegrationData, WhatsAppIntegrationDataResponse>();

            // Mapeamentos Polimórficos de Dados de Integração (DTO -> Domínio)
            CreateMap<IntegrationDataResponseBase, IntegrationDataBase>()
                .Include<EmailIntegrationDataResponse, EmailIntegrationData>()
                .Include<SmsIntegrationDataResponse, SmsIntegrationData>()
                .Include<PushIntegrationDataResponse, PushIntegrationData>()
                .Include<WhatsAppIntegrationDataResponse, WhatsAppIntegrationData>();

            CreateMap<EmailIntegrationDataResponse, EmailIntegrationData>();
            CreateMap<SmsIntegrationDataResponse, SmsIntegrationData>();
            CreateMap<PushIntegrationDataResponse, PushIntegrationData>();
            CreateMap<WhatsAppIntegrationDataResponse, WhatsAppIntegrationData>();            
        }
    }
}
