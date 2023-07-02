using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMediaProject.Data.Entities
{
    public class Follow
    {
        public int ID { get; set; }

        public required int FollowerID { get; set; }

        public User Follower { get; set; }

        public required int FollowedID { get; set; }

        public required DateTime Creation { get; set; }
    }
}