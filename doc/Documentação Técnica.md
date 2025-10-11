# Documentação Técnica - Campaign Watch Worker

## Especificações Técnicas

### Stack Tecnológico
- **.NET 5.0**: Framework base
- **MongoDB**: Base de dados principal (MongoDB.Bson 3.5.0)
- **AutoMapper 12.0.1**: Mapeamento entre objetos
- **NCrontab.Signed 3.4.0**: Processamento de expressões crontab
- **Microsoft.Extensions.Hosting**: Worker service
- **HashiCorp Vault**: Gerenciamento de secrets

### Arquitetura de Camadas

```
┌─────────────────────────────────┐
│      Worker.Channels            │ ← Entry Point
├─────────────────────────────────┤
│      Application Layer          │ ← Services & DTOs
├─────────────────────────────────┤
│      Domain Layer               │ ← Entities & Enums
├─────────────────────────────────┤
│      Infrastructure Layer       │ ← Repositories & External Services
└─────────────────────────────────┘
```

## Interfaces e Contratos

### Core Interfaces

#### ICampaignMonitorFlow
```csharp
public interface ICampaignMonitorFlow
{
    Task MonitorarCampanhasAsync();
}
```
**Implementação**: `CampaignMonitorFlow`  
**Responsabilidade**: Orquestração principal do fluxo de monitoramento

#### ICampaignDataProcessor
```csharp
public interface ICampaignDataProcessor
{
    Task<CampaignEntity> ProcessAndEnrichCampaignDataAsync(ClientResponse client, CampaignRead campaignSource);
}
```
**Implementação**: `CampaignDataProcessor`  
**Responsabilidade**: Processamento e enriquecimento de dados de campanhas

#### ICampaignHealthCalculator
```csharp
public interface ICampaignHealthCalculator
{
    CampaignHealthResult Calculate(CampaignEntity campaign, DateTime now);
}

public record CampaignHealthResult(
    MonitoringHealthStatus HealthStatus,
    MonitoringStatus MonitoringStatus,
    CampaignType CampaignType,
    DateTime? NextExecutionTime
);
```
**Implementação**: `CampaignHealthCalculator`  
**Responsabilidade**: Cálculo de métricas de saúde e status

### Repository Interfaces

#### Campaign Operations
```csharp
public interface ICampaignApplication
{
    Task<CampaignResponse> CreateCampaignAsync(CampaignResponse dto);
    Task<bool> UpdateCampaignAsync(string id, CampaignResponse dto);
    Task<IEnumerable<CampaignResponse>> GetAllCampaignsAsync();
    Task<CampaignResponse> GetCampaignByIdAsync(string id);
    Task<CampaignResponse> GetCampaignByIdCampaignAsync(string clientName, string idCampaign);
    Task<IEnumerable<CampaignResponse>> GetCampaignsDueForMonitoringAsync();
    // ... outros métodos
}
```

#### Source Data Access
```csharp
public interface ICampaignMonitorApplication
{
    Task<IEnumerable<CampaignRead>> GetSourceCampaignsByClientAsync(string dbName);
    Task<IEnumerable<ExecutionRead>> GetSourceExecutionsByCampaignAsync(string dbName, string campaignId);
    Task<CampaignRead> GetSourceCampaignByIdAsync(string dbName, string campaignId);
}
```

#### Channel-Specific Services
```csharp
// Email Channel
public interface IEffmailReadService
{
    Task<IEnumerable<EffmailRead>> GetTriggerEffmail(string dbName, string stepId);
}

// SMS Channel
public interface IEffsmsReadService
{
    Task<IEnumerable<EffsmsRead>> GetTriggerEffsms(string dbName, string stepId);
}

// Push Channel
public interface IEffpushReadService
{
    Task<IEnumerable<EffpushRead>> GetTriggerEffpush(string dbName, string stepId);
}

// WhatsApp Channel
public interface IEffwhatsappReadService
{
    Task<IEnumerable<EffwhatsappRead>> GetTriggerEffwhatsapp(string dbName, string stepId);
}
```

## Modelos de Dados

### Entidades Principais

