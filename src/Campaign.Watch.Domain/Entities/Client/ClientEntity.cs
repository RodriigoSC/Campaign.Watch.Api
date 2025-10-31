using Campaign.Watch.Domain.Entities.Common;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Campaign.Watch.Domain.Entities.Client
{
    /// <summary>
    /// Entidade principal do Cliente.
    /// </summary>
    public class ClientEntity : CommonEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public CampaignConfig CampaignConfig { get; set; }

        // MODIFICADO: Trocado de List<EffectiveChannel> para o objeto aninhado
        public EffectiveChannels EffectiveChannels { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }

    /// <summary>
    /// Configuração do banco de dados de campanha do cliente.
    /// </summary>
    public class CampaignConfig
    {
        public string ProjectID { get; set; }
        public string Database { get; set; }
    }

    /// <summary>
    /// NOVO: Representa o objeto aninhado "EffectiveChannels"
    /// </summary>
    [BsonIgnoreExtraElements]
    public class EffectiveChannels
    {
        [BsonElement("EFFMAIL")]
        public List<ChannelDbConfig> Effmail { get; set; } = new();

        [BsonElement("EFFSMS")]
        public List<ChannelDbConfig> Effsms { get; set; } = new();

        [BsonElement("EFFPUSH")]
        public List<ChannelDbConfig> Effpush { get; set; } = new();

        [BsonElement("EFFWHATSAPP")]
        public List<ChannelDbConfig> Effwhatsapp { get; set; } = new();
    }

    /// <summary>
    /// NOVO: Representa a configuração de um banco de dados de canal
    /// (Item dentro das listas EFFMAIL, EFFSMS, etc.)
    /// </summary>
    [BsonIgnoreExtraElements]
    public class ChannelDbConfig
    {
        public string Name { get; set; }
        public string Integration { get; set; }
        public string DataBase { get; set; }
    }

    // REMOVIDO: As classes antigas
    // [BsonKnownTypes(...)]
    // public abstract class EffectiveChannel { ... }
    // public class EffectiveMail : EffectiveChannel { }
    // ... (e as demais classes de canal)
}