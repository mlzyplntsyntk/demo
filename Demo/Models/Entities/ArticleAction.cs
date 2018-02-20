using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Models.Entities
{
    public class ArticleAction : AbstractEntity
    {
        public ArticleActionType ArticleActionType { get; set; }
        public Article Article { get; set; }
        public User User { get; set; }

    }
}
