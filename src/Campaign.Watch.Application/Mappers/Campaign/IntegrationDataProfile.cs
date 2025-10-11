using AutoMapper;
using Campaign.Watch.Application.Dtos.Campaign;
using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Entities.Read.Effmail;
using Campaign.Watch.Domain.Entities.Read.Effpush;
using Campaign.Watch.Domain.Entities.Read.Effsms;
using Campaign.Watch.Domain.Entities.Read.Effwhatsapp;

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

            // Mapeamentos de Leitura de Canais para Entidades de Integração de Domínio
            CreateMap<EffmailRead, EmailIntegrationData>();
            CreateMap<EffsmsRead, SmsIntegrationData>();
            CreateMap<EffpushRead, PushIntegrationData>();
            CreateMap<EffwhatsappRead, WhatsAppIntegrationData>()
                .ForMember(dest => dest.File, opt => opt.MapFrom(src => src.Archive));

            // Mapeamentos de sub-tipos de Leitura para Domínio
            CreateMap<Domain.Entities.Read.Effmail.FileInfo, FileInfoData>();
            CreateMap<Domain.Entities.Read.Effsms.FileInfo, FileInfoData>();
            CreateMap<Domain.Entities.Read.Effpush.FileInfo, FileInfoData>();
            CreateMap<Domain.Entities.Read.Effwhatsapp.ArchiveInfo, FileInfoData>()
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total));

            CreateMap<Domain.Entities.Read.Effmail.Leads, LeadsData>();
            CreateMap<Domain.Entities.Read.Effsms.Leads, LeadsData>();
            CreateMap<Domain.Entities.Read.Effpush.Leads, LeadsData>();
            CreateMap<Domain.Entities.Read.Effwhatsapp.Leads, LeadsData>();
        }
    }
}
