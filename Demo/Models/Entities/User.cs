using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IdentityModel.Tokens.Jwt;

namespace Demo.Models.Entities
{
    public class User : AbstractEntity
    {
        [MaxLength(50)]
        [Required]
        public string FirstName { get; set; }

        [MaxLength(50)]
        [Required]
        public string LastName { get; set; }

        public UserType UserType { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [MinLength(8)]
        public string Password { get; set; }

        public DateTime Birthday { get; set; }

        public virtual ICollection<Article> Articles { get; set; }

        [NotMapped]
        public string UserToken { get; set; }
    }
}
