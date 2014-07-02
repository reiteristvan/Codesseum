using System.ComponentModel.DataAnnotations;

namespace Codesseum.Web.Areas.Beta.Models
{
    public class SubscribeModel
    {
        [Display(Name = "Felhasználónév")]
        public string Username { get; set; }

        [Display(Name = "Email cím")]
        public string Email { get; set; }
    }
}