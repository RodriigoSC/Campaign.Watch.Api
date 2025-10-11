using Campaign.Watch.Domain.Entities.Read.Effmail;
using Campaign.Watch.Domain.Interfaces.Services.Read.Effmail;
using Campaign.Watch.Infra.Effmail.Factories;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Campaign.Watch.Infra.Effmail.Services
{
    public class EffmailReadService : IEffmailReadService
    {
        private readonly IEffmailMongoFactory _factory;

        public EffmailReadService(IEffmailMongoFactory factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<EffmailRead>> GetTriggerEffmail(string dbName, string workflowId)
        {
            if (workflowId == "99aa534b77ed5eccd4331934")
            {
                Console.WriteLine("Email encontrado");
            }
            var database = _factory.GetDatabase(dbName);
            var triggerCollection = database.GetCollection<EffmailRead>("Trigger");

            var pipeline = new BsonDocument[]
            {
                // ETAPA 1: Filtrar as triggers pelo WorkflowId
                new BsonDocument("$match", new BsonDocument
                {
                    { "Parameters.WorkflowId", workflowId },
                    { "IsTest", false }
                }),

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
                                { "_id", "$LastStatus" },
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
                    { "StatusTrigger", 1 }, { "Parameters", 1 }, { "IsTest", 1 }, { "ReplyTo", 1 },
                    { "CreatedAt", 1 }, { "ModifiedAt", 1 }, { "TemplateId", 1 }, { "SchedulerId", 1 },
                    { "Transactional", 1 }, { "WebhookEnabled", 1 }, { "ExistsExternalId", 1 },
                    { "WebhookAPIs", 1 }, { "File", 1 }, { "Error", 1 },

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

            var aggregation = await triggerCollection.Aggregate<EffmailRead>(pipeline).ToListAsync();
            return aggregation;
        }
    }
}