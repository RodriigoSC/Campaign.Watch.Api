using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Campaign.Watch.Domain.Entities.Read.Campaign
{
    /// <summary>
    /// Representa os dados brutos de uma campanha lidos da fonte de dados de origem.
    /// A anotação BsonIgnoreExtraElements permite que campos não mapeados na origem sejam ignorados durante a desserialização.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class CampaignRead
    {
        /// <summary>
        /// O ID único da campanha na origem, mapeado do campo "_id".
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; }

        /// <summary>
        /// ID numérico sequencial da campanha na origem.
        /// </summary>
        [BsonElement("NumberId")]
        public long NumberId { get; set; }

        /// <summary>
        /// Nome da campanha na origem.
        /// </summary>
        [BsonElement("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Tipo da campanha, representado como um inteiro.
        /// </summary>
        [BsonElement("Type")]
        public int Type { get; set; }

        /// <summary>
        /// Descrição da campanha.
        /// </summary>
        [BsonElement("Description")]
        public string Description { get; set; }

        /// <summary>
        /// ID do projeto ao qual a campanha pertence.
        /// </summary>
        [BsonElement("ProjectId")]
        public string ProjectId { get; set; }

        /// <summary>
        /// Indica se a campanha está ativa na origem.
        /// </summary>
        [BsonElement("IsActive")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsActive { get; set; }

        /// <summary>
        /// Data e hora de criação da campanha na origem.
        /// </summary>
        [BsonElement("CreatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data e hora da última modificação da campanha na origem.
        /// </summary>
        [BsonElement("ModifiedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime ModifiedAt { get; set; }

        /// <summary>
        /// Status da campanha na origem, representado como um inteiro.
        /// </summary>
        [BsonElement("Status")]
        public int Status { get; set; }

        /// <summary>
        /// Indica se a campanha foi marcada como deletada na origem.
        /// </summary>
        [BsonElement("IsDeleted")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Indica se a campanha foi restaurada após a exclusão na origem.
        /// </summary>
        [BsonElement("IsRestored")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsRestored { get; set; }

        /// <summary>
        /// Objeto com os detalhes de agendamento da campanha na origem.
        /// </summary>
        [BsonElement("Scheduler")]
        public SchedulerReadModel Scheduler { get; set; }

        /// <summary>
        /// Objeto que contém a jornada (fluxo de trabalho) da campanha.
        /// </summary>
        [BsonElement("Journey")]
        public JourneyReadModel Journey { get; set; }
    }

    /// <summary>
    /// Representa os dados de agendamento de uma campanha lidos da fonte de dados.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class SchedulerReadModel
    {
        /// <summary>
        /// ID do agendamento na API de origem.
        /// </summary>
        public string SchedulerAPIId { get; set; }

        /// <summary>
        /// Expressão Crontab que define a recorrência.
        /// </summary>
        public string Crontab { get; set; }

        /// <summary>
        /// Data e hora de início do agendamento.
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Data e hora de término do agendamento (opcional).
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// Indica se o agendamento é recorrente.
        /// </summary>
        /// 
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsRecurrent { get; set; }

        /// <summary>
        /// Indica se o agendamento está pausado.
        /// </summary>
        /// 
        [BsonRepresentation(BsonType.Boolean)]
        public bool IsPaused { get; set; }
    }

    /// <summary>
    /// Representa a jornada de uma campanha, contendo a lista de etapas do workflow.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class JourneyReadModel
    {
        /// <summary>
        /// Lista de etapas (workflows) que compõem a jornada da campanha.
        /// </summary>
        [BsonElement("Workflow")]
        public List<WorkflowReadModel> Workflow { get; set; }
    }

    /// <summary>
    /// Representa uma etapa individual (componente) dentro do workflow de uma campanha.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class WorkflowReadModel
    {
        /// <summary>
        /// ID da etapa do workflow.
        /// </summary>
        [BsonElement("_id")]
        public string Id { get; set; }

        /// <summary>
        /// Nome da etapa.
        /// </summary>
        [BsonElement("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Tipo do componente da etapa, representado como um inteiro.
        /// </summary>
        [BsonElement("ComponentType")]
        public int ComponentType { get; set; }
    }
}