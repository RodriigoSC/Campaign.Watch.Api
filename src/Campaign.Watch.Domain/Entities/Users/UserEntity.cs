using Campaign.Watch.Domain.Entities.Common;
using System;

namespace Campaign.Watch.Domain.Entities.Users
{
    public class UserEntity : CommonEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; } = true;
        public UserSettings Settings { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    }
    public class UserSettings
    {
        public string ApiUrl { get; set; }
        public int Timeout { get; set; }
        public int RefreshInterval { get; set; }
        public bool EnableDetailedLogs { get; set; }
        public bool DebugMode { get; set; }
        public string Language { get; set; }
        public string Timezone { get; set; }
        public string DateFormat { get; set; }
        public bool EmailNotifications { get; set; }
        public bool PushNotifications { get; set; }
        public bool AlertSounds { get; set; }

        public UserSettings()
        {
            ApiUrl = "";
            Timeout = 30;
            RefreshInterval = 5;
            Language = "pt-BR";
            Timezone = "America/Sao_Paulo";
            DateFormat = "DD/MM/YYYY";
            EmailNotifications = true;
            PushNotifications = true;
        }
    }

}
