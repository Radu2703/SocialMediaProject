using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaProject.Data;
using SocialMediaProject.Data.Entities;
using SocialMediaProject.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace SocialMediaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RemovalController : ControllerBase
    {
        private readonly SocialMediaProjectDb _db;
        private readonly IConfiguration _config;

        public RemovalController(SocialMediaProjectDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }


        [HttpPost("LikeRemoval")]//remove a like
        public ActionResult LikeRemoval([FromBody] LikeDTO payload)
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

            Post? registeredPost = _db.Posts
                .Where(u => u.ID == payload.PostID)
                .SingleOrDefault();

            if (registeredPost is null)
            {
                return new JsonResult(new { Error = "There is no post with the specified ID!" });
            }

            Like? existingLike = _db.Likes
                .Where(u => u.UserID == userID && u.PostID == payload.PostID)
                .SingleOrDefault();

            if (existingLike is null)
            {
                return new JsonResult(new { Error = "You have not given the post a like yet!" });
            }

            _db.Likes.Remove(existingLike);

            _db.SaveChanges();

            var registeredLike = _db.Likes
                .Where(u => u.PostID == payload.PostID && u.UserID == userID)
                .SingleOrDefault();

            if (registeredLike is not null)
            {
                return new JsonResult(new { Error = "Deletion failed!" });
            }
            else
            {
                return new JsonResult(new { Success = "Like deletion was successful!", UserID = userID, payload.PostID });
            }
        }


        [HttpPost("DislikeRemoval")]//remove a dislike
        public ActionResult DislikeRemoval([FromBody] DislikeDTO payload)
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

            Post? registeredPost = _db.Posts
                .Where(u => u.ID == payload.PostID)
                .SingleOrDefault();

            if (registeredPost is null)
            {
                return new JsonResult(new { Error = "There is no post with the specified ID!" });
            }

            Dislike? existingDislike = _db.Dislikes
                .Where(u => u.UserID == userID && u.PostID == payload.PostID)
                .SingleOrDefault();

            if (existingDislike is null)
            {
                return new JsonResult(new { Error = "You have not given the post a dislike yet!" });
            }

            _db.Dislikes.Remove(existingDislike);

            _db.SaveChanges();

            var registeredDislike = _db.Dislikes
                .Where(u => u.PostID == payload.PostID && u.UserID == userID)
                .SingleOrDefault();

            if (registeredDislike is not null)
            {
                return new JsonResult(new { Error = "Deletion failed!" });
            }
            else
            {
                return new JsonResult(new { Success = "Dislike deletion was successful!", UserID = userID, payload.PostID });
            }
        }


        [HttpPost("ShareRemoval")]//remove a share
        public ActionResult ShareRemoval([FromBody] RemoveShareDTO payload)
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

            _db.Shares.Remove(ownedShare);

            _db.SaveChanges();

            Share? registeredShare = _db.Shares
                .Where(u => u.UserID == userID && u.ID == payload.ShareID)
                .SingleOrDefault();

            if (registeredShare is not null)
            {
                return new JsonResult(new { Error = "Deletion failed!" });
            }
            else
            {
                return new JsonResult(new { Success = "Share deletion was successful!", UserID = userID, payload.ShareID });
            }
        }


        [HttpPost("SharePostRemoval")]//remove a share of one of your own posts
        public ActionResult SharePostRemoval([FromBody] RemoveSharePostDTO payload)
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
                int p = payload.PostID;
                if (i <= 0 || p <= 0)
                {
                    return new JsonResult(new { Error = "Share's ID and post's ID needs to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

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

            Share? existingShare = _db.Shares
                .Where(u => u.ID == payload.ShareID)
                .SingleOrDefault();

            if (existingShare is null)
            {
                return new JsonResult(new { Error = "There is no share with the specified ID!" });
            }

            Share? ownedShare = _db.Shares
                .Where(u => u.PostID == payload.PostID && u.ID == payload.ShareID)
                .SingleOrDefault();

            if (ownedShare is null)
            {
                return new JsonResult(new { Error = "The share is not associated with the post!" });
            }

            _db.Shares.Remove(ownedShare);

            _db.SaveChanges();

            Share? registeredShare = _db.Shares
                .Where(u => u.PostID == payload.PostID && u.ID == payload.ShareID)
                .SingleOrDefault();

            if (registeredShare is not null)
            {
                return new JsonResult(new { Error = "Deletion failed!" });
            }
            else
            {
                return new JsonResult(new { Success = "Post share deletion was successful!", payload.PostID, payload.ShareID });
            }
        }


        [HttpPost("CommentPostRemoval")]//remove a comment on one of your own posts
        public ActionResult CommentPostRemoval([FromBody] RemoveCommentPostDTO payload)
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
                int p = payload.PostID;
                if (i <= 0 || p <= 0)
                {
                    return new JsonResult(new { Error = "Comment's ID and post's ID needs to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

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

            Comment? existingComment = _db.Comments
                .Where(u => u.ID == payload.CommentID)
                .SingleOrDefault();

            if (existingComment is null)
            {
                return new JsonResult(new { Error = "There is no comment with the specified ID!" });
            }

            Comment? ownedComment = _db.Comments
                .Where(u => u.PostID == payload.PostID && u.ID == payload.CommentID)
                .SingleOrDefault();

            if (ownedComment is null)
            {
                return new JsonResult(new { Error = "The comment is not associated with the post!" });
            }

            _db.Comments.Remove(ownedComment);

            _db.SaveChanges();

            Comment? registeredComment = _db.Comments
                .Where(u => u.PostID == payload.PostID && u.ID == payload.CommentID)
                .SingleOrDefault();

            if (registeredComment is not null)
            {
                return new JsonResult(new { Error = "Deletion failed!" });
            }
            else
            {
                return new JsonResult(new { Success = "Post comment deletion was successful!", payload.PostID, payload.CommentID });
            }
        }


        [HttpPost("CommentRemoval")]//remove a comment
        public ActionResult CommentRemoval([FromBody] RemoveCommentDTO payload)
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

            _db.Comments.Remove(ownedComment);

            _db.SaveChanges();

            Comment? registeredComment = _db.Comments
                .Where(u => u.UserID == userID && u.ID == payload.CommentID)
                .SingleOrDefault();

            if (registeredComment is not null)
            {
                return new JsonResult(new { Error = "Deletion failed!" });
            }
            else
            {
                return new JsonResult(new { Success = "Comment deletion was successful!", UserID = userID, payload.CommentID });
            }
        }


        [HttpPost("PostRemoval")]//remove a post
        public ActionResult PostRemoval([FromBody] RemovePostDTO payload)
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

            _db.Posts.Remove(ownedPost);

            _db.SaveChanges();

            Post? registeredPost = _db.Posts
                .Where(u => u.UserID == userID && u.ID == payload.PostID)
                .SingleOrDefault();

            if (registeredPost is not null)
            {
                return new JsonResult(new { Error = "Deletion failed!" });
            }
            else
            {
                return new JsonResult(new { Success = "Post deletion was successful!", UserID = userID, payload.PostID });
            }
        }


        [HttpPost("BlockRemoval")]//remove a block
        public ActionResult BlockRemoval([FromBody] BlockDTO payload)
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
                int i = payload.UserID;
                if (i <= 0)
                {
                    return new JsonResult(new { Error = "Block's ID needs to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            User? registeredUser = _db.Users
                .Where(u => u.ID == payload.UserID)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return new JsonResult(new { Error = "There is no user with the specified ID!" });
            }

            Block? existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock is null)
            {
                return new JsonResult(new { Error = "You have not blocked this user yet!" });
            }

            _db.Blocks.Remove(existingBlock);

            _db.SaveChanges();

            Block? registeredBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID)
                .SingleOrDefault();

            if (registeredBlock is not null)
            {
                return new JsonResult(new { Error = "Deletion failed!" });
            }
            else
            {
                return new JsonResult(new { Success = "Block deletion was successful!", BlockerID = userID, BlockedID = payload.UserID });
            }
        }


        [HttpPost("FollowRemoval")]//remove a follow
        public ActionResult FollowRemoval([FromBody] FollowDTO payload)
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
                int i = payload.UserID;
                if (i <= 0)
                {
                    return new JsonResult(new { Error = "Follow's ID needs to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            User? registeredUser = _db.Users
                .Where(u => u.ID == payload.UserID)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return new JsonResult(new { Error = "There is no user with the specified ID!" });
            }

            Follow? existingFollow = _db.Follows
                .Where(u => u.FollowerID == userID && u.FollowedID == payload.UserID)
                .SingleOrDefault();

            if (existingFollow is null)
            {
                return new JsonResult(new { Error = "You are not following this user yet!" });
            }

            _db.Follows.Remove(existingFollow);

            _db.SaveChanges();

            Follow? registeredFollow = _db.Follows
                .Where(u => u.FollowerID == userID && u.FollowedID == payload.UserID)
                .SingleOrDefault();

            if (registeredFollow is not null)
            {
                return new JsonResult(new { Error = "Deletion failed!" });
            }
            else
            {
                return new JsonResult(new { Success = "Follow deletion was successful!", FollowerID = userID, FollowedID = payload.UserID });
            }
        }


        [HttpPost("MessageRemoval")]//remove a message
        public ActionResult MessageRemoval([FromBody] RemoveMessageDTO payload)
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

            _db.Messages.Remove(ownedMessage);

            _db.SaveChanges();

            Message? registeredMessage = _db.Messages
                .Where(u => u.MessagerID == userID && u.ID == payload.MessageID)
                .SingleOrDefault();

            if (registeredMessage is not null)
            {
                return new JsonResult(new { Error = "Deletion failed!" });
            }
            else
            {
                return new JsonResult(new { Success = "Message deletion was successful!", UserID = userID, payload.MessageID });
            }
        }


        [HttpPost("MessageUserRemoval")]//remove a message you received
        public ActionResult MessageUserRemoval([FromBody] RemoveMessageUserDTO payload)
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

            Message? existingMessage = _db.Messages
                .Where(u => u.ID == payload.MessageID)
                .SingleOrDefault();

            if (existingMessage is null)
            {
                return new JsonResult(new { Error = "There is no message with the specified ID!" });
            }

            Message? ownedMessage = _db.Messages
                .Where(u => u.MessagedID == userID && u.ID == payload.MessageID)
                .SingleOrDefault();

            if (ownedMessage is null)
            {
                return new JsonResult(new { Error = "The message was not sent to you!" });
            }

            _db.Messages.Remove(ownedMessage);

            _db.SaveChanges();

            Message? registeredMessage = _db.Messages
                .Where(u => u.MessagedID == userID && u.ID == payload.MessageID)
                .SingleOrDefault();

            if (registeredMessage is not null)
            {
                return new JsonResult(new { Error = "Deletion failed!" });
            }
            else
            {
                return new JsonResult(new { Success = "User message deletion was successful!", MessagedID = userID, payload.MessageID });
            }
        }


        [HttpPost("UserRemoval")]//remove your account
        public ActionResult UserRemoval([FromBody] PasswordDTO payload)
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

            User? ownedUser = _db.Users
                .Where(u => u.ID == userID)
                .SingleOrDefault();

            if (ownedUser is null)
                return new JsonResult(new { Error = "Your account could not be found at the moment!" });

            string base64HashedPasswordBytes;
            byte[] passwordBytes = Encoding.UTF8.GetBytes(payload.password);
            byte[] hashedPasswordBytes = SHA256.HashData(passwordBytes);
            base64HashedPasswordBytes = Convert.ToBase64String(hashedPasswordBytes);

            if (!ownedUser.HashedPassword.Equals(base64HashedPasswordBytes))
                return new JsonResult(new { Error = "Password is incorrect!" });

            _db.Users.Remove(ownedUser);

            List<Follow> Follows = _db.Follows.Where(u => u.FollowedID == userID).ToList();
            foreach (Follow f in Follows)
                    { _db.Follows.Remove(f); }

            List<Block> Blocks = _db.Blocks.Where(u => u.BlockedID == userID).ToList();
            foreach (Block f in Blocks)
                    { _db.Blocks.Remove(f); }

            List<Message> Messages = _db.Messages.Where(u => u.MessagedID == userID).ToList();
            foreach (Message f in Messages)
                    { _db.Messages.Remove(f); }

            List<Like> Likes = _db.Likes.Where(u => u.UserID == userID).ToList();
            foreach (Like f in Likes)
                    { _db.Likes.Remove(f); }

            List<Dislike> Dislikes = _db.Dislikes.Where(u => u.UserID == userID).ToList();
            foreach (Dislike f in Dislikes)
                    { _db.Dislikes.Remove(f); }

            List<Share> Shares = _db.Shares.Where(u => u.UserID == userID).ToList();
            foreach (Share f in Shares)
                    { _db.Shares.Remove(f); }

            List<Comment> Comments = _db.Comments.Where(u => u.UserID == userID).ToList();
            foreach (Comment f in Comments)
                    { _db.Comments.Remove(f); }

            _db.SaveChanges();

            User? registeredUser = _db.Users
                .Where(u => u.ID == userID)
                .SingleOrDefault();

            if (registeredUser is not null)
            {
                return new JsonResult(new { Error = "Deletion failed!" });
            }
            else
            {
                return new JsonResult(new { Success = "Your account was successfully deleted!" });
            }
        }
    }
}
