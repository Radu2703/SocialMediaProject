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
    public class SpecificPostPopularityController : ControllerBase
    {
        private readonly SocialMediaProjectDb _db;
        private readonly IConfiguration _config;

        public SpecificPostPopularityController(SocialMediaProjectDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }


        [HttpPost("SharedPostInfo")]//see your post's shares
        public ActionResult SharedPostInfo([FromBody] SpecificPostInfoDTO payload)
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

            int? registeredPost = _db.Posts
                .Where(u => u.ID == payload.PostID).Select(u => u.ID)
                .SingleOrDefault();

            if (registeredPost is null || registeredPost == 0)
            {
                return new JsonResult(new { Error = "There is no post with the specified ID!" });
            }

            int? ownedPost = _db.Posts
                .Where(u => u.ID == payload.PostID && u.UserID == userID).Select(u => u.ID)
                .SingleOrDefault();

            if (ownedPost is null || ownedPost == 0)
            {
                return new JsonResult(new { Error = "You do not own the post!" });
            }

            List<Share> Shares = _db.Shares.Where(u => u.PostID == payload.PostID).OrderByDescending(u => u.Creation).ToList();
            
            if (Shares is null || Shares.Count == 0)
            {
                return new JsonResult(new { Error = "The post has not been shared by anyone yet!" });
            }
            else
            {
                return new JsonResult(new { Shares_received = Shares.Count, Shares });
            }
        }


        [HttpPost("CommentedPostInfo")]//see your post's comments
        public ActionResult CommentedPostInfo([FromBody] SpecificPostInfoDTO payload)
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

            int? registeredPost = _db.Posts
                .Where(u => u.ID == payload.PostID).Select(u => u.ID)
                .SingleOrDefault();

            if (registeredPost is null || registeredPost == 0)
            {
                return new JsonResult(new { Error = "There is no post with the specified ID!" });
            }

            int? ownedPost = _db.Posts
                .Where(u => u.ID == payload.PostID && u.UserID == userID).Select(u => u.ID)
                .SingleOrDefault();

            if (ownedPost is null || ownedPost == 0)
            {
                return new JsonResult(new { Error = "You do not own the post!" });
            }

            List<Comment> Comments = _db.Comments.Where(u => u.PostID == payload.PostID).OrderByDescending(u => u.Creation).ToList();
            
            if (Comments is null || Comments.Count == 0)
            {
                return new JsonResult(new { Error = "The post has not been commented on by anyone yet!" });
            }
            else
            {
                return new JsonResult(new { Comments_received = Comments.Count, Comments });
            }
        }


        [HttpPost("LikedPostInfo")]//see your post's likes
        public ActionResult LikedPostInfo([FromBody] SpecificPostInfoDTO payload)
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

            int? registeredPost = _db.Posts
                .Where(u => u.ID == payload.PostID).Select(u => u.ID)
                .SingleOrDefault();

            if (registeredPost is null || registeredPost == 0)
            {
                return new JsonResult(new { Error = "There is no post with the specified ID!" });
            }

            int? ownedPost = _db.Posts
                .Where(u => u.ID == payload.PostID && u.UserID == userID).Select(u => u.ID)
                .SingleOrDefault();

            if (ownedPost is null || ownedPost == 0)
            {
                return new JsonResult(new { Error = "You do not own the post!" });
            }

            List<Like> Likes = _db.Likes.Where(u => u.PostID == payload.PostID).OrderByDescending(u => u.Creation).ToList();
            
            if (Likes is null || Likes.Count == 0)
            {
                return new JsonResult(new { Error = "The post has not been liked by anyone yet!" });
            }
            else
            {
                return new JsonResult(new { Likes_received = Likes.Count, Likes });
            }
        }


        [HttpPost("DislikedPostInfo")]//see your post's dislikes
        public ActionResult DislikedPostInfo([FromBody] SpecificPostInfoDTO payload)
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

            int? registeredPost = _db.Posts
                .Where(u => u.ID == payload.PostID).Select(u => u.ID)
                .SingleOrDefault();

            if (registeredPost is null || registeredPost == 0)
            {
                return new JsonResult(new { Error = "There is no post with the specified ID!" });
            }

            int? ownedPost = _db.Posts
                .Where(u => u.ID == payload.PostID && u.UserID == userID).Select(u => u.ID)
                .SingleOrDefault();

            if (ownedPost is null || ownedPost == 0)
            {
                return new JsonResult(new { Error = "You do not own the post!" });
            }

            List<Dislike> Dislikes = _db.Dislikes.Where(u => u.PostID == payload.PostID).OrderByDescending(u => u.Creation).ToList();
            
            if (Dislikes is null || Dislikes.Count == 0)
            {
                return new JsonResult(new { Error = "The post has not been disliked by anyone yet!" });
            }
            else
            {
                return new JsonResult(new { Dislikes_received = Dislikes.Count, Dislikes });
            }
        }
    }
}
