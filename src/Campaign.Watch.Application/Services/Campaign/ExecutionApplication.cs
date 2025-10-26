using AutoMapper;
using Campaign.Watch.Application.Dtos.Execution;
using Campaign.Watch.Application.Interfaces.Campaign;
using Campaign.Watch.Domain.Entities.Campaign;
using Campaign.Watch.Domain.Interfaces.Repositories.Campaign;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Services.Campaign
{
    public class ExecutionApplication : IExecutionApplication
    {
        private readonly IExecutionRepository _executionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ExecutionApplication> _logger;

        public ExecutionApplication(IExecutionRepository executionRepository, IMapper mapper, ILogger<ExecutionApplication> logger)
        {
            _executionRepository = executionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ExecutionMonitoringResponse>> ObterExecucoesPorCampanhaAsync(string campaignMonitoringId)
        {
            if (!ObjectId.TryParse(campaignMonitoringId, out var objectId))
                return Enumerable.Empty<ExecutionMonitoringResponse>();

            var executions = await _executionRepository.ObterExecucoesPorCampanhaAsync(objectId);

            return executions.Select(exec =>
            {
                var response = _mapper.Map<ExecutionMonitoringResponse>(exec);
                response.HealthSummary = CalcularResumoSaudeExecucao(exec);
                return response;
            });
        }

        public async Task<ExecutionMonitoringResponse> ObterExecucaoPorIdAsync(string executionId)
        {
            var execution = await _executionRepository.ObterExecucaoPorIdAsync(executionId);

            if (execution == null)
                return null;

            var response = _mapper.Map<ExecutionMonitoringResponse>(execution);
            response.HealthSummary = CalcularResumoSaudeExecucao(execution);
            return response;
        }

        public async Task<IEnumerable<ExecutionMonitoringResponse>> ObterExecucoesComErrosAsync(string clientName = null, System.DateTime? dataInicio = null, System.DateTime? dataFim = null)
        {
            var executions = await _executionRepository.ObterExecucoesComErrosAsync(
                clientName, dataInicio, dataFim);

            return executions.Select(exec =>
            {
                var response = _mapper.Map<ExecutionMonitoringResponse>(exec);
                response.HealthSummary = CalcularResumoSaudeExecucao(exec);
                return response;
            });
        }

        #region Métodos Auxiliares (Movidos)

        private ExecutionHealthSummaryDto CalcularResumoSaudeExecucao(ExecutionEntity execution)
        {
            var steps = execution.Steps ?? Enumerable.Empty<WorkflowStepEntity>();

            return new ExecutionHealthSummaryDto
            {
                OverallHealth = execution.HasMonitoringErrors ? "Error" : "Healthy",
                TotalSteps = steps.Count(),
                HealthySteps = steps.Count(s =>
                    string.IsNullOrEmpty(s.Error) && string.IsNullOrEmpty(s.MonitoringNotes)),
                StepsWithWarnings = steps.Count(s =>
                    !string.IsNullOrEmpty(s.MonitoringNotes) && string.IsNullOrEmpty(s.Error)),
                StepsWithErrors = steps.Count(s => !string.IsNullOrEmpty(s.Error)),
                CriticalSteps = 0,
                MainIssues = steps
                    .Where(s => !string.IsNullOrEmpty(s.MonitoringNotes) ||
                               !string.IsNullOrEmpty(s.Error))
                    .Select(s => s.MonitoringNotes ?? s.Error)
                    .Take(5)
                    .ToList()
            };
        }

        #endregion
    }
}
