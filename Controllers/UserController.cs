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
    public class UserController : ControllerBase
    {
        private readonly SocialMediaProjectDb _db;
        private readonly IConfiguration _config;

        public UserController(SocialMediaProjectDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("FollowUser")]//follow an user
        public ActionResult FollowUser([FromBody] FollowDTO payload)
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

            var registeredUser = _db.Users
                .Where(u => u.ID == payload.UserID)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return new JsonResult(new { Error = "There is no user with the specified ID!" });
            }

            if (registeredUser.ID == userID)
            {
                return new JsonResult(new { Error = "You can not follow yourself!" });
            }

            Follow? existingFollow = _db.Follows
                .Where(u => u.FollowerID == userID && u.FollowedID == payload.UserID)
                .SingleOrDefault();

            if (existingFollow is not null)
            {
                return new JsonResult(new { Error = "You are already following this user!" });
            }

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not follow this user, as you are blocked by them!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not follow this user, as you are blocking them!" });
            }

            DateTime ct = DateTime.Now;

            _db.Follows.Add(new Data.Entities.Follow
            {
                FollowerID = userID,
                FollowedID = payload.UserID,
                Creation = ct
            });

            _db.SaveChanges();

            Follow? registeredFollow = _db.Follows
                .Where(u => u.FollowedID == payload.UserID && u.FollowerID == userID && u.Creation == ct)
                .SingleOrDefault();

            if (registeredFollow is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You are now following the specified user!", FollowerID = userID, FollowedID = payload.UserID, Creation = ct });
            }
        }

        [HttpPost("BlockUser")]//block an user
        public ActionResult BlockUser([FromBody] BlockDTO payload)
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

            var registeredUser = _db.Users
                .Where(u => u.ID == payload.UserID)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return new JsonResult(new { Error = "There is no user with the specified ID!" });
            }

            if (registeredUser.ID == userID)
            {
                return new JsonResult(new { Error = "You can not block yourself!" });
            }

            Block? existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You are already blocking this user!" });
            }

            DateTime ct = DateTime.Now;

            _db.Blocks.Add(new Data.Entities.Block
            {
                BlockerID = userID,
                BlockedID = payload.UserID,
                Creation = ct
            });

            Follow? existingFollow1 = _db.Follows
                .Where(u => u.FollowedID == userID && u.FollowerID == payload.UserID)
                .SingleOrDefault();

            Follow? existingFollow2 = _db.Follows
                .Where(u => u.FollowerID == userID && u.FollowedID == payload.UserID)
                .SingleOrDefault();

            if (existingFollow1 is not null)
            {
                _db.Follows.Remove(existingFollow1);
            }

            if (existingFollow2 is not null)
            {
                _db.Follows.Remove(existingFollow2);
            }

            _db.SaveChanges();

            Block? registeredBlock = _db.Blocks
                .Where(u => u.BlockedID == payload.UserID && u.BlockerID == userID && u.Creation == ct)
                .SingleOrDefault();

            if (registeredBlock is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You are now blocking the specified user!", BlockerID = userID, BlockedID = payload.UserID, Creation = ct });
            }
        }

        [HttpPost("MessageUser")]//message an user
        public ActionResult MessageUser([FromBody] CreateMessageDTO payload)
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

            if (String.IsNullOrEmpty(payload.Content))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            var registeredUser = _db.Users
                .Where(u => u.ID == payload.UserID)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return new JsonResult(new { Error = "There is no user with the specified ID!" });
            }

            if (registeredUser.ID == userID)
            {
                return new JsonResult(new { Error = "You can not send a message to yourself!" });
            }

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not message this user, as you are blocked by them!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not message this user, as you are blocking them!" });
            }

            DateTime ct = DateTime.Now;

            Message? existingMessage = _db.Messages
                .Where(u => u.MessagerID == userID && u.Creation == ct)
                .SingleOrDefault();

            if (existingMessage is not null)
            {
                return new JsonResult(new { Error = "Do not send a message more than once at the same time!" });
            }

            _db.Messages.Add(new Data.Entities.Message
            {
                MessagerID = userID,
                MessagedID = payload.UserID,
                Content = payload.Content,
                Creation = ct
            });

            _db.SaveChanges();

            Message? registeredMessage = _db.Messages
                .Where(u => u.Content == payload.Content && u.MessagedID == payload.UserID && u.MessagerID == userID && u.Creation == ct)
                .SingleOrDefault();

            if (registeredMessage is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You sent a message to the specified user!", MessagerID = userID, MessagedID = payload.UserID, payload.Content, Creation = ct });
            }
        }
    }
}
