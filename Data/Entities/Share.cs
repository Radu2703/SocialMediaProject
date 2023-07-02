using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMediaProject.Data.Entities
{
    public class Share
    {
        public int ID { get; set; }

        public required int UserID { get; set; }

        public required int PostID { get; set; }

        public Post Post { get; set; }

        public required string Content { get; set; }

        public required DateTime Creation { get; set; }
    }
}
