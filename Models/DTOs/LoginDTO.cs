using System.ComponentModel.DataAnnotations;

namespace pcp2p.Models
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Please fill in your Username.")]
        public string Username {get;set;}
        [Required(ErrorMessage = "Please fill in your Password.")]
        public string Password {set;get;}
        public bool Rememberme = false;
    }
}