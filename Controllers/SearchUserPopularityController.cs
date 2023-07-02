using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMediaProject.Data;
using SocialMediaProject.Data.Entities;
using SocialMediaProject.DTOs;
using System.Xml.Linq;

namespace SocialMediaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchUserPopularityController : ControllerBase
    {
        private readonly SocialMediaProjectDb _db;
        private readonly IConfiguration _config;

        public SearchUserPopularityController(SocialMediaProjectDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("SearchUserLiked")]//search an user's overall received likes
        public ActionResult SearchUserLiked([FromBody] SearchUserIDDTO payload)
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

            if (registeredUser == userID)
            {
                return new JsonResult(new { Error = "To see your account info, use personal (main) popularity!" });
            }

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocked by them!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocking them!" });
            }

            List<int> ids = _db.Posts.Where(u => u.UserID == registeredUser).Select(u => u.ID).ToList();

            List<Like> Likes = _db.Likes.Where(u => ids.Contains(u.PostID)).OrderByDescending(u => u.Creation).ToList();
            
            if (Likes is null || Likes.Count == 0)
            {
                return new JsonResult(new { Error = "The specified user has not been liked by anyone yet!" });
            }

            List<int> eb1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> eb2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            Likes = _db.Likes.Where(u => ids.Contains(u.PostID) && !eb1.Contains(u.UserID) && !eb2.Contains(u.UserID)).OrderByDescending(u => u.Creation).ToList();

            if (Likes is null || Likes.Count == 0)
            {
                return new JsonResult(new { Error = "The specified user has only been liked by users you blocked, or those that blocked you!" });
            }

            else
            {
                return new JsonResult(new { likes_received = Likes.Count, Likes });
            }
        }


        [HttpPost("SearchUserDisliked")]//search an user's overall received dislikes
        public ActionResult SearchUserDisliked([FromBody] SearchUserIDDTO payload)
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

            if (registeredUser == userID)
            {
                return new JsonResult(new { Error = "To see your account info, use personal (main) popularity!" });
            }

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocked by them!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocking them!" });
            }

            List<int> ids = _db.Posts.Where(u => u.UserID == registeredUser).Select(u => u.ID).ToList();

            List<Dislike> Dislikes = _db.Dislikes.Where(u => ids.Contains(u.PostID)).OrderByDescending(u => u.Creation).ToList();

            if (Dislikes is null || Dislikes.Count == 0)
            {
                return new JsonResult(new { Error = "The specified user has not been disliked by anyone yet!" });
            }

            List<int> eb1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> eb2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            Dislikes = _db.Dislikes.Where(u => ids.Contains(u.PostID) && !eb1.Contains(u.UserID) && !eb2.Contains(u.UserID)).OrderByDescending(u => u.Creation).ToList();

            if (Dislikes is null || Dislikes.Count == 0)
            {
                return new JsonResult(new { Error = "The specified user has only been disliked by users you blocked, or those that blocked you!" });
            }

            else
            {
                return new JsonResult(new { dislikes_received = Dislikes.Count, Dislikes });
            }
        }


        [HttpPost("SearchUserShared")]//search an user's overall received shares
        public ActionResult SearchUserShared([FromBody] SearchUserIDDTO payload)
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

            if (registeredUser == userID)
            {
                return new JsonResult(new { Error = "To see your account info, use personal (main) popularity!" });
            }

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocked by them!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocking them!" });
            }

            List<int> ids = _db.Posts.Where(u => u.UserID == registeredUser).Select(u => u.ID).ToList();

            List<Share> Shares = _db.Shares.Where(u => ids.Contains(u.PostID)).OrderByDescending(u => u.Creation).ToList();

            if (Shares is null || Shares.Count == 0)
            {
                return new JsonResult(new { Error = "The specified user has not been shared by anyone yet!" });
            }

            List<int> eb1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> eb2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            Shares = _db.Shares.Where(u => ids.Contains(u.PostID) && !eb1.Contains(u.UserID) && !eb2.Contains(u.UserID)).OrderByDescending(u => u.Creation).ToList();

            if (Shares is null || Shares.Count == 0)
            {
                return new JsonResult(new { Error = "The specified user has only been shared by users you blocked, or those that blocked you!" });
            }

            else
            {
                return new JsonResult(new { shares_received = Shares.Count, Shares });
            }
        }


        [HttpPost("SearchUserCommented")]//search an user's overall received comments
        public ActionResult SearchUserCommented([FromBody] SearchUserIDDTO payload)
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

            if (registeredUser == userID)
            {
                return new JsonResult(new { Error = "To see your account info, use personal (main) popularity!" });
            }

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocked by them!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocking them!" });
            }

            List<int> ids = _db.Posts.Where(u => u.UserID == registeredUser).Select(u => u.ID).ToList();

            List<Comment> Comments = _db.Comments.Where(u => ids.Contains(u.PostID)).OrderByDescending(u => u.Creation).ToList();

            if (Comments is null || Comments.Count == 0)
            {
                return new JsonResult(new { Error = "The specified user has not been commented on by anyone yet!" });
            }

            List<int> eb1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> eb2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            Comments = _db.Comments.Where(u => ids.Contains(u.PostID) && !eb1.Contains(u.UserID) && !eb2.Contains(u.UserID)).OrderByDescending(u => u.Creation).ToList();

            if (Comments is null || Comments.Count == 0)
            {
                return new JsonResult(new { Error = "The specified user has only been commented by users you blocked, or those that blocked you!" });
            }

            else
            {
                return new JsonResult(new { comments_received = Comments.Count, Comments });
            }
        }


        [HttpPost("SearchUserFollowed")]//search an user's overall received follows
        public ActionResult SearchUserFollowed([FromBody] SearchUserIDDTO payload)
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

            if (registeredUser == userID)
            {
                return new JsonResult(new { Error = "To see your account info, use personal (main) popularity!" });
            }

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocked by them!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocking them!" });
            }

            List<Follow> Follows = _db.Follows.Where(u => u.FollowedID == registeredUser).OrderByDescending(u => u.Creation).ToList();

            if (Follows is null || Follows.Count == 0)
            {
                return new JsonResult(new { Error = "The specified user is not followed by anyone yet!" });
            }

            List<int> eb1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> eb2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            Follows = _db.Follows.Where(u => u.FollowedID == registeredUser && !eb1.Contains(u.FollowerID) && !eb2.Contains(u.FollowerID)).OrderByDescending(u => u.Creation).ToList();

            if (Follows is null || Follows.Count == 0)
            {
                return new JsonResult(new { Error = "The specified user is only followed by users you blocked, or those that blocked you!" });
            }

            else
            {
                return new JsonResult(new { follows_received = Follows.Count, Follows });
            }
        }


        [HttpPost("SearchUserBlocked")]//search an user's overall received blocks
        public ActionResult SearchUserBlocked([FromBody] SearchUserIDDTO payload)
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

            if (registeredUser == userID)
            {
                return new JsonResult(new { Error = "To see your account info, use personal (main) popularity!" });
            }

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocked by them!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocking them!" });
            }

            List<Block> Blocks = _db.Blocks.Where(u => u.BlockedID == registeredUser).OrderByDescending(u => u.Creation).ToList();

            if (Blocks is null || Blocks.Count == 0)
            {
                return new JsonResult(new { Error = "The specified user is not blocked by anyone yet!" });
            }

            List<int> eb1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> eb2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            Blocks = _db.Blocks.Where(u => u.BlockedID == registeredUser && !eb1.Contains(u.BlockerID) && !eb2.Contains(u.BlockerID)).OrderByDescending(u => u.Creation).ToList();

            if (Blocks is null || Blocks.Count == 0)
            {
                return new JsonResult(new { Error = "The specified user is only blocked by users you blocked, or those that blocked you!" });
            }

            else
            {
                return new JsonResult(new { blocks_received = Blocks.Count, Blocks });
            }
        }
    }
}
