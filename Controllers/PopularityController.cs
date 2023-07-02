using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMediaProject.Data;
using SocialMediaProject.Data.Entities;
using SocialMediaProject.DTOs;
using System.Collections.Immutable;
using static System.Reflection.Metadata.BlobBuilder;

namespace SocialMediaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PopularityController : ControllerBase
    {
        private readonly SocialMediaProjectDb _db;
        private readonly IConfiguration _config;

        public PopularityController(SocialMediaProjectDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }


        [HttpGet("BlockedInfo")]//see every person who blocked you
        public ActionResult BlockedInfo()
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

            List<Block> Blocks = _db.Blocks.Where(u => u.BlockedID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Blocks is null || Blocks.Count == 0)
            {
                return new JsonResult(new { Error = "You have not been blocked by anyone yet!" });
            }
            else
            {
                return new JsonResult(new { Blocks_received = Blocks.Count, Blocks });
            }
        }


        [HttpGet("FollowedInfo")]//see every person who is following you
        public ActionResult FollowedInfo()
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

            List<Follow> Follows = _db.Follows.Where(u => u.FollowedID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Follows is null || Follows.Count == 0)
            {
                return new JsonResult(new { Error = "You are not followed by anyone yet!" });
            }
            else
            {
                return new JsonResult(new { Follows_received = Follows.Count, Follows });
            }
        }


        [HttpGet("MessagedInfo")]//see every message you received
        public ActionResult MessagedInfo()
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

            List<Message> Messages = _db.Messages.Where(u => u.MessagedID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Messages is null || Messages.Count == 0)
            {
                return new JsonResult(new { Error = "You have not been messaged by anyone yet!" });
            }
            else
            {
                return new JsonResult(new { Messages_received = Messages.Count, Messages });
            }
        }


        [HttpGet("LikedInfo")]//see every like you received
        public ActionResult LikedInfo()
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

            List<int> ids = _db.Posts.Where(u => u.UserID == userID).Select(u => u.ID).ToList();

            List<Like> Likes = _db.Likes.Where(u => ids.Contains(u.PostID)).OrderByDescending(u => u.Creation).ToList();
            
            if (Likes is null || Likes.Count == 0)
            {
                return new JsonResult(new { Error = "You have not been liked by anyone yet!" });
            }
            else
            {
                return new JsonResult(new { Likes_received = Likes.Count, Likes });
            }
        }


        [HttpGet("DislikedInfo")]//see every dislike you received
        public ActionResult DislikedInfo()
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

            List<int> ids = _db.Posts.Where(u => u.UserID == userID).Select(u => u.ID).ToList();

            List<Dislike> Dislikes = _db.Dislikes.Where(u => ids.Contains(u.PostID)).OrderByDescending(u => u.Creation).ToList();
            
            if (Dislikes is null || Dislikes.Count == 0)
            {
                return new JsonResult(new { Error = "You have not been disliked by anyone yet!" });
            }
            else
            {
                return new JsonResult(new { Dislikes_received = Dislikes.Count, Dislikes });
            }
        }


        [HttpGet("SharedInfo")]//see every share you received
        public ActionResult SharedInfo()
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

            List<int> ids = _db.Posts.Where(u => u.UserID == userID).Select(u => u.ID).ToList();

            List<Share> Shares = _db.Shares.Where(u => ids.Contains(u.PostID)).OrderByDescending(u => u.Creation).ToList();
            
            if (Shares is null || Shares.Count == 0)
            {
                return new JsonResult(new { Error = "Your posts have not been shared by anyone yet!" });
            }
            else
            {
                return new JsonResult(new { Shares_received = Shares.Count, Shares });
            }
        }


        [HttpGet("CommentedInfo")]//see every comment you received
        public ActionResult CommentedInfo()
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

            List<int> ids = _db.Posts.Where(u => u.UserID == userID).Select(u => u.ID).ToList();

            List<Comment> Comments = _db.Comments.Where(u => ids.Contains(u.PostID)).OrderByDescending(u => u.Creation).ToList();
            
            if (Comments is null || Comments.Count == 0)
            {
                return new JsonResult(new { Error = "Your posts have not been commented on by anyone yet!" });
            }
            else
            {
                return new JsonResult(new { Comments_received = Comments.Count, Comments });
            }
        }
    }
}
