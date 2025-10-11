using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Campaign.Watch.Domain.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Retorna a descrição do enum definida pelo DescriptionAttribute.
        /// </summary>
        /// <param name="value">O valor do enum.</param>
        /// <returns>A string de descrição, ou o nome do enum se o atributo não for encontrado.</returns>
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field != null)
            {
                var attribute = field.GetCustomAttribute<DescriptionAttribute>();
                if (attribute != null)
                    return attribute.Description;
            }
            return value.ToString();
        }

        /// <summary>
        /// Converte uma string de descrição de volta para o valor do enum correspondente.
        /// Este é o método que você precisa para o mapeamento de DTO para Entity.
        /// </summary>
        /// <typeparam name="T">O tipo do enum de destino.</typeparam>
        /// <param name="description">A string (descrição) a ser convertida.</param>
        /// <returns>O valor do enum correspondente.</returns>
        /// <exception cref="ArgumentException">Lançada se a descrição não for encontrada em nenhum membro do enum.</exception>
        public static T ToEnumByDescription<T>(this string description) where T : Enum
        {
            if (string.IsNullOrEmpty(description))
            {
                // Decida se quer retornar o valor default(T) ou lançar uma exceção se a string for vazia/nula.
                // Exemplo:
                // return default(T);
                throw new ArgumentException("A descrição do enum não pode ser nula ou vazia.");
            }

            foreach (var field in typeof(T).GetFields())
            {
                // 1. Tenta encontrar a descrição no DescriptionAttribute
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description.Equals(description, StringComparison.OrdinalIgnoreCase))
                        return (T)field.GetValue(null);
                }

                // 2. Tenta fazer um fallback para o nome do membro do enum (se a descrição não for encontrada)
                if (field.Name.Equals(description, StringComparison.OrdinalIgnoreCase))
                {
                    return (T)field.GetValue(null);
                }
            }

            // Se não encontrou nenhuma correspondência, lança uma exceção.
            throw new ArgumentException($"A descrição '{description}' não corresponde a nenhum membro do enum {typeof(T).Name}.");
        }
    }
}