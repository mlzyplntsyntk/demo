using Demo.Library.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models.Entities
{
    public class Article : AbstractEntity
    {
        [Required(ErrorMessage ="Title is Required")]
        public string Title { get; set; }
        [Required(ErrorMessage ="Content is Required")]
        public string Content { get; set; }
        
        public int? UserId { get; set; }

        public string Photo { get; set; }

        public string GetFriendlyURL() =>
            String.Format("{0}-{1}", this.Title.ToSlug(), this.Id.ToString());

        public int TotalLikes { get; set; } = 0;

        public int TotalReads { get; set; } = 0;

        public virtual User User { get; set; }
    }
}
