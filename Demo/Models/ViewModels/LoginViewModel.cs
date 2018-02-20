using Demo.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please Enter a valid E-mail")]
        [EmailAddress]
        public string EMailAddress { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password should be minimum of 8 Characters")]
        [MaxLength(50, ErrorMessage = "Password should be maximum of 50 Characters")]
        [UIHint("password")]
        public string Password { get; set; }

        // this property is used to redirect the user to the Referer Page
        // when he/she successfully logins. Honestly, we may need this
        // future for other Actions too, So it would be better to Create an Abstract
        // ViewModel for All ViewModels and extend them from the Abstract Class. 
        public string ReturnURL { get; set; }
    }
}
