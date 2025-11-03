using AutoMapper;
using Campaign.Watch.Application.Dtos.Alerts;
using Campaign.Watch.Application.Interfaces.Alerts;
using Campaign.Watch.Domain.Entities.Alerts;
using Campaign.Watch.Domain.Interfaces.Repositories.Alerts;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Services.Alerts
{
    public class AlertApplication : IAlertApplication
    {
        private readonly IAlertConfigurationRepository _configRepository;
        private readonly IAlertHistoryRepository _historyRepository;
        private readonly IMapper _mapper;

        public AlertApplication(
            IAlertConfigurationRepository configRepository,
            IAlertHistoryRepository historyRepository,
            IMapper mapper)
        {
            _configRepository = configRepository;
            _historyRepository = historyRepository;
            _mapper = mapper;
        }

        private ObjectId? ParseScope(string clientId)
        {
            if (string.IsNullOrEmpty(clientId) || clientId.Equals("global", StringComparison.OrdinalIgnoreCase))
            {
                return null; // Global
            }
            if (ObjectId.TryParse(clientId, out var id))
            {
                return id; // Específico
            }
            throw new ArgumentException("ClientId inválido.");
        }

        public async Task<AlertConfigurationResponse> CreateAlertAsync(SaveAlertConfigurationRequest request)
        {
            var entity = _mapper.Map<AlertConfigurationEntity>(request);
            // O mapper já deve ter lidado com ClientId (string) para ClientId (ObjectId?)

            var createdEntity = await _configRepository.CreateAsync(entity);
            return _mapper.Map<AlertConfigurationResponse>(createdEntity);
        }

        public async Task<bool> DeleteAlertAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return false;

            return await _configRepository.DeleteAsync(objectId);
        }

        public async Task<IEnumerable<AlertConfigurationResponse>> GetAllAlertsAsync(string clientId)
        {
            var scope = ParseScope(clientId);
            var entities = await _configRepository.GetByScopeAsync(scope);
            return _mapper.Map<IEnumerable<AlertConfigurationResponse>>(entities);
        }

        public async Task<IEnumerable<AlertHistoryResponse>> GetAlertHistoryAsync(string clientId)
        {
            var scope = ParseScope(clientId);
            var entities = await _historyRepository.GetByScopeAsync(scope);
            return _mapper.Map<IEnumerable<AlertHistoryResponse>>(entities);
        }

        public async Task<AlertConfigurationResponse> GetAlertByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return null;

            var entity = await _configRepository.GetByIdAsync(objectId);
            return _mapper.Map<AlertConfigurationResponse>(entity);
        }

        public async Task<bool> UpdateAlertAsync(string id, SaveAlertConfigurationRequest request)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return false;

            // Busca a entidade existente para garantir que o ClientId não seja alterado
            var existing = await _configRepository.GetByIdAsync(objectId);
            if (existing == null)
                return false;

            // Mapeia os dados do request para a entidade (sem alterar ClientId ou CreatedAt)
            _mapper.Map(request, existing);

            return await _configRepository.UpdateAsync(objectId, existing);
        }
    }
}
