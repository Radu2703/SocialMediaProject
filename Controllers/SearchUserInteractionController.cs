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
    public class SearchUserInteractionController : ControllerBase
    {
        private readonly SocialMediaProjectDb _db;
        private readonly IConfiguration _config;

        public SearchUserInteractionController(SocialMediaProjectDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }


        [HttpPost("SearchUsersFollowing")]//search if an user is following another user
        public ActionResult SearchUsersFollowing([FromBody] SearchUserUserDTO payload)
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
                int i = payload.UserID1;
                int j = payload.UserID2;
                if (i <= 0 || j <= 0)
                {
                    return new JsonResult(new { Error = "Users' IDs need to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            int? registeredUser1 = _db.Users
                .Where(u => u.ID == payload.UserID1).Select(u => u.ID)
                .SingleOrDefault();

            int? registeredUser2 = _db.Users
                .Where(u => u.ID == payload.UserID2).Select(u => u.ID)
                .SingleOrDefault();

            if (registeredUser1 is null || registeredUser1 == 0)
            {
                return new JsonResult(new { Error = "There is no user with the first specified ID!" });
            }

            if (registeredUser2 is null || registeredUser2 == 0)
            {
                return new JsonResult(new { Error = "There is no user with the second specified ID!" });
            }

            if (payload.UserID1 == payload.UserID2)
            {
                return new JsonResult(new { Error = "The IDs need to be different!" });
            }

            if (registeredUser1 == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific user history!" });
            }

            if (registeredUser2 == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific user popularity!" });
            }

            Block? existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID1)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the first user, as you are blocked by them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID1)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the first user, as you are blocking them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID2)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the second user, as you are blocked by them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID2)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the second user, as you are blocking them!" });
            }

            Follow? Follows = _db.Follows.Where(u => u.FollowerID == payload.UserID1 && u.FollowedID == payload.UserID2).SingleOrDefault();

            if (Follows is null)
            {
                return new JsonResult(new { Error = "The first user is not following the second user!" });
            }

            else
            {
                return new JsonResult(new { Message = "The first user is following the second user!", Follows });
            }
        }


        [HttpPost("SearchUsersBlocking")]//search if an user is blocking another user
        public ActionResult SearchUsersBlocking([FromBody] SearchUserUserDTO payload)
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
                int i = payload.UserID1;
                int j = payload.UserID2;
                if (i <= 0 || j <= 0)
                {
                    return new JsonResult(new { Error = "Users' IDs need to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            int? registeredUser1 = _db.Users
                .Where(u => u.ID == payload.UserID1).Select(u => u.ID)
                .SingleOrDefault();

            int? registeredUser2 = _db.Users
                .Where(u => u.ID == payload.UserID2).Select(u => u.ID)
                .SingleOrDefault();

            if (registeredUser1 is null || registeredUser1 == 0)
            {
                return new JsonResult(new { Error = "There is no user with the first specified ID!" });
            }

            if (registeredUser2 is null || registeredUser2 == 0)
            {
                return new JsonResult(new { Error = "There is no user with the second specified ID!" });
            }

            if (payload.UserID1 == payload.UserID2)
            {
                return new JsonResult(new { Error = "The IDs need to be different!" });
            }

            if (registeredUser1 == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific user history!" });
            }

            if (registeredUser2 == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific user popularity!" });
            }

            Block? existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID1)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the first user, as you are blocked by them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID1)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the first user, as you are blocking them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID2)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the second user, as you are blocked by them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID2)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the second user, as you are blocking them!" });
            }

            Block? Blocks = _db.Blocks.Where(u => u.BlockerID == payload.UserID1 && u.BlockedID == payload.UserID2).SingleOrDefault();

            if (Blocks is null)
            {
                return new JsonResult(new { Error = "The first user is not blocking the second user!" });
            }

            else
            {
                return new JsonResult(new { Message = "The first user is blocking the second user!", Blocks });
            }
        }


        [HttpPost("SearchUserLikingPost")]//search if an user liked a post
        public ActionResult SearchUserLikingPost([FromBody] SearchUserPostDTO payload)
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

            if (registeredUser == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific post history!" });
            }

            if (postUser == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific user post!" });
            }
            
            if (payload.UserID == postUser)
            {
                return new JsonResult(new { Error = "The post can not be created by the user!" });
            }

            Block? existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the user, as you are blocked by them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the user, as you are blocking them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == postUser)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the post, as you are blocked by its creator!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == postUser)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the post, as you are blocking its creator!" });
            }

            Like? Likes = _db.Likes.Where(u => u.UserID == payload.UserID && u.PostID == payload.PostID).SingleOrDefault();

            if (Likes is null)
            {
                return new JsonResult(new { Error = "The user is not liking the post!" });
            }

            else
            {
                return new JsonResult(new { Message = "The user is liking the post!", Likes });
            }
        }


        [HttpPost("SearchUserDislikingPost")]//search if an user disliked a post
        public ActionResult SearchUserDislikingPost([FromBody] SearchUserPostDTO payload)
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

            if (registeredUser == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific post history!" });
            }

            if (postUser == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific user post!" });
            }

            if (payload.UserID == postUser)
            {
                return new JsonResult(new { Error = "The post can not be created by the user!" });
            }

            Block? existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the user, as you are blocked by them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the user, as you are blocking them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == postUser)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the post, as you are blocked by its creator!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == postUser)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the post, as you are blocking its creator!" });
            }

            Dislike? Dislikes = _db.Dislikes.Where(u => u.UserID == payload.UserID && u.PostID == payload.PostID).SingleOrDefault();

            if (Dislikes is null)
            {
                return new JsonResult(new { Error = "The user is not disliking the post!" });
            }

            else
            {
                return new JsonResult(new { Message = "The user is disliking the post!", Dislikes });
            }
        }


        [HttpPost("SearchUserSharingPost")]//search if an user shared a post
        public ActionResult SearchUserSharingPost([FromBody] SearchUserPostDTO payload)
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

            if (registeredUser == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific post history!" });
            }

            if (postUser == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific user post!" });
            }

            if (payload.UserID == postUser)
            {
                return new JsonResult(new { Error = "The post can not be created by the user!" });
            }

            Block? existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the user, as you are blocked by them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the user, as you are blocking them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == postUser)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the post, as you are blocked by its creator!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == postUser)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the post, as you are blocking its creator!" });
            }

            List <Share> Shares = _db.Shares.Where(u => u.UserID == payload.UserID && u.PostID == payload.PostID).OrderByDescending(u => u.Creation).ToList();

            if (Shares is null || Shares.Count == 0)
            {
                return new JsonResult(new { Error = "The user did not share the post!" });
            }

            else
            {
                return new JsonResult(new { Message = "The user shared the post!", count_shares= Shares.Count, Shares });
            }
        }


        [HttpPost("SearchUserCommentingPost")]//search if an user commented on a post
        public ActionResult SearchUserCommentingPost([FromBody] SearchUserPostDTO payload)
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

            if (registeredUser == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific post history!" });
            }

            if (postUser == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific user post!" });
            }

            Block? existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the user, as you are blocked by them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the user, as you are blocking them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == postUser)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the post, as you are blocked by its creator!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == postUser)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the post, as you are blocking its creator!" });
            }

            List<Comment> Comments = _db.Comments.Where(u => u.UserID == payload.UserID && u.PostID == payload.PostID).OrderByDescending(u => u.Creation).ToList();

            if (Comments is null || Comments.Count == 0)
            {
                return new JsonResult(new { Error = "The user did not comment on the post!" });
            }

            else
            {
                return new JsonResult(new { Message = "The user commented on the post!", count_comments = Comments.Count, Comments });
            }
        }


        [HttpPost("SearchUsersShares")]//search if an user shared any of another user's posts
        public ActionResult SearchUsersShares([FromBody] SearchUserUserDTO payload)
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
                int i = payload.UserID1;
                int j = payload.UserID2;
                if (i <= 0 || j <= 0)
                {
                    return new JsonResult(new { Error = "Users' IDs need to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            int? registeredUser1 = _db.Users
                .Where(u => u.ID == payload.UserID1).Select(u => u.ID)
                .SingleOrDefault();

            int? registeredUser2 = _db.Users
                .Where(u => u.ID == payload.UserID2).Select(u => u.ID)
                .SingleOrDefault();

            if (registeredUser1 is null || registeredUser1 == 0)
            {
                return new JsonResult(new { Error = "There is no user with the first specified ID!" });
            }

            if (registeredUser2 is null || registeredUser2 == 0)
            {
                return new JsonResult(new { Error = "There is no user with the second specified ID!" });
            }

            if (payload.UserID1 == payload.UserID2)
            {
                return new JsonResult(new { Error = "The IDs need to be different!" });
            }

            if (registeredUser1 == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific user history!" });
            }

            if (registeredUser2 == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific user popularity!" });
            }

            Block? existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID1)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the first user, as you are blocked by them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID1)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the first user, as you are blocking them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID2)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the second user, as you are blocked by them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID2)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the second user, as you are blocking them!" });
            }

            List<int> ids = _db.Posts.Where(u => u.UserID == payload.UserID2).Select(u => u.ID).ToList();

            List<Share> Shares = _db.Shares.Where(u => ids.Contains(u.PostID) && u.UserID == payload.UserID1).OrderByDescending(u => u.Creation).ToList();
            
            if (Shares is null || Shares.Count == 0)
            {
                return new JsonResult(new { Error = "None of the second user's posts have been shared by the first user!" });
            }
            else
            {
                return new JsonResult(new { Message = "The first user has shared posts of the second user!", Shares_count = Shares.Count, Shares });
            }
        }


        [HttpPost("SearchUsersComments")]//search if an user commented on any of another user's posts
        public ActionResult SearchUsersComments([FromBody] SearchUserUserDTO payload)
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
                int i = payload.UserID1;
                int j = payload.UserID2;
                if (i <= 0 || j <= 0)
                {
                    return new JsonResult(new { Error = "Users' IDs need to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            int? registeredUser1 = _db.Users
                .Where(u => u.ID == payload.UserID1).Select(u => u.ID)
                .SingleOrDefault();

            int? registeredUser2 = _db.Users
                .Where(u => u.ID == payload.UserID2).Select(u => u.ID)
                .SingleOrDefault();

            if (registeredUser1 is null || registeredUser1 == 0)
            {
                return new JsonResult(new { Error = "There is no user with the first specified ID!" });
            }

            if (registeredUser2 is null || registeredUser2 == 0)
            {
                return new JsonResult(new { Error = "There is no user with the second specified ID!" });
            }

            if (registeredUser1 == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific user history!" });
            }

            if (registeredUser2 == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific user popularity!" });
            }

            Block? existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID1)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the first user, as you are blocked by them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID1)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the first user, as you are blocking them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID2)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the second user, as you are blocked by them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID2)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the second user, as you are blocking them!" });
            }

            List<int> ids = _db.Posts.Where(u => u.UserID == payload.UserID2).Select(u => u.ID).ToList();

            List<Comment> Comments = _db.Comments.Where(u => ids.Contains(u.PostID) && u.UserID == payload.UserID1).OrderByDescending(u => u.Creation).ToList();
            
            if (Comments is null || Comments.Count == 0)
            {
                return new JsonResult(new { Error = "None of the second user's posts have been commented on by the first user!" });
            }
            else
            {
                return new JsonResult(new { Message = "The first user has commented on posts of the second user!", Comments_count = Comments.Count, Comments });
            }
        }


        [HttpPost("SearchUsersLikes")]//search if an user liked any of another user's posts
        public ActionResult SearchUsersLikes([FromBody] SearchUserUserDTO payload)
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
                int i = payload.UserID1;
                int j = payload.UserID2;
                if (i <= 0 || j <= 0)
                {
                    return new JsonResult(new { Error = "Users' IDs need to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            int? registeredUser1 = _db.Users
                .Where(u => u.ID == payload.UserID1).Select(u => u.ID)
                .SingleOrDefault();

            int? registeredUser2 = _db.Users
                .Where(u => u.ID == payload.UserID2).Select(u => u.ID)
                .SingleOrDefault();

            if (registeredUser1 is null || registeredUser1 == 0)
            {
                return new JsonResult(new { Error = "There is no user with the first specified ID!" });
            }

            if (registeredUser2 is null || registeredUser2 == 0)
            {
                return new JsonResult(new { Error = "There is no user with the second specified ID!" });
            }

            if (payload.UserID1 == payload.UserID2)
            {
                return new JsonResult(new { Error = "The IDs need to be different!" });
            }

            if (registeredUser1 == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific user history!" });
            }

            if (registeredUser2 == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific user popularity!" });
            }

            Block? existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID1)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the first user, as you are blocked by them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID1)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the first user, as you are blocking them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID2)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the second user, as you are blocked by them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID2)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the second user, as you are blocking them!" });
            }

            List<int> ids = _db.Posts.Where(u => u.UserID == payload.UserID2).Select(u => u.ID).ToList();

            List<Like> Likes = _db.Likes.Where(u => ids.Contains(u.PostID) && u.UserID == payload.UserID1).OrderByDescending(u => u.Creation).ToList();

            if (Likes is null || Likes.Count == 0)
            {
                return new JsonResult(new { Error = "None of the second user's posts have been liked by the first user!" });
            }
            else
            {
                return new JsonResult(new { Message = "The first user has liked posts of the second user!", Likes_count = Likes.Count, Likes });
            }
        }


        [HttpPost("SearchUsersDislikes")]//search if an user disliked any of another user's posts
        public ActionResult SearchUsersDislikes([FromBody] SearchUserUserDTO payload)
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
                int i = payload.UserID1;
                int j = payload.UserID2;
                if (i <= 0 || j <= 0)
                {
                    return new JsonResult(new { Error = "Users' IDs need to be strictly bigger than 0!" });
                }
            }
            catch (Exception e) { return new JsonResult(new { Error = e.Message }); }

            int? registeredUser1 = _db.Users
                .Where(u => u.ID == payload.UserID1).Select(u => u.ID)
                .SingleOrDefault();

            int? registeredUser2 = _db.Users
                .Where(u => u.ID == payload.UserID2).Select(u => u.ID)
                .SingleOrDefault();

            if (registeredUser1 is null || registeredUser1 == 0)
            {
                return new JsonResult(new { Error = "There is no user with the first specified ID!" });
            }

            if (registeredUser2 is null || registeredUser2 == 0)
            {
                return new JsonResult(new { Error = "There is no user with the second specified ID!" });
            }

            if (payload.UserID1 == payload.UserID2)
            {
                return new JsonResult(new { Error = "The IDs need to be different!" });
            }

            if (registeredUser1 == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific user history!" });
            }

            if (registeredUser2 == userID)
            {
                return new JsonResult(new { Error = "To see your interactions with an user or a post, use specific user popularity!" });
            }

            Block? existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID1)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the first user, as you are blocked by them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID1)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the first user, as you are blocking them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == payload.UserID2)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the second user, as you are blocked by them!" });
            }

            existingBlock = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == payload.UserID2)
                .SingleOrDefault();

            if (existingBlock is not null)
            {
                return new JsonResult(new { Error = "You can not search info about the second user, as you are blocking them!" });
            }

            List<int> ids = _db.Posts.Where(u => u.UserID == payload.UserID2).Select(u => u.ID).ToList();

            List<Dislike> Dislikes = _db.Dislikes.Where(u => ids.Contains(u.PostID) && u.UserID == payload.UserID1).OrderByDescending(u => u.Creation).ToList();

            if (Dislikes is null || Dislikes.Count == 0)
            {
                return new JsonResult(new { Error = "None of the second user's posts have been disliked by the first user!" });
            }
            else
            {
                return new JsonResult(new { Message = "The first user has disliked posts of the second user!", Dislikes_count = Dislikes.Count, Dislikes });
            }
        }
    }
}
