using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Diagnostic
{
    /// <summary>
    /// Recomendação baseada no diagnóstico
    /// </summary>
    public class DiagnosticRecommendationDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
    }
}
