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
    public class SpecificPostHistoryController : ControllerBase
    {
        private readonly SocialMediaProjectDb _db;
        private readonly IConfiguration _config;

        public SpecificPostHistoryController(SocialMediaProjectDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }


        [HttpPost("SharerPostInfo")]//see if you shared a specific post
        public ActionResult SharerPostInfo([FromBody] SpecificPostInfoDTO payload)
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

            if (ownedPost > 0)
            {
                return new JsonResult(new { Error = "You own the post!" });
            }

            List<Share> Shares = _db.Shares.Where(u => u.PostID == payload.PostID && u.UserID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Shares is null || Shares.Count == 0)
            {
                return new JsonResult(new { Error = "You did not share the post yet!" });
            }
            else
            {
                return new JsonResult(new { Shares_sent = Shares.Count, Shares });
            }
        }


        [HttpPost("CommenterPostInfo")]//see if you commented on a specific post
        public ActionResult CommenterPostInfo([FromBody] SpecificPostInfoDTO payload)
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

            List<Comment> Comments = _db.Comments.Where(u => u.PostID == payload.PostID && u.UserID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Comments is null || Comments.Count == 0)
            {
                return new JsonResult(new { Error = "You did not comment on the post yet!" });
            }
            else
            {
                return new JsonResult(new { Comments_sent = Comments.Count, Comments });
            }
        }


        [HttpPost("LikerPostInfo")]//see if you liked a specific post
        public ActionResult LikerPostInfo([FromBody] SpecificPostInfoDTO payload)
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

            if (ownedPost > 0)
            {
                return new JsonResult(new { Error = "You own the post!" });
            }

            List<Like> Likes = _db.Likes.Where(u => u.PostID == payload.PostID && u.UserID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Likes is null || Likes.Count == 0)
            {
                return new JsonResult(new { Error = "You did not like the post yet!" });
            }
            else
            {
                return new JsonResult(new { Likes_sent = Likes.Count, Likes });
            }
        }


        [HttpPost("DislikerPostInfo")]//see if you disliked a specific post
        public ActionResult DislikerPostInfo([FromBody] SpecificPostInfoDTO payload)
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

            if (ownedPost > 0)
            {
                return new JsonResult(new { Error = "You own the post!" });
            }

            List<Dislike> Dislikes = _db.Dislikes.Where(u => u.PostID == payload.PostID && u.UserID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Dislikes is null || Dislikes.Count == 0)
            {
                return new JsonResult(new { Error = "You did not dislike the post yet!" });
            }
            else
            {
                return new JsonResult(new { Dislikes_sent = Dislikes.Count, Dislikes });
            }
        }
    }
}
