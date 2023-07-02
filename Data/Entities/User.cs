using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMediaProject.Data.Entities
{
    public class User
    {
        public int ID { get; set; }

        public required string Username { get; set; }

        public required string HashedPassword { get; set; }

        public required string Email { get; set; }

        public required string PhoneNumber { get; set; }

        public required string Description { get; set; }

        public required string Name { get; set; }

        public required string Country { get; set; }

        public required string City { get; set; }

        public required string Gender { get; set; }

        public required DateTime Birthdate { get; set; }

        public required DateTime Creation { get; set; }

        public ICollection<Post>? Posts { get; set; }

        public ICollection<Message>? SentMessages { get; set; }

        public ICollection<Block>? Blocking { get; set; }

        public ICollection<Follow>? Following { get; set; }
    }
}
