using Campaign.Watch.Application.Dtos.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Interfaces.Campaign
{
    public interface IExecutionApplication
    {
        Task<IEnumerable<ExecutionMonitoringResponse>> ObterExecucoesPorCampanhaAsync(string campaignMonitoringId);
        Task<ExecutionMonitoringResponse> ObterExecucaoPorIdAsync(string executionId);
        Task<IEnumerable<ExecutionMonitoringResponse>> ObterExecucoesComErrosAsync(string clientName = null, DateTime? dataInicio = null, DateTime? dataFim = null);
    }
}
