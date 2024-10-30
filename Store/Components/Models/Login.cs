using System.ComponentModel.DataAnnotations;

namespace Store.Components.Models
{
    public class Login
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}