using System.ComponentModel.DataAnnotations;

namespace pcp2p.Models
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Please fill in your Username.")]
        public string Username;
        [Required(ErrorMessage = "Please fill in your Password.")]
        public string Password;
        public bool Rememberme = false;
    }
}