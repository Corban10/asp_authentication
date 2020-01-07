using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace taohi_backend.Models
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}
