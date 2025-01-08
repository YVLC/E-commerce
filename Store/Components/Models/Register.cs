using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Store.Components.Models
{
    public class Register
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username must be at most 50 characters long.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "Password must be at least 6 characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string RepeatPassword { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string Phone_number { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [Phone(ErrorMessage = "Invalid address.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Postcode required.")]
        [Phone(ErrorMessage = "Invalid postcode")]
        public string Postcode { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }
        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }
    }
}