#### CampaignEntity (Domain)
```csharp
public class CampaignEntity
{
    public ObjectId Id { get; set; }
    public string ClientName { get; set; }
    public string IdCampaign { get; set; }          // ID da campanha na origem
    public long NumberId { get; set; }
    public string Name { get; set; }
    public string TypeCampaign { get; set; }
    public string Description { get; set; }
    public string ProjectId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public CampaignStatus StatusCampaign { get; set; }
    public CampaignType CampaignType { get; set; }
    public MonitoringStatus MonitoringStatus { get; set; }
    public DateTime? NextExecutionMonitoring { get; set; }
    public DateTime? LastCheckMonitoring { get; set; }
    public MonitoringHealthStatus HealthStatus { get; set; }
    public Scheduler Scheduler { get; set; }
    public List<Execution> Executions { get; set; }
}
```

#### MonitoringHealthStatus
```csharp
public class MonitoringHealthStatus
{
    public bool IsFullyVerified { get; set; }       // Todos os dados foram verificados
    public bool HasPendingExecution { get; set; }   // Execução pendente/atrasada
    public bool HasIntegrationErrors { get; set; }  // Erros de integração detectados
    public string LastExecutionWithIssueId { get; set; } // ID da última execução com problema
    public string LastMessage { get; set; }         // Mensagem de status atual
}
```

#### Execution
```csharp
public class Execution
{
    public string ExecutionId { get; set; }
    public string CampaignName { get; set; }
    public string Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsFullyVerifiedByMonitoring { get; set; }
    public bool HasMonitoringErrors { get; set; }
    public List<Workflows> Steps { get; set; }
}
```

#### Workflows (Steps)
```csharp
public class Workflows
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }              // "Channel", "Wait", etc.
    public string ChannelName { get; set; }       // Nome do canal (para steps Channel)
    public string Status { get; set; }
    public long TotalUser { get; set; }
    public long TotalExecutionTime { get; set; }  // Em segundos
    public object Error { get; set; }
    public string MonitoringNotes { get; set; }   // Notas do worker
    public IntegrationDataBase IntegrationData { get; set; }
}
```

### Enumerações

#### CampaignStatus
```csharp
public enum CampaignStatus
{
    Scheduled = 1,    // Agendada
    Executing = 2,    // Executando
    Completed = 3,    // Concluída
    Error = 4,        // Erro
    Canceled = 5      // Cancelada
}
```

#### MonitoringStatus
```csharp
public enum MonitoringStatus
{
    Pending = 1,                    // Aguardando início
    InProgress = 2,                 // Em execução
    Completed = 3,                  // Concluída
    Failed = 4,                     // Falha
    ExecutionDelayed = 5,           // Execução atrasada
    WaitingForNextExecution = 6     // Aguardando próxima (recorrente)
}
```

#### ChannelType
```csharp
public enum ChannelType
{
    EffectiveMail = 1,
    EffectiveSms = 2,
    EffectivePush = 3,
    EffectiveApi = 4,
    EffectivePages = 5,
    EffectiveSocial = 6,
    EffectiveWhatsApp = 7
}
```

## Algoritmos Principais

### 1. Detecção de Execuções Faltantes

```csharp
private List<Execution> VerificarEAdicionarExecucoesFaltantes(CampaignEntity campaign)
{
    // Aplica apenas a campanhas recorrentes ativas
    if (campaign.Scheduler?.IsRecurrent != true || 
        !campaign.IsActive || 
        DateTime.UtcNow < campaign.Scheduler.StartDateTime)
    {
        return campaign.Executions ?? new List<Execution>();
    }

    var execucoesReais = campaign.Executions ?? new List<Execution>();
    var execucoesCombinadas = new List<Execution>(execucoesReais);

    // Calcula todas as execuções esperadas desde o início
    var datasEsperadas = SchedulerHelper.GetAllOccurrences(
        campaign.Scheduler.Crontab,
        campaign.Scheduler.StartDateTime,
        DateTime.UtcNow
    );

    // Otimização: HashSet para busca O(1)
    var datasReais = new HashSet<DateTime>(
        execucoesReais.Select(e => e.StartDate.Date)
    );

    // Identifica execuções faltantes
    foreach (var dataEsperada in datasEsperadas)
    {
        if (dataEsperada.Date >= DateTime.UtcNow.Date) continue;
        
        if (!datasReais.Contains(dataEsperada.Date))
        {
            var placeholder = CreateMissingExecutionPlaceholder(dataEsperada, campaign.Name);
            execucoesCombinadas.Add(placeholder);
        }
    }

    return execucoesCombinadas.OrderBy(e => e.StartDate).ToList();
}
```

