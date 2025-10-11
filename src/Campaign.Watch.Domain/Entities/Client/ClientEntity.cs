using Campaign.Watch.Domain.Entities.Common;
using Campaign.Watch.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Campaign.Watch.Domain.Entities.Client
{
    /// <summary>
    /// Representa a entidade de um cliente no sistema.
    /// Herda propriedades comuns de CommonEntity.
    /// </summary>
    public class ClientEntity : CommonEntity
    {
        /// <summary>
        /// O nome do cliente.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indica se o cliente está ativo no sistema.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Configurações específicas para as campanhas do cliente.
        /// </summary>
        public CampaignConfig CampaignConfig { get; set; }

        /// <summary>
        /// Lista dos canais de comunicação efetivos configurados para o cliente.
        /// </summary>
        public List<EffectiveChannel> EffectiveChannels { get; set; }

        /// <summary>
        /// Data e hora de criação do registro do cliente.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data e hora da última modificação no registro do cliente.
        /// </summary>
        public DateTime ModifiedAt { get; set; }
    }

    /// <summary>
    /// Armazena as configurações de acesso aos dados de campanha de um cliente.
    /// </summary>
    public class CampaignConfig
    {
        /// <summary>
        /// O ID do projeto associado ao cliente.
        /// </summary>
        public string ProjectID { get; set; }

        /// <summary>
        /// O nome do banco de dados onde os dados de campanha do cliente estão armazenados.
        /// </summary>
        public string Database { get; set; }
    }

    /// <summary>
    /// Classe base abstrata para representar um canal de comunicação efetivo.
    /// Utiliza BsonKnownTypes para suportar polimorfismo no MongoDB para as classes derivadas.
    /// </summary>
    [BsonKnownTypes(typeof(EffectiveMail),typeof(EffectiveSms),typeof(EffectivePush),typeof(EffectivePages),typeof(EffectiveSocial),typeof(EffectiveWhatsApp), typeof(EffectiveApi))]
    public abstract class EffectiveChannel
    {
        /// <summary>
        /// O tipo do canal de comunicação (ex: Email, Sms).
        /// </summary>
        public ChannelType TypeChannel { get; set; }

        /// <summary>
        /// O nome descritivo do canal.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// O nome do banco de dados associado a este canal.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// O ID do tenant (inquilino) associado a este canal.
        /// </summary>
        public string TenantID { get; set; }
    }

    /// <summary>
    /// Representa o canal de comunicação E-mail.
    /// </summary>
    public class EffectiveMail : EffectiveChannel { }

    /// <summary>
    /// Representa o canal de comunicação SMS.
    /// </summary>
    public class EffectiveSms : EffectiveChannel { }

    /// <summary>
    /// Representa o canal de comunicação Push Notification.
    /// </summary>
    public class EffectivePush : EffectiveChannel { }

    /// <summary>
    /// Representa o canal de comunicação Pages (páginas web/landing pages).
    /// </summary>
    public class EffectivePages : EffectiveChannel { }

    /// <summary>
    /// Representa o canal de comunicação Social (redes sociais).
    /// </summary>
    public class EffectiveSocial : EffectiveChannel { }

    /// <summary>
    /// Representa o canal de comunicação WhatsApp.
    /// </summary>
    public class EffectiveWhatsApp : EffectiveChannel { }

    /// <summary>
    /// Representa o canal de API.
    /// </summary>
    public class EffectiveApi : EffectiveChannel { }
}