using Campaign.Watch.Application.Dtos.Diagnostic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Interfaces.Campaign
{
    public interface IDiagnosticApplication
    {
        Task<CampaignDiagnosticResponse> ObterDiagnosticoCampanhaAsync(string campaignId);
        Task<IEnumerable<DiagnosticIssueDto>> ObterProblemasDetectadosAsync(string severity = null, DateTime? desde = null, int limite = 100);
    }
}