### 2. Normalização de Crontab

```csharp
private static string NormalizeCrontabExpression(string crontabExpression)
{
    var parts = crontabExpression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    string fivePartExpression;

    if (parts.Length == 7)
    {
        // Formato: sec min hour day month dayweek year
        fivePartExpression = string.Join(" ", parts.Skip(1).Take(5));
    }
    else if (parts.Length == 6)
    {
        if (int.TryParse(parts.Last(), out var year) && year > 1970)
        {
            // Remove ano no final
            fivePartExpression = string.Join(" ", parts.Take(5));
        }
        else
        {
            // Remove segundos no início
            fivePartExpression = string.Join(" ", parts.Skip(1));
        }
    }
    else
    {
        fivePartExpression = crontabExpression;
    }

    // Substitui caracteres incompatíveis
    return fivePartExpression.Replace("?", "*");
}
```

### 3. Cálculo de Status de Saúde

```csharp
public CampaignHealthResult Calculate(CampaignEntity campaign, DateTime now)
{
    var campaignType = DeterminarTipoCampanha(campaign);
    var nextExecution = CalcularProximaExecucao(campaign, campaignType, now);
    
    var healthStatus = new MonitoringHealthStatus();
    var execucoesComErro = campaign.Executions?.Where(e => e.HasMonitoringErrors).ToList();

    // Verifica erros de integração
    healthStatus.HasIntegrationErrors = execucoesComErro?.Any() ?? false;
    if (healthStatus.HasIntegrationErrors)
    {
        var ultimaExecucaoComErro = execucoesComErro.OrderBy(e => e.StartDate).Last();
        healthStatus.LastExecutionWithIssueId = ultimaExecucaoComErro.ExecutionId;
        healthStatus.LastMessage = ExtrairMensagemDeErro(ultimaExecucaoComErro);
    }

    // Verifica componentes Wait ativos
    if (!healthStatus.HasIntegrationErrors)
    {
        VerificarEtapaDeEsperaAtiva(campaign, healthStatus);
    }

    var monitoringStatus = DeterminarStatusMonitoramento(campaign, healthStatus, campaignType);

    return new CampaignHealthResult(healthStatus, monitoringStatus, campaignType, nextExecution);
}
```

## Padrões de Mapeamento

### AutoMapper Profiles

#### CampaignMapper
```csharp
public class CampaignMapper : Profile
{
    public CampaignMapper()
    {
        // Origem para Domínio
        CreateMap<CampaignRead, CampaignEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IdCampaign, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.StatusCampaign, opt => opt.MapFrom(src => (CampaignStatus)src.Status))
            .ForMember(dest => dest.CampaignType, opt => opt.MapFrom(src => (CampaignType)src.Type));

        // Scheduler específico
        CreateMap<SchedulerReadModel, Scheduler>();

        // Execuções
        CreateMap<ExecutionRead, Execution>()
            .ForMember(dest => dest.ExecutionId, opt => opt.MapFrom(src => src.ExecutionId.ToString()))
            .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.WorkflowExecution));

        // Steps com mapeamento condicional de canal
        CreateMap<WorkflowExecutionReadModel, Workflows>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.TotalUser, opt => opt.MapFrom(src => src.TotalUsers))
            .ForMember(dest => dest.ChannelName, opt => opt.MapFrom(src =>
                (src.ExecutionData != null && src.ExecutionData.Contains("ChannelName"))
                    ? src.ExecutionData["ChannelName"].AsString
                    : null));

        // Mapeamentos polimórficos para IntegrationData
        CreateMap<IntegrationDataBase, IntegrationDataResponseBase>()
            .Include<EmailIntegrationData, EmailIntegrationDataResponse>()
            .Include<SmsIntegrationData, SmsIntegrationDataResponse>()
            .Include<PushIntegrationData, PushIntegrationDataResponse>()
            .Include<WhatsAppIntegrationData, WhatsAppIntegrationDataResponse>();
    }
}
```

## Configurações de Injeção de Dependência

