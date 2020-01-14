using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace asp_auth.Models
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public UserType UserType { get; set; }
        public bool IsActive { get; set; }
        public string Token { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
    }
}
