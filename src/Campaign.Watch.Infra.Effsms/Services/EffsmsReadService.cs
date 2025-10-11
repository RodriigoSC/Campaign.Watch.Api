using Campaign.Watch.Domain.Entities.Read.Effsms;
using Campaign.Watch.Domain.Interfaces.Services.Read.Effsms;
using Campaign.Watch.Infra.Effsms.Factories;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Infra.Effsms.Services
{
    public class EffsmsReadService : IEffsmsReadService
    {
        private readonly IEffsmsMongoFactory _factory;

        public EffsmsReadService(IEffsmsMongoFactory factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<EffsmsRead>> GetTriggerEffsms(string dbName, string workflowId)
        {
            var database = _factory.GetDatabase(dbName);
            var triggerCollection = database.GetCollection<EffsmsRead>("Trigger");

            var pipeline = new BsonDocument[]
            {
                // ETAPA 1: Filtrar as triggers pelo WorkflowId
                new BsonDocument("$match", new BsonDocument("Parameters.WorkflowId", workflowId)),

                // ETAPA 2: Realizar o $lookup com a sub-pipeline para contar os leads
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Lead" },
                    { "let", new BsonDocument("trigger_id", "$_id") },
                    { "pipeline", new BsonArray
                        {
                            new BsonDocument("$match", new BsonDocument("$expr",
                                new BsonDocument("$eq", new BsonArray { "$TriggerId", new BsonDocument("$toString", "$$trigger_id") })
                            )),
                            new BsonDocument("$group", new BsonDocument
                            {
                                { "_id", "$LastStatus" }, // Usando LastStatus para SMS
                                { "count", new BsonDocument("$sum", 1) }
                            })
                        }
                    },
                    { "as", "leadCounts" }
                }),

                // ETAPA 3 (NOVA): Transformar o array de contagens em um objeto para acesso seguro
                new BsonDocument("$addFields", new BsonDocument
                {
                    { "leadCountsObj", new BsonDocument("$arrayToObject",
                        new BsonDocument("$map", new BsonDocument
                            {
                                { "input", "$leadCounts" },
                                { "as", "item" },
                                { "in", new BsonDocument { { "k", "$$item._id" }, { "v", "$$item.count" } } }
                            }
                        ))
                    }
                }),

                // ETAPA 4 (FINAL): Projetar o resultado final lendo do novo objeto
                new BsonDocument("$project", new BsonDocument
                {
                    // Mantém todos os campos originais da Trigger
                    { "_id", 1 }, { "Status", 1 }, { "Name", 1 }, { "AppointmentDate", 1 },
                    { "StatusTrigger", 1 }, { "Parameters", 1 }, { "CreatedAt", 1 },
                    { "ModifiedAt", 1 }, { "TemplateId", 1 }, { "SchedulerId", 1 },
                    { "BrokerId", 1 }, { "Transactional", 1 }, { "BrokerName", 1 },
                    { "WebhookEnabled", 1 }, { "WebhookAPIs", 1 }, { "File", 1 }, { "Error", 1 },

                    // Cria o objeto Leads a partir do objeto de contagens, de forma segura
                    { "Leads", new BsonDocument
                        {
                            { "Success", new BsonDocument("$ifNull", new BsonArray { "$leadCountsObj.Success", 0 }) },
                            { "Error", new BsonDocument("$ifNull", new BsonArray { "$leadCountsObj.Error", 0 }) },
                            { "Blocked", new BsonDocument("$ifNull", new BsonArray { "$leadCountsObj.Blocked", 0 }) },
                            { "Optout", new BsonDocument("$ifNull", new BsonArray { "$leadCountsObj.Optout", 0 }) },
                            { "Deduplication", new BsonDocument("$ifNull", new BsonArray { "$leadCountsObj.Deduplication", 0 }) }
                        }
                    }
                })
            };

            var aggregation = await triggerCollection.Aggregate<EffsmsRead>(pipeline).ToListAsync();
            return aggregation;
        }
    }
}