### ResolverIoC
```csharp
public static class ResolverIoC
{
    public static IServiceCollection AddApplications(this IServiceCollection services)
    {
        // Core Services
        services.AddTransient<IClientApplication, ClientApplication>();
        services.AddTransient<ICampaignApplication, CampaignApplication>();

        // Monitoring Services
        services.AddTransient<ICampaignMonitorApplication, CampaignMonitorApplication>();
        services.AddTransient<ICampaignMonitorFlow, CampaignMonitorFlow>();
        services.AddScoped<ICampaignHealthCalculator, CampaignHealthCalculator>();
        services.AddScoped<ICampaignDataProcessor, CampaignDataProcessor>();

        // Mappers
        services.AddAutoMapper(typeof(ClientMapper));
        services.AddAutoMapper(typeof(CampaignMapper));

        return services;
    }
}
```

## Estratégias de Error Handling

### Hierarquia de Erros

#### 1. Cliente-Level Error Handling
```csharp
try 
{
    await ProcessarCampanhasDoClienteAsync(cliente);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Erro ao processar campanhas do cliente '{ClientName}'. Continuando com próximo cliente.", cliente.Name);
    // Não interrompe o loop principal
    continue;
}
```

#### 2. Campanha-Level Error Handling
```csharp
try
{
    await ProcessarCampanhaUnicaAsync(cliente, campanhaOrigem);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Falha ao processar a campanha '{CampaignName}'.", campanha.Name);
    // Não interrompe processamento de outras campanhas
    continue;
}
```

#### 3. Integration-Level Error Handling
```csharp
private void DefinirErroDeMonitoramento(Execution execution, Workflows step, string errorMessage)
{
    step.MonitoringNotes = errorMessage;
    execution.HasMonitoringErrors = true;
    execution.IsFullyVerifiedByMonitoring = false;
    _logger.LogWarning("Erro de monitoramento no step '{StepName}': {ErrorMessage}", step.Name, errorMessage);
}
```

## Performance Considerations

### Otimizações Implementadas

#### 1. HashSet para Lookup Rápido
```csharp
// O(1) para verificação de existência
var datasReais = new HashSet<DateTime>(
    execucoesReais.Select(e => e.StartDate.Date)
);
```

#### 2. Processamento Assíncrono
```csharp
// Processamento não-bloqueante de campanhas
foreach (var campaign in campaigns)
{
    await ProcessarCampanhaUnicaAsync(client, campaign);
}
```

#### 3. Lazy Loading de Integração
```csharp
// Só busca dados de integração para steps Channel
if (step.Type != "Channel" || string.IsNullOrEmpty(step.ChannelName)) return;
```

### Limitações de Performance

1. **Processamento Sequencial**: Campanhas por cliente são processadas sequencialmente
2. **Sem Cache Persistente**: Configurações são re-buscadas a cada ciclo
3. **Dependência de Rede**: Múltiplas consultas a bases externas por campanha

## Configurações de Sistema

### appsettings.json Structure
```json
{
  "WorkerSettings": {
    "Enabled": true,
    "ExecutionIntervalMinutes": 1,
    "MaxRetryAttempts": 3,
    "RetryDelaySeconds": 30,
    "HealthCheckIntervalMinutes": 15
  },
  "MonitoringSettings": {
    "CampaignBatchSize": 50,
    "MaxConcurrentClients": 3,
    "DelayedExecutionThresholdMinutes": 15,
    "EnableDetailedLogging": false
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Campaign.Watch": "Debug",
      "Microsoft": "Warning"
    }
  }
}
```

### Environment Variables
```bash
# HashiCorp Vault Configuration
CONN_STRING_VAULT=http://vault.company.com:8200
USER_VAULT=campaign-watch-service
PASS_VAULT=<secret-password>

# Runtime Environment
ASPNETCORE_ENVIRONMENT=Production
```

## Database Schema Considerations

### MongoDB Collections

#### campaigns (Principal)
```javascript
{
  "_id": ObjectId("..."),
  "clientName": "ClienteABC",
  "idCampaign": "camp_123",
  "numberId": 123,
  "name": "Campanha Marketing Q1",
  "typeCampaign": "Promotional",
  "statusCampaign": 2, // CampaignStatus enum
  "monitoringStatus": 1, // MonitoringStatus enum
  "nextExecutionMonitoring": ISODate("2024-01-15T10:00:00Z"),
  "lastCheckMonitoring": ISODate("2024-01-14T15:30:00Z"),
  "healthStatus": {
    "isFullyVerified": true,
    "hasPendingExecution": false,
    "hasIntegrationErrors": false,
    "lastMessage": "Campanha monitorada sem problemas"
  },
  "scheduler": {
    "startDateTime": ISODate("2024-01-01T09:00:00Z"),
    "endDateTime": ISODate("2024-12-31T23:59:59Z"),
    "isRecurrent": true,
    "crontab": "0 9 * * MON"
  },
  "executions": [...]
}
```

