namespace SocialMediaProject.DTOs
{
    public class RegisterDTO
    {
        public required string Username { get; set; }

        public required string Password { get; set; }

        public required string Email { get; set; }

        public required string PhoneNumber { get; set; }

        public required string Description { get; set; }

        public required string Name { get; set; }

        public required string Country { get; set; }

        public required string City { get; set; }

        public required string Gender { get; set; }

        public required DateOnly Birthdate { get; set; }
    }
}
