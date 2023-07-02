using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMediaProject.Data;
using SocialMediaProject.Data.Entities;
using SocialMediaProject.DTOs;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;

namespace SocialMediaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchUserHistoryController : ControllerBase
    {
        private readonly SocialMediaProjectDb _db;
        private readonly IConfiguration _config;

        public SearchUserHistoryController(SocialMediaProjectDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }


        [HttpPost("SearchUserPosts")]//search an user's posts
        public ActionResult SearchUserPosts([FromBody] SearchUserIDDTO payload)
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

            if (registeredUser == userID)
            {
                return new JsonResult(new { Error = "To see your account info, use personal (main) history!" });
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

            List <Post> Posts = _db.Posts.Where(u => u.UserID == payload.UserID).OrderByDescending(u => u.Creation).ToList();

            if (Posts is null || Posts.Count()==0)
            {
                return new JsonResult(new { Error = "The specified user has not posted anything yet!" });
            }

            else
            {
                return new JsonResult(new { Message = "User's posts:", posts_count = Posts.Count, Posts});
            }
        }


        [HttpPost("SearchUserLiker")]//search an user's sent likes
        public ActionResult SearchUserLiker([FromBody] SearchUserIDDTO payload)
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
                return new JsonResult(new { Error = "To see your account info, use personal (main) history!" });
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

            List<Like> Likes = _db.Likes.Where(u => u.UserID == payload.UserID).OrderByDescending(u => u.Creation).ToList();

            if (Likes is null || Likes.Count() == 0)
            {
                return new JsonResult(new { Error = "The specified user has not liked anything yet!" });
            }

            List<int> eb1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> eb2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            List<int> pb = _db.Posts.Where(u => eb1.Contains(u.UserID) || eb2.Contains(u.UserID)).Select(u => u.ID).ToList();

            Likes = _db.Likes.Where(u => u.UserID == payload.UserID && !pb.Contains(u.PostID)).OrderByDescending(u => u.Creation).ToList();

            if (Likes is null || Likes.Count == 0)
            {
                return new JsonResult(new { Error = "The specified user has only liked users you blocked, or those that blocked you!" });
            }

            else
            {
                return new JsonResult(new { likes_sent = Likes.Count, Likes });
            }
        }


        [HttpPost("SearchUserDisliker")]//search an user's sent dislikes
        public ActionResult SearchUserDisliker([FromBody] SearchUserIDDTO payload)
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
                return new JsonResult(new { Error = "To see your account info, use personal (main) history!" });
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

            List<Dislike> Dislikes = _db.Dislikes.Where(u => u.UserID == payload.UserID).OrderByDescending(u => u.Creation).ToList();

            if (Dislikes is null || Dislikes.Count() == 0)
            {
                return new JsonResult(new { Error = "The specified user has not disliked anything yet!" });
            }

            List<int> eb1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> eb2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            List<int> pb = _db.Posts.Where(u => eb1.Contains(u.UserID) || eb2.Contains(u.UserID)).Select(u => u.ID).ToList();

            Dislikes = _db.Dislikes.Where(u => u.UserID == payload.UserID && !pb.Contains(u.PostID)).OrderByDescending(u => u.Creation).ToList();

            if (Dislikes is null || Dislikes.Count == 0)
            {
                return new JsonResult(new { Error = "The specified user has only disliked users you blocked, or those that blocked you!" });
            }

            else
            {
                return new JsonResult(new { dislikes_sent = Dislikes.Count, Dislikes });
            }
        }


        [HttpPost("SearchUserSharer")]//search an user's sent shares
        public ActionResult SearchUserSharer([FromBody] SearchUserIDDTO payload)
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
                return new JsonResult(new { Error = "To see your account info, use personal (main) history!" });
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

            List<Share> Shares = _db.Shares.Where(u => u.UserID == payload.UserID).OrderByDescending(u => u.Creation).ToList();

            if (Shares is null || Shares.Count() == 0)
            {
                return new JsonResult(new { Error = "The specified user has not shared anything yet!" });
            }

            List<int> eb1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> eb2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            List<int> pb = _db.Posts.Where(u => eb1.Contains(u.UserID) || eb2.Contains(u.UserID)).Select(u => u.ID).ToList();

            Shares = _db.Shares.Where(u => u.UserID == payload.UserID && !pb.Contains(u.PostID)).OrderByDescending(u => u.Creation).ToList();

            if (Shares is null || Shares.Count == 0)
            {
                return new JsonResult(new { Error = "The specified user has only shared users you blocked, or those that blocked you!" });
            }

            else
            {
                return new JsonResult(new { shares_sent = Shares.Count, Shares });
            }
        }


        [HttpPost("SearchUserCommenter")]//search an user's sent comments
        public ActionResult SearchUserCommenter([FromBody] SearchUserIDDTO payload)
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
                return new JsonResult(new { Error = "To see your account info, use personal (main) history!" });
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

            List<Comment> Comments = _db.Comments.Where(u => u.UserID == payload.UserID).OrderByDescending(u => u.Creation).ToList();

            if (Comments is null || Comments.Count() == 0)
            {
                return new JsonResult(new { Error = "The specified user has not commented on anything yet!" });
            }

            List<int> eb1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> eb2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            List<int> pb = _db.Posts.Where(u => eb1.Contains(u.UserID) || eb2.Contains(u.UserID)).Select(u => u.ID).ToList();

            Comments = _db.Comments.Where(u => u.UserID == payload.UserID && !pb.Contains(u.PostID)).OrderByDescending(u => u.Creation).ToList();

            if (Comments is null || Comments.Count == 0)
            {
                return new JsonResult(new { Error = "The specified user has only commented on users you blocked, or those that blocked you!" });
            }

            else
            {
                return new JsonResult(new { comments_sent = Comments.Count, Comments });
            }
        }


        [HttpPost("SearchUserFollower")]//search an user's sent follows
        public ActionResult SearchUserFollower([FromBody] SearchUserIDDTO payload)
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
                return new JsonResult(new { Error = "To see your account info, use personal (main) history!" });
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

            List<Follow> Follows = _db.Follows.Where(u => u.FollowerID == payload.UserID).OrderByDescending(u => u.Creation).ToList();

            if (Follows is null || Follows.Count() == 0)
            {
                return new JsonResult(new { Error = "The specified user is not following anyone yet!" });
            }

            List<int> eb1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> eb2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            Follows = _db.Follows.Where(u => u.FollowerID == payload.UserID && !eb1.Contains(u.FollowedID) && !eb2.Contains(u.FollowedID)).OrderByDescending(u => u.Creation).ToList();

            if (Follows is null || Follows.Count == 0)
            {
                return new JsonResult(new { Error = "The specified user is only following users you blocked, or those that blocked you!" });
            }

            else
            {
                return new JsonResult(new { follows_sent = Follows.Count, Follows });
            }
        }


        [HttpPost("SearchUserBlocker")]//search an user's sent blocks
        public ActionResult SearchUserBlocker([FromBody] SearchUserIDDTO payload)
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
                return new JsonResult(new { Error = "To see your account info, use personal (main) history!" });
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

            List<Block> Blocks = _db.Blocks.Where(u => u.BlockerID == payload.UserID).OrderByDescending(u => u.Creation).ToList();

            if (Blocks is null || Blocks.Count() == 0)
            {
                return new JsonResult(new { Error = "The specified user is not blocking anyone yet!" });
            }

            List<int> eb1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> eb2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            Blocks = _db.Blocks.Where(u => u.BlockerID == payload.UserID && !eb1.Contains(u.BlockedID) && !eb2.Contains(u.BlockedID)).OrderByDescending(u => u.Creation).ToList();

            if (Blocks is null || Blocks.Count == 0)
            {
                return new JsonResult(new { Error = "The specified user is only blocking users you blocked, or those that blocked you!" });
            }

            else
            {
                return new JsonResult(new { blocks_sent = Blocks.Count, Blocks });
            }
        }
    }
}
