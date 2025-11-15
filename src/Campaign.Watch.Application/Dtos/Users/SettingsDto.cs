using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Users
{
    public class SettingsDto
    {
        public ProfileSettingsDto Profile { get; set; }
        public SystemSettingsDto System { get; set; }
        public GeneralSettingsDto General { get; set; }
    }

    public class ProfileSettingsDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
    }

    public class SystemSettingsDto
    {
        public string ApiUrl { get; set; }
        public int Timeout { get; set; }
        public int RefreshInterval { get; set; }
        public bool EnableDetailedLogs { get; set; }
        public bool DebugMode { get; set; }
    }

    public class GeneralSettingsDto
    {
        public string Language { get; set; }
        public string Timezone { get; set; }
        public string DateFormat { get; set; }
        public bool EmailNotifications { get; set; }
        public bool PushNotifications { get; set; }
        public bool AlertSounds { get; set; }
    }
}
