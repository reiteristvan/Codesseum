using System.ComponentModel.DataAnnotations;

namespace Codesseum.Web.Areas.Beta.Models
{
    public class SubscribeModel
    {
        [Required]
        [Display(Name = "Felhasználónév")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email cím")]
        public string Email { get; set; }
    }
}