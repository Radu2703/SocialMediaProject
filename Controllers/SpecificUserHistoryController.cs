using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SocialMediaProject.Data;
using SocialMediaProject.Data.Entities;
using SocialMediaProject.DTOs;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SocialMediaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SpecificUserHistoryController : ControllerBase
    {
        private readonly SocialMediaProjectDb _db;
        private readonly IConfiguration _config;

        public SpecificUserHistoryController(SocialMediaProjectDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("MessagerUserInfo")]//see your messages to a specific user
        public ActionResult MessagerUserInfo([FromBody] SpecificUserInfoDTO payload)
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
                    return new JsonResult(new { Error = "User's ID needs to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            int? registeredUser = _db.Users
                .Where(u => u.ID == payload.UserID).Select(u => u.ID)
                .SingleOrDefault();

            if (registeredUser is null || registeredUser == 0)
            {
                return new JsonResult(new { Error = "There is no user with the specified ID!" });
            }

            List<Message> Messages = _db.Messages.Where(u => u.MessagerID == userID && u.MessagedID == registeredUser).OrderByDescending(u => u.Creation).ToList();
            
            if (Messages is null || Messages.Count == 0)
            {
                return new JsonResult(new { Error = "You have not messaged this user yet!" });
            }
            else
            {
                return new JsonResult(new { Messages_sent = Messages.Count, Messages });
            }
        }


        [HttpPost("BlockerUserInfo")]//see if you blocked a specific user
        public ActionResult BlockerUserInfo([FromBody] SpecificUserInfoDTO payload)
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
                    return new JsonResult(new { Error = "User's ID needs to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            int? registeredUser = _db.Users
                .Where(u => u.ID == payload.UserID).Select(u => u.ID)
                .SingleOrDefault();

            if (registeredUser is null || registeredUser == 0)
            {
                return new JsonResult(new { Error = "There is no user with the specified ID!" });
            }

            List<Block> Blocks = _db.Blocks.Where(u => u.BlockerID == userID && u.BlockedID == registeredUser).OrderByDescending(u => u.Creation).ToList();
            
            if (Blocks is null || Blocks.Count == 0)
            {
                return new JsonResult(new { Error = "You have not blocked this user yet!" });
            }
            else
            {
                return new JsonResult(new { Blocks_sent = Blocks.Count, Blocks });
            }
        }


        [HttpPost("FollowerUserInfo")]//see if you are following a specific user
        public ActionResult FollowerUserInfo([FromBody] SpecificUserInfoDTO payload)
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
                    return new JsonResult(new { Error = "User's ID needs to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            int? registeredUser = _db.Users
                .Where(u => u.ID == payload.UserID).Select(u => u.ID)
                .SingleOrDefault();

            if (registeredUser is null || registeredUser == 0)
            {
                return new JsonResult(new { Error = "There is no user with the specified ID!" });
            }

            List<Follow> Follows = _db.Follows.Where(u => u.FollowerID == userID && u.FollowedID == registeredUser).OrderByDescending(u => u.Creation).ToList();
            
            if (Follows is null || Follows.Count == 0)
            {
                return new JsonResult(new { Error = "You are not following this user yet!" });
            }
            else
            {
                return new JsonResult(new { Follows_sent = Follows.Count, Follows });
            }
        }


        [HttpPost("SharerUserInfo")]//see if you shared anything from a specific user
        public ActionResult SharerUserInfo([FromBody] SpecificUserInfoDTO payload)
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
                    return new JsonResult(new { Error = "User's ID needs to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            int? registeredUser = _db.Users
                .Where(u => u.ID == payload.UserID).Select(u => u.ID)
                .SingleOrDefault();

            if (registeredUser is null || registeredUser == 0)
            {
                return new JsonResult(new { Error = "There is no user with the specified ID!" });
            }

            List<int> ids = _db.Posts.Where(u => u.UserID == registeredUser).Select(u => u.ID).ToList();

            List<Share> Shares = _db.Shares.Where(u => ids.Contains(u.PostID) && u.UserID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Shares is null || Shares.Count == 0)
            {
                return new JsonResult(new { Error = "None of the user's posts have been shared by you!" });
            }
            else
            {
                return new JsonResult(new { Shares_sent = Shares.Count, Shares });
            }
        }


        [HttpPost("CommenterUserInfo")]//see if you commented on anything from a specific user
        public ActionResult CommenterUserInfo([FromBody] SpecificUserInfoDTO payload)
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
                    return new JsonResult(new { Error = "User's ID needs to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            int? registeredUser = _db.Users
                .Where(u => u.ID == payload.UserID).Select(u => u.ID)
                .SingleOrDefault();

            if (registeredUser is null || registeredUser == 0)
            {
                return new JsonResult(new { Error = "There is no user with the specified ID!" });
            }

            List<int> ids = _db.Posts.Where(u => u.UserID == registeredUser).Select(u => u.ID).ToList();

            List<Comment> Comments = _db.Comments.Where(u => ids.Contains(u.PostID) && u.UserID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Comments is null || Comments.Count == 0)
            {
                return new JsonResult(new { Error = "None of the user's posts have been commented on by you!" });
            }
            else
            {
                return new JsonResult(new { Comments_sent = Comments.Count, Comments });
            }
        }


        [HttpPost("LikerUserInfo")]//see if you liked anything from a specific user
        public ActionResult LikerUserInfo([FromBody] SpecificUserInfoDTO payload)
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
                    return new JsonResult(new { Error = "User's ID needs to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            int? registeredUser = _db.Users
                .Where(u => u.ID == payload.UserID).Select(u => u.ID)
                .SingleOrDefault();

            if (registeredUser is null || registeredUser == 0)
            {
                return new JsonResult(new { Error = "There is no user with the specified ID!" });
            }

            List<int> ids = _db.Posts.Where(u => u.UserID == registeredUser).Select(u => u.ID).ToList();

            List<Like> Likes = _db.Likes.Where(u => ids.Contains(u.PostID) && u.UserID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Likes is null || Likes.Count == 0)
            {
                return new JsonResult(new { Error = "None of the user's posts have been liked by you!" });
            }
            else
            {
                return new JsonResult(new { Likes_sent = Likes.Count, Likes });
            }
        }


        [HttpPost("DislikerUserInfo")]//see if you disliked anything from a specific user
        public ActionResult DislikerUserInfo([FromBody] SpecificUserInfoDTO payload)
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
                    return new JsonResult(new { Error = "User's ID needs to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            int? registeredUser = _db.Users
                .Where(u => u.ID == payload.UserID).Select(u => u.ID)
                .SingleOrDefault();

            if (registeredUser is null || registeredUser == 0)
            {
                return new JsonResult(new { Error = "There is no user with the specified ID!" });
            }

            List<int> ids = _db.Posts.Where(u => u.UserID == registeredUser).Select(u => u.ID).ToList();

            List<Dislike> Dislikes = _db.Dislikes.Where(u => ids.Contains(u.PostID) && u.UserID == userID).OrderByDescending(u => u.Creation).ToList();
            
            if (Dislikes is null || Dislikes.Count == 0)
            {
                return new JsonResult(new { Error = "None of the user's posts have been disliked by you!" });
            }
            else
            {
                return new JsonResult(new { Dislikes_sent = Dislikes.Count, Dislikes });
            }
        }
    }
}
