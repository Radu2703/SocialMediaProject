using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMediaProject.Data.Entities
{
    public class Post
    {
        public int ID { get; set; }

        public required int UserID { get; set; }

        public required string Content { get; set; }

        public required DateTime Creation { get; set; }

        public User User { get; set; }

        public ICollection<Like>? Likes { get; set; }

        public ICollection<Dislike>? Dislikes { get; set; }

        public ICollection<Share>? Shares { get; set; }

        public ICollection<Comment>? Comments { get; set; }
    }
}
