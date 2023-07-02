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
    public class SpecificUserPostInteractionController : ControllerBase
    {
        private readonly SocialMediaProjectDb _db;
        private readonly IConfiguration _config;

        public SpecificUserPostInteractionController(SocialMediaProjectDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("SpecificUserLikingPost")]//see if a specific user liked a specific post made by you
        public ActionResult SpecificUserLikingPost([FromBody] SearchUserPostDTO payload)
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
                int j = payload.PostID;
                if (i <= 0 || j <= 0)
                {
                    return new JsonResult(new { Error = "The IDs need to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            int? registeredUser = _db.Users
                .Where(u => u.ID == payload.UserID).Select(u => u.ID)
                .SingleOrDefault();

            int? registeredPost = _db.Posts
                .Where(u => u.ID == payload.PostID).Select(u => u.ID)
                .SingleOrDefault();

            int? postUser = _db.Posts
                .Where(u => u.ID == payload.PostID).Select(u => u.UserID)
                .SingleOrDefault();

            if (registeredUser is null || registeredUser == 0)
            {
                return new JsonResult(new { Error = "There is no user with the first specified ID!" });
            }

            if (registeredPost is null || registeredPost == 0)
            {
                return new JsonResult(new { Error = "There is no post with the second specified ID!" });
            }

            if (postUser != userID)
            {
                return new JsonResult(new { Error = "The specified post is not yours!" });
            }

            if (registeredUser == userID)
            {
                return new JsonResult(new { Error = "You can not check for owned likes on your own posts, as they are impossible!" });
            }

            Like? Likes = _db.Likes.Where(u => u.UserID == payload.UserID && u.PostID == payload.PostID).SingleOrDefault();

            if (Likes is null)
            {
                return new JsonResult(new { Error = "The user is not liking your post!" });
            }

            else
            {
                return new JsonResult(new { Message = "The user is liking your post!", Likes });
            }
        }


        [HttpPost("SpecificUserDislikingPost")]//see if a specific user disliked a specific post made by you
        public ActionResult SpecificUserDislikingPost([FromBody] SearchUserPostDTO payload)
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
                int j = payload.PostID;
                if (i <= 0 || j <= 0)
                {
                    return new JsonResult(new { Error = "The IDs need to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            int? registeredUser = _db.Users
                .Where(u => u.ID == payload.UserID).Select(u => u.ID)
                .SingleOrDefault();

            int? registeredPost = _db.Posts
                .Where(u => u.ID == payload.PostID).Select(u => u.ID)
                .SingleOrDefault();

            int? postUser = _db.Posts
                .Where(u => u.ID == payload.PostID).Select(u => u.UserID)
                .SingleOrDefault();

            if (registeredUser is null || registeredUser == 0)
            {
                return new JsonResult(new { Error = "There is no user with the first specified ID!" });
            }

            if (registeredPost is null || registeredPost == 0)
            {
                return new JsonResult(new { Error = "There is no post with the second specified ID!" });
            }

            if (postUser != userID)
            {
                return new JsonResult(new { Error = "The specified post is not yours!" });
            }

            if (registeredUser == userID)
            {
                return new JsonResult(new { Error = "You can not check for owned dislikes on your own posts, as they are impossible!" });
            }

            Dislike? Dislikes = _db.Dislikes.Where(u => u.UserID == payload.UserID && u.PostID == payload.PostID).SingleOrDefault();

            if (Dislikes is null)
            {
                return new JsonResult(new { Error = "The user is not disliking your post!" });
            }

            else
            {
                return new JsonResult(new { Message = "The user is disliking your post!", Dislikes });
            }
        }


        [HttpPost("SpecificUserSharingPost")]//see if a specific user shared a specific post made by you
        public ActionResult SpecificUserSharingPost([FromBody] SearchUserPostDTO payload)
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
                int j = payload.PostID;
                if (i <= 0 || j <= 0)
                {
                    return new JsonResult(new { Error = "The IDs need to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            int? registeredUser = _db.Users
                .Where(u => u.ID == payload.UserID).Select(u => u.ID)
                .SingleOrDefault();

            int? registeredPost = _db.Posts
                .Where(u => u.ID == payload.PostID).Select(u => u.ID)
                .SingleOrDefault();

            int? postUser = _db.Posts
                .Where(u => u.ID == payload.PostID).Select(u => u.UserID)
                .SingleOrDefault();

            if (registeredUser is null || registeredUser == 0)
            {
                return new JsonResult(new { Error = "There is no user with the first specified ID!" });
            }

            if (registeredPost is null || registeredPost == 0)
            {
                return new JsonResult(new { Error = "There is no post with the second specified ID!" });
            }

            if (postUser != userID)
            {
                return new JsonResult(new { Error = "The specified post is not yours!" });
            }

            if (registeredUser == userID)
            {
                return new JsonResult(new { Error = "You can not check for owned shares on your own posts, as they are impossible!" });
            }

            List<Share> Shares = _db.Shares.Where(u => u.UserID == payload.UserID && u.PostID == payload.PostID).OrderByDescending(u => u.Creation).ToList();

            if (Shares is null || Shares.Count == 0)
            {
                return new JsonResult(new { Error = "The user did not share your post!" });
            }

            else
            {
                return new JsonResult(new { Message = "The user shared your post!", count_shares = Shares.Count, Shares });
            }
        }


        [HttpPost("SpecificUserCommentingPost")]//see if a specific user commented on a specific post made by you
        public ActionResult SpecificUserCommentingPost([FromBody] SearchUserPostDTO payload)
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
                int j = payload.PostID;
                if (i <= 0 || j <= 0)
                {
                    return new JsonResult(new { Error = "The IDs need to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            int? registeredUser = _db.Users
                .Where(u => u.ID == payload.UserID).Select(u => u.ID)
                .SingleOrDefault();

            int? registeredPost = _db.Posts
                .Where(u => u.ID == payload.PostID).Select(u => u.ID)
                .SingleOrDefault();

            int? postUser = _db.Posts
                .Where(u => u.ID == payload.PostID).Select(u => u.UserID)
                .SingleOrDefault();

            if (registeredUser is null || registeredUser == 0)
            {
                return new JsonResult(new { Error = "There is no user with the first specified ID!" });
            }

            if (registeredPost is null || registeredPost == 0)
            {
                return new JsonResult(new { Error = "There is no post with the second specified ID!" });
            }
            
            if (postUser != userID)
            {
                return new JsonResult(new { Error = "The specified post is not yours!" });
            }

            List<Comment> Comments = _db.Comments.Where(u => u.UserID == payload.UserID && u.PostID == payload.PostID).OrderByDescending(u => u.Creation).ToList();

            if (Comments is null || Comments.Count == 0)
            {
                return new JsonResult(new { Error = "The user did not comment on your post!" });
            }

            else
            {
                return new JsonResult(new { Message = "The user commented on your post!", count_comments = Comments.Count, Comments });
            }
        }
    }
}
