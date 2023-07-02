namespace SocialMediaProject.DTOs
{
    public class CreateMessageDTO
    {
        public required int UserID { get; set; }

        public required string Content { get; set; }
    }
}
