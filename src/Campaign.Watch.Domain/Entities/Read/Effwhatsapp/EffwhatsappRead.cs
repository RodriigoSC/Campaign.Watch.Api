using Campaign.Watch.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Domain.Entities.Read.Effwhatsapp
{
    [BsonIgnoreExtraElements]
    public class EffwhatsappRead
    {
        /// <summary>
        /// O ID único do documento na origem.
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }

        /// <summary>
        /// O status geral da trigger.
        /// </summary>
        [BsonElement("Status")]
        public string Status { get; set; }

        /// <summary>
        /// O nome da trigger de e-mail.
        /// </summary>
        [BsonElement("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Parâmetros de identificação que conectam a trigger a outras entidades.
        /// </summary>
        [BsonElement("Parameters")]
        public Parameters Parameters { get; set; }

        /// <summary>
        /// Data de criação do registro na origem.
        /// </summary>
        [BsonElement("CreatedAt")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Data da última modificação do registro na origem.
        /// </summary>
        [BsonElement("ModifiedAt")]
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// ID do template de e-mail utilizado.
        /// </summary>
        [BsonElement("TemplateId")]
        public string TemplateId { get; set; }

        /// <summary>
        /// Informações sobre o arquivo de leads associado.
        /// </summary>
        [BsonElement("Archive")]
        public ArchiveInfo Archive { get; set; }

        /// <summary>
        /// Informações sobre o tipo do broker associado.
        /// </summary>
        [BsonElement("BrokerType")]
        public int BrokerType { get; set; }

        /// <summary>
        /// Mensagem de erro, caso tenha ocorrido algum.
        /// </summary>
        [BsonElement("Error")]
        public string Error { get; set; }

        /// <summary>
        /// Estatísticas do processamento dos leads.
        /// </summary>
        [BsonElement("Leads")]
        public Leads Leads { get; set; }
    }

    /// <summary>
    /// Contém os parâmetros de identificação de uma trigger de e-mail.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Parameters
    {
        /// <summary>
        /// ID da campanha associada.
        /// </summary>
        [BsonElement("CampaignId")]
        public string CampaignId { get; set; }

        /// <summary>
        /// ID da execução da campanha associada.
        /// </summary>
        [BsonElement("ExecutionId")]
        public string ExecutionId { get; set; }

        /// <summary>
        /// ID do workflow associado.
        /// </summary>
        [BsonElement("WorkflowId")]
        public string WorkflowId { get; set; }

        /// <summary>
        /// ID do projeto associado.
        /// </summary>
        [BsonElement("ProjectId")]
        public string ProjectId { get; set; }

        /// <summary>
        /// Nome do workflow associado.
        /// </summary>
        [BsonElement("WorkflowName")]
        public string WorkflowName { get; set; }

        /// <summary>
        /// ID numérico da campanha associada.
        /// </summary>
        [BsonElement("NumberId")]
        public string NumberId { get; set; }
    }

    /// <summary>
    /// Contém informações detalhadas sobre um arquivo de leads.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class ArchiveInfo
    {
        /// <summary>
        /// Nome do arquivo.
        /// </summary>
        [BsonElement("Name")]
        public string Name { get; set; }        

        /// <summary>
        /// Hash de verificação do arquivo.
        /// </summary>
        [BsonElement("Hash")]
        public string Hash { get; set; }

        /// <summary>
        /// Caractere separador de colunas no arquivo (ex: vírgula, ponto e vírgula).
        /// </summary>
        [BsonElement("Separator")]
        public string Separator { get; set; }

        /// <summary>
        /// Dados de personalização contidos no arquivo.
        /// </summary>
        [BsonElement("Personalizations")]
        public string Personalizations { get; set; }

        /// <summary>
        /// Data de criação do arquivo.
        /// </summary>
        [BsonElement("CreatedAt")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Data de início do processamento do arquivo.
        /// </summary>
        [BsonElement("StartedAt")]
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// Data de finalização do processamento do arquivo.
        /// </summary>
        [BsonElement("FinishedAt")]
        public DateTime? FinishedAt { get; set; }

        /// <summary>
        /// Indica se o processamento do arquivo foi concluído.
        /// </summary>
        [BsonElement("Completed")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool Completed { get; set; }

        /// <summary>
        /// Número total de registros no arquivo.
        /// </summary>
        [BsonElement("Total")]
        public long Total { get; set; }
    }

    /// <summary>
    /// Contém as estatísticas do processamento de leads.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Leads
    {
        /// <summary>
        /// ID do gatilho ao qual estas estatísticas pertencem.
        /// </summary>
        [BsonElement("TriggerId")]
        public string TriggerId { get; set; }

        /// <summary>
        /// Número de leads bloqueados.
        /// </summary>
        [BsonElement("Blocked")]
        public int? Blocked { get; set; }

        /// <summary>
        /// Número de leads removidos por duplicação.
        /// </summary>
        [BsonElement("Deduplication")]
        public int? Deduplication { get; set; }

        /// <summary>
        /// Número de leads que resultaram em erro.
        /// </summary>
        [BsonElement("Error")]
        public int? Error { get; set; }

        /// <summary>
        /// Número de leads que solicitaram opt-out.
        /// </summary>
        [BsonElement("Optout")]
        public int? Optout { get; set; }

        /// <summary>
        /// Número de leads processados com sucesso.
        /// </summary>
        [BsonElement("Success")]
        public int? Success { get; set; }

        /// <summary>
        /// Propriedade temporária para armazenar os leads vindos do $lookup na agregação.
        /// Não precisa ser mapeada no seu banco de dados, é apenas para uso em tempo de execução.
        /// </summary>
        [BsonElement("Items")]
        public List<LeadDocument> Items { get; set; } = new List<LeadDocument>();
    }

    /// <summary>
    /// Classe auxiliar para desserializar os campos necessários da collection 'Lead' durante a agregação.
    /// O driver precisa de um tipo para mapear os dados do $lookup.
    /// </summary>
    public class LeadDocument
    {
        [BsonElement("TriggerId")]
        public string TriggerId { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; }
    }
}
