using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMediaProject.Data;
using SocialMediaProject.Data.Entities;
using SocialMediaProject.DTOs;

namespace SocialMediaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostModificationController : ControllerBase
    {
        private readonly SocialMediaProjectDb _db;
        private readonly IConfiguration _config;

        public PostModificationController(SocialMediaProjectDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("ModifyPost")]//modify a post
        public ActionResult ModifyPost([FromBody] ModifyPostDTO payload)
        {
            System.Security.Claims.ClaimsPrincipal currentUser = HttpContext.User;
            int userID;

            try
            {
                if (currentUser.HasClaim(c => c.Type == "ID"))
                {
                    userID = int.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "ID").Value);
                }
                else { return new JsonResult(new { Error = "User ID could not be found in the JWT token!" }); }
            }
            catch (FormatException e) { return new JsonResult(new { Error = e.Message }); }

            try
            {
                int i = payload.PostID;
                if (i <= 0)
                {
                    return new JsonResult(new { Error = "Post's ID needs to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            if (String.IsNullOrEmpty(payload.Content))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            Post? existingPost = _db.Posts
                .Where(u => u.ID == payload.PostID)
                .SingleOrDefault();

            if (existingPost is null)
            {
                return new JsonResult(new { Error = "There is no post with the specified ID!" });
            }

            Post? ownedPost = _db.Posts
                .Where(u => u.UserID == userID && u.ID == payload.PostID)
                .SingleOrDefault();

            if (ownedPost is null)
            {
                return new JsonResult(new { Error = "You do not own the post!" });
            }


            if (ownedPost.Content.Equals(payload.Content))
            {
                return new JsonResult(new { Error = "There is no difference between the old post and the new post!" });
            }

            ownedPost.Content = payload.Content;

            _db.SaveChanges();

            Post? registeredPost = _db.Posts
                .Where(u => u.Content == payload.Content && u.UserID == userID && u.ID == payload.PostID)
                .SingleOrDefault();

            if (registeredPost is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You have successfully modified the post:", UserID = userID, payload.PostID, payload.Content });
            }
        }


        [HttpPost("ModifyShare")]//modify a share (its text)
        public ActionResult ModifyShare([FromBody] ModifyShareDTO payload)
        {
            System.Security.Claims.ClaimsPrincipal currentUser = HttpContext.User;
            int userID;

            try
            {
                if (currentUser.HasClaim(c => c.Type == "ID"))
                {
                    userID = int.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "ID").Value);
                }
                else { return new JsonResult(new { Error = "User ID could not be found in the JWT token!" }); }
            }
            catch (FormatException e) { return new JsonResult(new { Error = e.Message }); }

            try
            {
                int i = payload.ShareID;
                if (i <= 0)
                {
                    return new JsonResult(new { Error = "Share's ID needs to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            if (String.IsNullOrEmpty(payload.Content))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            Share? existingShare = _db.Shares
                .Where(u => u.ID == payload.ShareID)
                .SingleOrDefault();

            if (existingShare is null)
            {
                return new JsonResult(new { Error = "There is no share with the specified ID!" });
            }

            Share? ownedShare = _db.Shares
                .Where(u => u.UserID == userID && u.ID == payload.ShareID)
                .SingleOrDefault();

            if (ownedShare is null)
            {
                return new JsonResult(new { Error = "You do not own the share!" });
            }


            if (ownedShare.Content.Equals(payload.Content))
            {
                return new JsonResult(new { Error = "There is no difference between the old share and the new share!" });
            }

            ownedShare.Content = payload.Content;

            _db.SaveChanges();

            Share? registeredShare = _db.Shares
                .Where(u => u.Content == payload.Content && u.UserID == userID && u.ID == payload.ShareID)
                .SingleOrDefault();

            if (registeredShare is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You have successfully modified the share:", UserID = userID, payload.ShareID, payload.Content });
            }
        }


        [HttpPost("ModifyComment")]//modify a comment
        public ActionResult ModifyComment([FromBody] ModifyCommentDTO payload)
        {
            System.Security.Claims.ClaimsPrincipal currentUser = HttpContext.User;
            int userID;

            try
            {
                if (currentUser.HasClaim(c => c.Type == "ID"))
                {
                    userID = int.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "ID").Value);
                }
                else { return new JsonResult(new { Error = "User ID could not be found in the JWT token!" }); }
            }
            catch (FormatException e) { return new JsonResult(new { Error = e.Message }); }

            try
            {
                int i = payload.CommentID;
                if (i <= 0)
                {
                    return new JsonResult(new { Error = "Comment's ID needs to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            if (String.IsNullOrEmpty(payload.Content))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            Comment? existingComment = _db.Comments
                .Where(u => u.ID == payload.CommentID)
                .SingleOrDefault();

            if (existingComment is null)
            {
                return new JsonResult(new { Error = "There is no comment with the specified ID!" });
            }

            Comment? ownedComment = _db.Comments
                .Where(u => u.UserID == userID && u.ID == payload.CommentID)
                .SingleOrDefault();

            if (ownedComment is null)
            {
                return new JsonResult(new { Error = "You do not own the comment!" });
            }


            if (ownedComment.Content.Equals(payload.Content))
            {
                return new JsonResult(new { Error = "There is no difference between the old comment and the new comment!" });
            }

            ownedComment.Content = payload.Content;

            _db.SaveChanges();

            Comment? registeredComment = _db.Comments
                .Where(u => u.Content == payload.Content && u.UserID == userID && u.ID == payload.CommentID)
                .SingleOrDefault();

            if (registeredComment is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You have successfully modified the comment:", UserID = userID, payload.CommentID, payload.Content });
            }
        }


        [HttpPost("ModifyMessage")]//modify a message
        public ActionResult ModifyMessage([FromBody] ModifyMessageDTO payload)
        {
            System.Security.Claims.ClaimsPrincipal currentUser = HttpContext.User;
            int userID;

            try
            {
                if (currentUser.HasClaim(c => c.Type == "ID"))
                {
                    userID = int.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "ID").Value);
                }
                else { return new JsonResult(new { Error = "User ID could not be found in the JWT token!" }); }
            }
            catch (FormatException e) { return new JsonResult(new { Error = e.Message }); }

            try
            {
                int i = payload.MessageID;
                if (i <= 0)
                {
                    return new JsonResult(new { Error = "Message's ID needs to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            if (String.IsNullOrEmpty(payload.Content))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            Message? existingMessage = _db.Messages
                .Where(u => u.ID == payload.MessageID)
                .SingleOrDefault();

            if (existingMessage is null)
            {
                return new JsonResult(new { Error = "There is no message with the specified ID!" });
            }

            Message? ownedMessage = _db.Messages
                .Where(u => u.MessagerID == userID && u.ID == payload.MessageID)
                .SingleOrDefault();

            if (ownedMessage is null)
            {
                return new JsonResult(new { Error = "You do not own the message!" });
            }

            if (ownedMessage.Content.Equals(payload.Content))
            {
                return new JsonResult(new { Error = "There is no difference between the old message and the new message!" });
            }

            ownedMessage.Content = payload.Content;

            _db.SaveChanges();

            Message? registeredMessage = _db.Messages
                .Where(u => u.Content == payload.Content && u.MessagerID == userID && u.ID == payload.MessageID)
                .SingleOrDefault();

            if (registeredMessage is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You have successfully modified the message:", UserID = userID, payload.MessageID, payload.Content });
            }
        }
    }
}
