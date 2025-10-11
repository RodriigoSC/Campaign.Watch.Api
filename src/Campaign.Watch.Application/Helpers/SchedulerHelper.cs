using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Campaign.Watch.Application.Helpers
{
    public static class SchedulerHelper
    {
        /// <summary>
        /// Obtém a próxima data de execução baseada na expressão crontab.
        /// </summary>
        /// <param name="crontabExpression">Expressão crontab (normalmente 5 campos: min hour day month dayweek).</param>
        /// <param name="fromDate">Data base para calcular (opcional, usa DateTime.UtcNow se não informado).</param>
        /// <returns>A próxima data de execução ou null se a expressão for inválida.</returns>
        public static DateTime? GetNextExecution(string crontabExpression, DateTime? fromDate = null)
        {
            if (string.IsNullOrWhiteSpace(crontabExpression))
            {
                return null;
            }

            try
            {
                var cleanExpression = NormalizeCrontabExpression(crontabExpression);
                var schedule = CrontabSchedule.Parse(cleanExpression);
                var baseDate = fromDate ?? DateTime.UtcNow;
                return schedule.GetNextOccurrence(baseDate);
            }
            catch (CrontabException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao processar crontab '{crontabExpression}': {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro inesperado ao processar crontab '{crontabExpression}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtém múltiplas próximas execuções de uma expressão crontab.
        /// </summary>
        /// <param name="crontabExpression">A expressão crontab.</param>
        /// <param name="count">O número de ocorrências a serem retornadas.</param>
        /// <param name="fromDate">A data a partir da qual o cálculo deve começar.</param>
        /// <returns>Uma lista enumerável com as próximas datas de execução.</returns>
        public static IEnumerable<DateTime> GetNextExecutions(string crontabExpression, int count = 5, DateTime? fromDate = null)
        {
            if (string.IsNullOrWhiteSpace(crontabExpression) || count <= 0)
            {
                return Enumerable.Empty<DateTime>();
            }

            CrontabSchedule schedule;
            try
            {
                var cleanExpression = NormalizeCrontabExpression(crontabExpression);
                schedule = CrontabSchedule.Parse(cleanExpression);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao processar múltiplas execuções para '{crontabExpression}': {ex.Message}");
                return Enumerable.Empty<DateTime>();
            }

            // A lógica de iteração (yield) é movida para um método auxiliar fora do try-catch.
            return GetNextOccurrencesIterator(schedule, count, fromDate);
        }

        /// <summary>
        /// Método auxiliar privado que realiza a iteração para GetNextExecutions.
        /// </summary>
        private static IEnumerable<DateTime> GetNextOccurrencesIterator(CrontabSchedule schedule, int count, DateTime? fromDate)
        {
            var current = fromDate ?? DateTime.UtcNow;
            for (int i = 0; i < count; i++)
            {
                current = schedule.GetNextOccurrence(current);
                yield return current;
            }
        }

        /// <summary>
        /// Verifica se uma expressão crontab é válida.
        /// </summary>
        /// <param name="crontabExpression">A expressão a ser validada.</param>
        /// <returns>True se a expressão for válida, caso contrário, false.</returns>
        public static bool IsValidCrontabExpression(string crontabExpression)
        {
            if (string.IsNullOrWhiteSpace(crontabExpression))
            {
                return false;
            }

            try
            {
                var cleanExpression = NormalizeCrontabExpression(crontabExpression);
                CrontabSchedule.Parse(cleanExpression);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica se uma campanha deveria ter sido executada baseada no seu cronograma.
        /// </summary>
        /// <param name="crontabExpression">A expressão crontab da campanha.</param>
        /// <param name="startDateTime">A data/hora de início configurada para a campanha.</param>
        /// <param name="lastExecution">A data da última execução conhecida (opcional).</param>
        /// <param name="checkTime">A data/hora atual da verificação (opcional, usa UtcNow como padrão).</param>
        /// <returns>True se a execução estiver atrasada, caso contrário, false.</returns>
        public static bool ShouldHaveExecuted(
            string crontabExpression,
            DateTime startDateTime,
            DateTime? lastExecution = null,
            DateTime? checkTime = null)
        {
            var now = checkTime ?? DateTime.UtcNow;

            if (now < startDateTime)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(crontabExpression))
            {
                return now > startDateTime && lastExecution == null;
            }

            try
            {
                var schedule = CrontabSchedule.Parse(NormalizeCrontabExpression(crontabExpression));
                // Define a data base para o cálculo: a última execução ou a data de início da campanha.
                var baseDate = lastExecution ?? startDateTime.AddMinutes(-1);

                var expectedExecution = schedule.GetNextOccurrence(baseDate);

                return now > expectedExecution;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Normaliza a expressão crontab para o formato de 5 campos esperado pela NCrontab.
        /// Remove campos extras e substitui caracteres incompatíveis como '?'.
        /// </summary>
        private static string NormalizeCrontabExpression(string crontabExpression)
        {
            var parts = crontabExpression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string fivePartExpression;

            if (parts.Length == 7)
            {
                fivePartExpression = string.Join(" ", parts.Skip(1).Take(5));
            }
            else if (parts.Length == 6) // Formatos com 6 campos
            {
                if (int.TryParse(parts.Last(), out var year) && year > 1970)
                {
                    fivePartExpression = string.Join(" ", parts.Take(5)); // Remove ano no final
                }
                else
                {
                    fivePartExpression = string.Join(" ", parts.Skip(1)); // Remove segundos no início
                }
            }
            else
            {
                fivePartExpression = crontabExpression; // Assume que já está no formato correto
            }

            return fivePartExpression.Replace("?", "*");
        }

        /// <summary>
        /// Obtém todas as ocorrências de uma expressão crontab dentro de um intervalo de datas.
        /// </summary>
        /// <param name="crontabExpression">A expressão crontab.</param>
        /// <param name="startTime">A data de início do intervalo.</param>
        /// <param name="endTime">A data de fim do intervalo.</param>
        /// <returns>Uma coleção com todas as datas de execução agendadas dentro do intervalo.</returns>
        public static IEnumerable<DateTime> GetAllOccurrences(string crontabExpression, DateTime startTime, DateTime endTime)
        {
            if (string.IsNullOrWhiteSpace(crontabExpression) || startTime > endTime)
            {
                return Enumerable.Empty<DateTime>();
            }

            var occurrences = new List<DateTime>();
            try
            {
                var cleanExpression = NormalizeCrontabExpression(crontabExpression);
                var schedule = CrontabSchedule.Parse(cleanExpression);

                // Pega a primeira ocorrência a partir da data de início
                var next = schedule.GetNextOccurrence(startTime);

                // Continua pegando a próxima ocorrência até que ela ultrapasse a data final
                while (next < endTime)
                {
                    occurrences.Add(next);
                    next = schedule.GetNextOccurrence(next);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter ocorrências para '{crontabExpression}': {ex.Message}");
                return Enumerable.Empty<DateTime>();
            }

            return occurrences;
        }
    }
}