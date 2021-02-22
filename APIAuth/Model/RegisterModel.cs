using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APIAuth.Model
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "User is required")]
        [StringLength(120)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is requried ")]
        public string Password { get; set; }
    }
}
