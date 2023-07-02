namespace SocialMediaProject.DTOs
{
    public class CreateCommentDTO
    {
        public required int PostID { get; set; }

        public required string Content { get; set; }
    }
}
