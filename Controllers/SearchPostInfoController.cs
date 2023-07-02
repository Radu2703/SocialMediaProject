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
    public class SearchPostInfoController : ControllerBase
    {
        private readonly SocialMediaProjectDb _db;
        private readonly IConfiguration _config;

        public SearchPostInfoController(SocialMediaProjectDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }


        [HttpPost("SearchPostContent")]//search a post
        public ActionResult SearchPostContent([FromBody] SearchPostInfoDTO payload)
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

            Post? existingPost = _db.Posts
                .Where(u => u.ID == payload.PostID)
                .SingleOrDefault();

            if (existingPost is null)
            {
                return new JsonResult(new { Error = "There is no post with the specified ID!" });
            }

            if (existingPost.UserID == userID)
            {
                return new JsonResult(new { Error = "To see your posts info, use specific popularity!" });
            }

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == existingPost.UserID)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this post, as you are blocked by its user!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == existingPost.UserID)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this post, as you are blocking its user!" });
            }

            else
            {
                return new JsonResult(new { Message = "Post info:", existingPost.ID, existingPost.UserID, existingPost.Content, existingPost.Creation });
            }
        }


        [HttpPost("SearchPostShares")]//search a post's shares
        public ActionResult SearchPostShares([FromBody] SearchPostInfoDTO payload)
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

            int? rp = _db.Posts
                .Where(u => u.ID == payload.PostID).Select(u => u.UserID)
                .SingleOrDefault();

            if (registeredPost is null || registeredPost == 0 || rp is null || rp == 0)
            {
                return new JsonResult(new { Error = "There is no post with the specified ID!" });
            }

            if (rp == userID)
            {
                return new JsonResult(new { Error = "To see your posts info, use specific popularity!" });
            }

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == rp)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this post, as you are blocked by its user!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == rp)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this post, as you are blocking its user!" });
            }

            List<Share> Shares = _db.Shares.Where(u => u.PostID == payload.PostID).OrderByDescending(u => u.Creation).ToList();
            
            if (Shares is null || Shares.Count == 0)
            {
                return new JsonResult(new { Error = "The searched post has not been shared by anyone yet!" });
            }

            List<int> eb1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> eb2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            Shares = _db.Shares.Where(u => u.PostID == payload.PostID && !eb1.Contains(u.UserID) && !eb2.Contains(u.UserID)).OrderByDescending(u => u.Creation).ToList();
            
            if (Shares is null || Shares.Count == 0)
            {
                return new JsonResult(new { Error = "The searched post has been shared only by users you blocked, or those that blocked you!" });
            }

            else
            {
                return new JsonResult(new { Message = "Post shares:", Shares_received = Shares.Count, Shares });
            }
        }


        [HttpPost("SearchPostComments")]//search a post's comments
        public ActionResult SearchPostComments([FromBody] SearchPostInfoDTO payload)
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

            int? rp = _db.Posts
                .Where(u => u.ID == payload.PostID).Select(u => u.UserID)
                .SingleOrDefault();

            if (registeredPost is null || registeredPost == 0 || rp is null || rp == 0)
            {
                return new JsonResult(new { Error = "There is no post with the specified ID!" });
            }

            if (rp == userID)
            {
                return new JsonResult(new { Error = "To see your posts info, use specific popularity!" });
            }

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == rp)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this post, as you are blocked by its user!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == rp)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this post, as you are blocking its user!" });
            }

            List<Comment> Comments = _db.Comments.Where(u => u.PostID == payload.PostID).OrderByDescending(u => u.Creation).ToList();
            
            if (Comments is null || Comments.Count == 0)
            {
                return new JsonResult(new { Error = "The searched post has not been commented on by anyone yet!" });
            }

            List<int> eb1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> eb2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            Comments = _db.Comments.Where(u => u.PostID == payload.PostID && !eb1.Contains(u.UserID) && !eb2.Contains(u.UserID)).OrderByDescending(u => u.Creation).ToList();
            
            if (Comments is null || Comments.Count == 0)
            {
                return new JsonResult(new { Error = "The searched post has been commented only by users you blocked, or those that blocked you!" });
            }

            else
            {
                return new JsonResult(new { Message = "Post comments:", Comments_received = Comments.Count, Comments });
            }
        }


        [HttpPost("SearchPostLikes")]//search a post's likes
        public ActionResult SearchPostLikes([FromBody] SearchPostInfoDTO payload)
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

            int? rp = _db.Posts
                .Where(u => u.ID == payload.PostID).Select(u => u.UserID)
                .SingleOrDefault();

            if (registeredPost is null || registeredPost == 0 || rp is null || rp == 0)
            {
                return new JsonResult(new { Error = "There is no post with the specified ID!" });
            }

            if (rp == userID)
            {
                return new JsonResult(new { Error = "To see your posts info, use specific popularity!" });
            }

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == rp)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this post, as you are blocked by its user!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == rp)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this post, as you are blocking its user!" });
            }

            List<Like> Likes = _db.Likes.Where(u => u.PostID == payload.PostID).OrderByDescending(u => u.Creation).ToList();
            
            if (Likes is null || Likes.Count == 0)
            {
                return new JsonResult(new { Error = "The searched post has not been liked by anyone yet!" });
            }

            List<int> eb1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> eb2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            Likes = _db.Likes.Where(u => u.PostID == payload.PostID && !eb1.Contains(u.UserID) && !eb2.Contains(u.UserID)).OrderByDescending(u => u.Creation).ToList();
            
            if (Likes is null || Likes.Count == 0)
            {
                return new JsonResult(new { Error = "The searched post has been liked only by users you blocked, or those that blocked you!" });
            }

            else
            {
                return new JsonResult(new { Message = "Post likes:", Likes_received = Likes.Count, Likes });
            }
        }


        [HttpPost("SearchPostDislikes")]//search a post's dislikes
        public ActionResult SearchPostDislikes([FromBody] SearchPostInfoDTO payload)
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

            int? rp = _db.Posts
                .Where(u => u.ID == payload.PostID).Select(u => u.UserID)
                .SingleOrDefault();

            if (registeredPost is null || registeredPost == 0 || rp is null || rp == 0)
            {
                return new JsonResult(new { Error = "There is no post with the specified ID!" });
            }

            if (rp == userID)
            {
                return new JsonResult(new { Error = "To see your posts info, use specific popularity!" });
            }

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == rp)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this post, as you are blocked by its user!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == rp)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this post, as you are blocking its user!" });
            }

            List<Dislike> Dislikes = _db.Dislikes.Where(u => u.PostID == payload.PostID).OrderByDescending(u => u.Creation).ToList();
            
            if (Dislikes is null || Dislikes.Count == 0)
            {
                return new JsonResult(new { Error = "The searched post has not been disliked by anyone yet!" });
            }

            List<int> eb1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> eb2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            Dislikes = _db.Dislikes.Where(u => u.PostID == payload.PostID && !eb1.Contains(u.UserID) && !eb2.Contains(u.UserID)).OrderByDescending(u => u.Creation).ToList();
            
            if (Dislikes is null || Dislikes.Count == 0)
            {
                return new JsonResult(new { Error = "The searched post has been disliked only by users you blocked, or those that blocked you!" });
            }

            else
            {
                return new JsonResult(new { Message = "Post dislikes:", Dislikes_received = Dislikes.Count, Dislikes });
            }
        }
    }
}
