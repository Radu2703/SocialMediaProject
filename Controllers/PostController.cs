using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMediaProject.Data;
using SocialMediaProject.Data.Entities;
using SocialMediaProject.DTOs;
using System.Text;

namespace SocialMediaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly SocialMediaProjectDb _db;
        private readonly IConfiguration _config;

        public PostController(SocialMediaProjectDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("CreatePost")]//create a post
        public ActionResult CreatePost([FromBody] CreatePostDTO payload)
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

            if (String.IsNullOrEmpty(payload.Content))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            DateTime ct = DateTime.Now;

            Post? existingPost = _db.Posts
                .Where(u => u.UserID == userID && u.Creation == ct)
                .SingleOrDefault();

            if (existingPost is not null)
            {
                return new JsonResult(new { Error = "Do not make more than one post at the same time!" });
            }

            _db.Posts.Add(new Data.Entities.Post
            {
                UserID = userID,
                Content = payload.Content,
                Creation = ct
            });

            _db.SaveChanges();


            Post? registeredPost = _db.Posts
                .Where(u => u.Content == payload.Content && u.UserID == userID && u.Creation == ct)
                .SingleOrDefault();

            if (registeredPost is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You created the following post:", UserID = userID, payload.Content, Creation = ct });
            }
        }


        [HttpPost("LikePost")]//like a post
        public ActionResult LikePost([FromBody] LikeDTO payload)
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
                if(i<=0)
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

            if (registeredPost.UserID == userID)
            {
                return new JsonResult(new { Error = "You can not give a like to your own posts!" });
            }

            Like? existingLike = _db.Likes
                .Where(u => u.UserID == userID && u.PostID == payload.PostID)
                .SingleOrDefault();

            if (existingLike is not null)
            {
                return new JsonResult(new { Error = "You have already given the post a like!" });
            }

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == registeredPost.UserID)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not like this post, as you are blocked by its user!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == registeredPost.UserID)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not like this post, as you are blocking its user!" });
            }

            DateTime ct = DateTime.Now;

            _db.Likes.Add(new Data.Entities.Like
            {
                UserID = userID,
                PostID = payload.PostID,
                Creation = ct
            });

            Dislike? existingDislike = _db.Dislikes
                .Where(u => u.UserID == userID && u.PostID == payload.PostID)
                .SingleOrDefault();

            if (existingDislike is not null)
            {
                _db.Dislikes.Remove(existingDislike);
            }

            _db.SaveChanges();

            var registeredLike = _db.Likes
                .Where(u => u.PostID == payload.PostID && u.UserID == userID && u.Creation == ct)
                .SingleOrDefault();

            if (registeredLike is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You created the following like:", UserID = userID , payload.PostID , Creation = ct}) ;
            }
        }

        [HttpPost("DislikePost")]//dislike a post
        public ActionResult DislikePost([FromBody] DislikeDTO payload)
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

            var registeredPost = _db.Posts
                .Where(u => u.ID == payload.PostID)
                .SingleOrDefault();

            if (registeredPost is null)
            {
                return new JsonResult(new { Error = "There is no post with the specified ID!" });
            }

            if (registeredPost.UserID == userID)
            {
                return new JsonResult(new { Error = "You can not give a dislike to your own posts!" });
            }

            Dislike? existingDislike = _db.Dislikes
                .Where(u => u.UserID == userID && u.PostID == payload.PostID)
                .SingleOrDefault();

            if (existingDislike is not null)
            {
                return new JsonResult(new { Error = "You have already given the post a dislike!" });
            }

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == registeredPost.UserID)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not dislike this post, as you are blocked by its user!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == registeredPost.UserID)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not dislike this post, as you are blocking its user!" });
            }

            DateTime ct = DateTime.Now;

            _db.Dislikes.Add(new Data.Entities.Dislike
            {
                UserID = userID,
                PostID = payload.PostID,
                Creation = ct
            });

            Like? existingLike = _db.Likes
                .Where(u => u.UserID == userID && u.PostID == payload.PostID)
                .SingleOrDefault();

            if (existingLike is not null)
            {
                _db.Likes.Remove(existingLike);
            }

            _db.SaveChanges();

            var registeredDislike = _db.Dislikes
                .Where(u => u.PostID == payload.PostID && u.UserID == userID && u.Creation == ct)
                .SingleOrDefault();

            if (registeredDislike is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You created the following dislike:", UserID = userID, payload.PostID, Creation = ct });
            }
        }

        [HttpPost("SharePost")]//share a post
        public ActionResult SharePost([FromBody] CreateShareDTO payload)
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

            var registeredPost = _db.Posts
                .Where(u => u.ID == payload.PostID)
                .SingleOrDefault();

            if (registeredPost is null)
            {
                return new JsonResult(new { Error = "There is no post with the specified ID!" });
            }

            if (registeredPost.UserID == userID)
            {
                return new JsonResult(new { Error = "You can not share one of your own posts!" });
            }

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == registeredPost.UserID)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not share this post, as you are blocked by its user!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == registeredPost.UserID)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not share this post, as you are blocking its user!" });
            }

            DateTime ct = DateTime.Now;

            Share? existingShare = _db.Shares
                .Where(u => u.UserID == userID && u.Creation == ct)
                .SingleOrDefault();

            if (existingShare is not null)
            {
                return new JsonResult(new { Error = "Do not share more than once at the same time!" });
            }

            _db.Shares.Add(new Data.Entities.Share
            {
                UserID = userID,
                PostID = payload.PostID,
                Content = payload.Content,
                Creation = ct
            });

            _db.SaveChanges();


            var registeredShare = _db.Shares
                .Where(u => u.Content == payload.Content && u.PostID == payload.PostID && u.UserID == userID && u.Creation == ct)
                .SingleOrDefault();

            if (registeredShare is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You created the following share:", UserID = userID, payload.PostID, payload.Content, Creation = ct });
            }
        }

        [HttpPost("CommentPost")]//comment on a post
        public ActionResult CommentPost([FromBody] CreateCommentDTO payload)
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

            Post? registeredPost = _db.Posts
                .Where(u => u.ID == payload.PostID)
                .SingleOrDefault();

            if (registeredPost is null)
            {
                return new JsonResult(new { Error = "There is no post with the specified ID!" });
            }

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == registeredPost.UserID)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not comment on this post, as you are blocked by its user!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == registeredPost.UserID)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not comment on this post, as you are blocking its user!" });
            }

            DateTime ct = DateTime.Now;

            Comment? existingComment = _db.Comments
                .Where(u => u.UserID == userID && u.Creation == ct)
                .SingleOrDefault();

            if (existingComment is not null)
            {
                return new JsonResult(new { Error = "Do not comment more than once at the same time!" });
            }

            _db.Comments.Add(new Data.Entities.Comment
            {
                UserID = userID,
                PostID = payload.PostID,
                Content = payload.Content,
                Creation = ct
            });

            _db.SaveChanges();


            var registeredComment = _db.Comments
                .Where(u => u.Content == payload.Content && u.PostID == payload.PostID && u.UserID == userID && u.Creation == ct)
                .SingleOrDefault();

            if (registeredComment is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You created the following comment:", UserID = userID, payload.PostID, payload.Content, Creation = ct });
            }
        }
    }
}