#### clients
```javascript
{
  "_id": ObjectId("..."),
  "name": "ClienteABC",
  "isActive": true,
  "campaignConfig": {
    "projectID": "proj_abc",
    "database": "client_abc_campaigns"
  },
  "effectiveChannels": [
    {
      "typeChannel": 1, // ChannelType.EffectiveMail
      "name": "Email Marketing",
      "database": "client_abc_email",
      "tenantID": "tenant_123"
    }
  ]
}
```

### Indexing Strategy

#### Recommended Indexes
```javascript
// campaigns collection
db.campaigns.createIndex({ "clientName": 1, "idCampaign": 1 }, { unique: true })
db.campaigns.createIndex({ "monitoringStatus": 1 })
db.campaigns.createIndex({ "nextExecutionMonitoring": 1 })
db.campaigns.createIndex({ "lastCheckMonitoring": 1 })
db.campaigns.createIndex({ "healthStatus.hasIntegrationErrors": 1 })

// clients collection
db.clients.createIndex({ "name": 1 }, { unique: true })
db.clients.createIndex({ "isActive": 1 })
```

## Testing Considerations

### Unit Test Categories

#### 1. SchedulerHelper Tests
```csharp
[Test]
public void GetNextExecution_ValidCrontab_ReturnsCorrectDate()
{
    var result = SchedulerHelper.GetNextExecution("0 9 * * MON", new DateTime(2024, 1, 1));
    Assert.That(result, Is.Not.Null);
}

[Test]  
public void NormalizeCrontabExpression_SevenFields_ReturnsFiveFields()
{
    var input = "0 0 9 * * MON 2024";
    var expected = "0 9 * * MON";
    // Test implementation
}
```

#### 2. Health Calculator Tests
```csharp
[Test]
public void Calculate_CampaignWithErrors_ReturnsFailedStatus()
{
    var campaign = new CampaignEntity { /* setup with errors */ };
    var result = _healthCalculator.Calculate(campaign, DateTime.Now);
    Assert.That(result.MonitoringStatus, Is.EqualTo(MonitoringStatus.Failed));
}
```

#### 3. Data Processor Tests
```csharp
[Test]
public async Task ProcessAndEnrichCampaignDataAsync_RecurrentCampaign_DetectsMissingExecutions()
{
    // Setup recurrent campaign with missing executions
    var result = await _dataProcessor.ProcessAndEnrichCampaignDataAsync(client, campaignSource);
    Assert.That(result.Executions.Any(e => e.Status == "MissingInSource"), Is.True);
}
```

## Security Considerations

### 1. Connection String Security
- Todas as connection strings são armazenadas no HashiCorp Vault
- Autenticação via usuário/senha do serviço
- Rotação regular de credenciais

### 2. Database Access
- Acesso read-only às bases de origem
- Conexões isoladas por cliente
- Timeout configurável nas operações

### 3. Logging Security
- Não log de dados sensíveis (IDs pessoais, etc.)
- Sanitização de mensagens de erro
- Structured logging para análise

## Deployment Architecture

### Production Setup
```yaml
# docker-compose.yml example
version: '3.8'
services:
  campaign-watch-worker:
    image: campaign-watch-worker:latest
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - CONN_STRING_VAULT=${VAULT_URL}
      - USER_VAULT=${VAULT_USER}
      - PASS_VAULT=${VAULT_PASS}
    deploy:
      replicas: 1  # Worker deve ter apenas 1 instância
      resources:
        limits:
          memory: 512M
          cpus: '0.5'
    depends_on:
      - mongodb
      - vault
```

### Health Check Endpoint
```csharp
// Para implementação futura
public class HealthCheckController : ControllerBase
{
    [HttpGet("/health")]
    public async Task<IActionResult> Get()
    {
        return Ok(new { 
            Status = "Healthy", 
            LastExecution = _lastExecution,
            ProcessedCampaigns = _processedCount 
        });
    }
}
```

---

**Nota**: Esta documentação reflete a arquitetura atual. Para implementação de melhorias, consulte o roadmap no arquivo `Melhorias.txt`.