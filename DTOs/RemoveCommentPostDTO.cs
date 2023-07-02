namespace SocialMediaProject.DTOs
{
    public class RemoveCommentPostDTO
    {
        public required int PostID { get; set; }

        public required int CommentID { get; set; }
    }
}
