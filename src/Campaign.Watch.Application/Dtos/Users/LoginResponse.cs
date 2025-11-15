using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Users
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public UserInfo User { get; set; }
    }
    public class UserInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
