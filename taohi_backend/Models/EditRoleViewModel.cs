using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace taohi_backend.Models
{
    public class EditRoleViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "RoleName is required")]
        public string RoleName { get; set; }
        public List<string> Users { get; set; }
    }
}
