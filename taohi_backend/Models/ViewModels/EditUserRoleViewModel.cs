using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace taohi_backend.Models
{
    public class EditUserRoleViewModel
    {
        public Guid Id { get; set; }
        public List<UserRoleViewModel> Users { get; set; }
    }
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
    }
}
