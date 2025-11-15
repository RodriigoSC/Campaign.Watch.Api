using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Users
{
    public class AdminUpdateUserRequest
    {
        [Required]
        public string Name { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public string Phone { get; set; }
    }
}
