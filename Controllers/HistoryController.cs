using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMediaProject.Data;
using SocialMediaProject.Data.Entities;
using System.Security.Cryptography;
using System.Text;
using static System.Reflection.Metadata.BlobBuilder;

namespace SocialMediaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HistoryController : ControllerBase
    {
        private readonly SocialMediaProjectDb _db;
        private readonly IConfiguration _config;

        public HistoryController(SocialMediaProjectDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }


        [HttpGet("UserInfo")]//see your account info
        public ActionResult UserInfo()
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

            User? registeredUser = _db.Users
                .Where(u => u.ID == userID)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return new JsonResult(new { Error = "Your account could not be found at the moment!" });
            }
            else
            {
                return new JsonResult(new {
                    Message = "Account info:",
                    registeredUser.ID, registeredUser.Username, registeredUser.Email, registeredUser.PhoneNumber, registeredUser.Description, 
                    registeredUser.Name, registeredUser.Country, registeredUser.City, registeredUser.Gender, birthdate = DateOnly.FromDateTime(registeredUser.Birthdate), registeredUser.Creation});
            }
        }


        [HttpGet("BlockerInfo")]//see everyone you blocked
        public ActionResult BlockerInfo()
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

            List<Block> Blocks = _db.Blocks.Where(u => u.BlockerID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Blocks is null || Blocks.Count==0)
            {
                return new JsonResult(new { Error = "You have not blocked anyone yet!" });
            }
            else
            {
                return new JsonResult(new { Blocks_sent = Blocks.Count, Blocks });
            }
        }


        [HttpGet("FollowerInfo")]//see everyone you are following
        public ActionResult FollowerInfo()
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

            List<Follow> Follows = _db.Follows.Where(u => u.FollowerID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Follows is null || Follows.Count == 0)
            {
                return new JsonResult(new { Error = "You are not following anyone yet!" });
            }
            else
            {
                return new JsonResult(new { Follows_sent = Follows.Count, Follows });
            }
        }


        [HttpGet("MessagerInfo")]//see every message you sent
        public ActionResult MessagerInfo()
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

            List<Message> Messages = _db.Messages.Where(u => u.MessagerID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Messages is null || Messages.Count == 0)
            {
                return new JsonResult(new { Error = "You have not messaged anyone yet!" });
            }
            else
            {
                return new JsonResult(new { Messages_sent = Messages.Count, Messages });
            }
        }


        [HttpGet("PostsInfo")]//see every post you made
        public ActionResult PostsInfo()
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

            List<Post> Posts = _db.Posts.Where(u => u.UserID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Posts is null || Posts.Count == 0)
            {
                return new JsonResult(new { Error = "You have not posted anything yet!" });
            }
            else
            {
                return new JsonResult(new { Number_posts = Posts.Count, Posts });
            }
        }


        [HttpGet("LikerInfo")]//see every like you gave
        public ActionResult LikerInfo()
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

            List<Like> Likes = _db.Likes.Where(u => u.UserID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Likes is null || Likes.Count == 0)
            {
                return new JsonResult(new { Error = "You have not liked anything yet!" });
            }
            else
            {
                return new JsonResult(new { Likes_sent = Likes.Count, Likes });
            }
        }


        [HttpGet("DislikerInfo")]//see every dislike you gave
        public ActionResult DislikerInfo()
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

            List<Dislike> Dislikes = _db.Dislikes.Where(u => u.UserID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Dislikes is null || Dislikes.Count == 0)
            {
                return new JsonResult(new { Error = "You have not disliked anything yet!" });
            }
            else
            {
                return new JsonResult(new { Dislikes_sent = Dislikes.Count, Dislikes });
            }
        }


        [HttpGet("SharerInfo")]//see every share you made
        public ActionResult SharerInfo()
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

            List<Share> Shares = _db.Shares.Where(u => u.UserID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Shares is null || Shares.Count == 0)
            {
                return new JsonResult(new { Error = "You have not shared anything yet!" });
            }
            else
            {
                return new JsonResult(new { Shares_sent = Shares.Count, Shares });
            }
        }


        [HttpGet("CommenterInfo")]//see every comment you made
        public ActionResult CommenterInfo()
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

            List<Comment> Comments = _db.Comments.Where(u => u.UserID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Comments is null || Comments.Count == 0)
            {
                return new JsonResult(new { Error = "You have not commented on anything anything yet!" });
            }
            else
            {
                return new JsonResult(new { Comments_sent = Comments.Count, Comments });
            }
        }
    }
}
