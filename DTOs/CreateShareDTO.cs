namespace SocialMediaProject.DTOs
{
    public class CreateShareDTO
    {
        public required int PostID { get; set; }

        public required string Content { get; set; }
    }
}
