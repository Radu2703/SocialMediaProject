using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMediaProject.Data.Entities
{
    public class Block
    {
        public int ID { get; set; }

        public required int BlockerID { get; set; }

        public User Blocker { get; set; }

        public required int BlockedID { get; set; }

        public required DateTime Creation { get; set; }

    }
}

