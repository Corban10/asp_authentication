using System.ComponentModel.DataAnnotations;

namespace taohi_backend.Models
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; }
    }
}
