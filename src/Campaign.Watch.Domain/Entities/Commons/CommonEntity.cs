using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Campaign.Watch.Domain.Entities.Common
{
    /// <summary>
    /// Classe base para entidades, fornecendo uma propriedade de identificação comum.
    /// </summary>
    public class CommonEntity
    {
        /// <summary>
        /// O identificador único da entidade no banco de dados (MongoDB).
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }
    }
}