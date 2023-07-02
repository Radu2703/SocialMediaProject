using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMediaProject.Data.Entities
{
    public class Message
    {
        public int ID { get; set; }

        public required int MessagerID { get; set; }

        public User Messager { get; set; }

        public required int MessagedID { get; set; }

        public required string Content { get; set; }

        public required DateTime Creation { get; set; }
    }
}