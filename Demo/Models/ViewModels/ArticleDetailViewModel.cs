using Demo.Models.Entities;

namespace Demo.Models.ViewModels
{
    public class ArticleDetailViewModel
    {
        public Article Article { get; set; }
        public bool Liked { get; set; }
        public User User { get; set; }
    }
}
