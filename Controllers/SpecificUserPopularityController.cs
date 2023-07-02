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
    public class SpecificUserPopularityController : ControllerBase
    {
        private readonly SocialMediaProjectDb _db;
        private readonly IConfiguration _config;

        public SpecificUserPopularityController(SocialMediaProjectDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("MessagedUserInfo")]//see your messages from a specific user
        public ActionResult MessagedUserInfo([FromBody] SpecificUserInfoDTO payload)
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

            if (registeredUser is null || registeredUser==0)
            {
                return new JsonResult(new { Error = "There is no user with the specified ID!" });
            }

            List<Message> Messages = _db.Messages.Where(u => u.MessagedID == userID && u.MessagerID == registeredUser).OrderByDescending(u => u.Creation).ToList();
            
            if (Messages is null || Messages.Count == 0)
            {
                return new JsonResult(new { Error = "You have not been messaged by this user yet!" });
            }
            else
            {
                return new JsonResult(new { Messages_received = Messages.Count, Messages });
            }
        }


        [HttpPost("BlockedUserInfo")]//see if you are blocked by a specific user
        public ActionResult BlockedUserInfo([FromBody] SpecificUserInfoDTO payload)
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

            List<Block> Blocks = _db.Blocks.Where(u => u.BlockedID == userID && u.BlockerID == registeredUser).OrderByDescending(u => u.Creation).ToList();
            
            if (Blocks is null || Blocks.Count == 0)
            {
                return new JsonResult(new { Error = "You have not been blocked by this user yet!" });
            }
            else
            {
                return new JsonResult(new { Blocks_received = Blocks.Count, Blocks });
            }
        }


        [HttpPost("FollowedUserInfo")]//see if you are followed by a specific user
        public ActionResult FollowedUserInfo([FromBody] SpecificUserInfoDTO payload)
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

            List<Follow> Follows = _db.Follows.Where(u => u.FollowedID == userID && u.FollowerID == registeredUser).OrderByDescending(u => u.Creation).ToList();
            
            if (Follows is null || Follows.Count == 0)
            {
                return new JsonResult(new { Error = "You are not followed by this user yet!" });
            }
            else
            {
                return new JsonResult(new { Follows_received = Follows.Count, Follows });
            }
        }


        [HttpPost("SharedUserInfo")]//see if any of your posts were shared by a specific user
        public ActionResult SharedUserInfo([FromBody] SpecificUserInfoDTO payload)
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

            List<int> ids = _db.Posts.Where(u => u.UserID == userID).Select(u => u.ID).ToList();

            List<Share> Shares = _db.Shares.Where(u => ids.Contains(u.PostID) && u.UserID == registeredUser).OrderByDescending(u => u.Creation).ToList();
            
            if (Shares is null || Shares.Count == 0)
            {
                return new JsonResult(new { Error = "None of your posts have been shared by this user yet!" });
            }
            else
            {
                return new JsonResult(new { Shares_received = Shares.Count, Shares });
            }
        }


        [HttpPost("CommentedUserInfo")]//see if any of your posts were commented on by a specific user
        public ActionResult CommentedUserInfo([FromBody] SpecificUserInfoDTO payload)
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

            List<int> ids = _db.Posts.Where(u => u.UserID == userID).Select(u => u.ID).ToList();

            List<Comment> Comments = _db.Comments.Where(u => ids.Contains(u.PostID) && u.UserID == registeredUser).OrderByDescending(u => u.Creation).ToList();
            
            if (Comments is null || Comments.Count == 0)
            {
                return new JsonResult(new { Error = "None of your posts have been commented on by this user yet!" });
            }
            else
            {
                return new JsonResult(new { Comments_received = Comments.Count, Comments });
            }
        }


        [HttpPost("LikedUserInfo")]//see if any of your posts were liked by a specific user
        public ActionResult LikedUserInfo([FromBody] SpecificUserInfoDTO payload)
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

            List<int> ids = _db.Posts.Where(u => u.UserID == userID).Select(u => u.ID).ToList();

            List<Like> Likes = _db.Likes.Where(u => ids.Contains(u.PostID) && u.UserID == registeredUser).OrderByDescending(u => u.Creation).ToList();
            
            if (Likes is null || Likes.Count == 0)
            {
                return new JsonResult(new { Error = "None of your posts have been liked by this user yet!" });
            }
            else
            {
                return new JsonResult(new { Likes_received = Likes.Count, Likes });
            }
        }


        [HttpPost("DislikedUserInfo")]//see if any of your posts were disliked by a specific user
        public ActionResult DislikedUserInfo([FromBody] SpecificUserInfoDTO payload)
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

            List<int> ids = _db.Posts.Where(u => u.UserID == userID).Select(u => u.ID).ToList();

            List<Dislike> Dislikes = _db.Dislikes.Where(u => ids.Contains(u.PostID) && u.UserID == registeredUser).OrderByDescending(u => u.Creation).ToList();
            
            if (Dislikes is null || Dislikes.Count == 0)
            {
                return new JsonResult(new { Error = "None of your posts have been disliked by this user yet!" });
            }
            else
            {
                return new JsonResult(new { Dislikes_received = Dislikes.Count, Dislikes });
            }
        }
    }
}
