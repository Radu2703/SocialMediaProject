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
    public class SearchUserInfoController : ControllerBase
    {
        private readonly SocialMediaProjectDb _db;
        private readonly IConfiguration _config;

        public SearchUserInfoController(SocialMediaProjectDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("SearchUserID")]//search an user's account by ID (an unique attribute)
        public ActionResult SearchUserID([FromBody] SearchUserIDDTO payload)
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

            User? registeredUser = _db.Users
                .Where(u => u.ID == payload.UserID)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return new JsonResult(new { Error = "There is no user with the specified ID!" });
            }

            if (registeredUser.ID == userID)
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

            else
            {
                return new JsonResult(new {
                    Message = "User's account:",
                    registeredUser.ID, registeredUser.Username, registeredUser.Description, registeredUser.Country,
                registeredUser.City, registeredUser.Gender, birthdate = DateOnly.FromDateTime(registeredUser.Birthdate), registeredUser.Creation });
            }
        }


        [HttpPost("SearchUserUsername")]//search an user's account by username (an unique attribute)
        public ActionResult SearchUserUsername([FromBody] SearchUserDTO payload)
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

            if (String.IsNullOrEmpty(payload.Data))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            User? registeredUser = _db.Users
                .Where(u => u.Username == payload.Data)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return new JsonResult(new { Error = "There is no user with the specified username!" });
            }

            if (registeredUser.ID == userID)
            {
                return new JsonResult(new { Error = "To see your account info, use personal (main) history!" });
            }

            int data = registeredUser.ID;

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == data)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocked by them!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == data)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocking them!" });
            }

            else
            {
                return new JsonResult(new
                {
                    Message = "User's account:",
                    registeredUser.ID,
                    registeredUser.Username,
                    registeredUser.Description,
                    registeredUser.Country,
                    registeredUser.City,
                    registeredUser.Gender,
                    birthdate = DateOnly.FromDateTime(registeredUser.Birthdate),
                    registeredUser.Creation
                });
            }
        }


        [HttpPost("SearchUserEmail")]//search an user's account by email (an unique attribute)
        public ActionResult SearchUserEmail([FromBody] SearchUserDTO payload)
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

            if (String.IsNullOrEmpty(payload.Data))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            if (payload.Data.StartsWith('@') || payload.Data.EndsWith('@') || !payload.Data.Contains('@'))
            {
                return new JsonResult(new { Error = "The email must contain a name, the @ character, and a web domain!" });
            }

            User? registeredUser = _db.Users
                .Where(u => u.Email == payload.Data)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return new JsonResult(new { Error = "There is no user with the specified email!" });
            }

            if (registeredUser.ID == userID)
            {
                return new JsonResult(new { Error = "To see your account info, use personal (main) history!" });
            }

            int data = registeredUser.ID;

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == data)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocked by them!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == data)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocking them!" });
            }

            else
            {
                return new JsonResult(new
                {
                    Message = "User's account:",
                    registeredUser.ID,
                    registeredUser.Username,
                    registeredUser.Description,
                    registeredUser.Name,
                    registeredUser.Country,
                    registeredUser.City,
                    registeredUser.Gender,
                    birthdate = DateOnly.FromDateTime(registeredUser.Birthdate),
                    registeredUser.Creation
                });
            }
        }


        [HttpPost("SearchUserPhoneNumber")]//search an user's account by phone number (an unique attribute)
        public ActionResult SearchUserPhoneNumber([FromBody] SearchUserDTO payload)
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

            if (String.IsNullOrEmpty(payload.Data))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            try
            {
                ulong pn = Convert.ToUInt64(payload.Data);
            }
            catch (OverflowException)
            {
                return new JsonResult(new { Error = "The phone number is too big!" });
            }
            catch (FormatException)
            {
                return new JsonResult(new { Error = "The phone number must be a positive number!" });
            }

            if (payload.Data.Length != 10)
            {
                return new JsonResult(new { Error = "The phone number must contain exactly 10 digits (without anything inbetween)!" });
            }

            User? registeredUser = _db.Users
                .Where(u => u.PhoneNumber == payload.Data)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return new JsonResult(new { Error = "There is no user with the specified phone number!" });
            }

            if (registeredUser.ID == userID)
            {
                return new JsonResult(new { Error = "To see your account info, use personal (main) history!" });
            }

            int data = registeredUser.ID;

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == data)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocked by them!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == data)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocking them!" });
            }

            else
            {
                return new JsonResult(new
                {
                    Message = "User's account:",
                    registeredUser.ID,
                    registeredUser.Username,
                    registeredUser.Description,
                    registeredUser.Name,
                    registeredUser.Country,
                    registeredUser.City,
                    registeredUser.Gender,
                    birthdate = DateOnly.FromDateTime(registeredUser.Birthdate),
                    registeredUser.Creation
                });
            }
        }


        [HttpPost("SearchUserDescription")]//search an user's account by description (an unique attribute)
        public ActionResult SearchUserDescription([FromBody] SearchUserDTO payload)
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

            if (String.IsNullOrEmpty(payload.Data))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            User? registeredUser = _db.Users
                .Where(u => u.Description.Equals(payload.Data))
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return new JsonResult(new { Error = "There is no user with the specified description!" });
            }

            if (registeredUser.ID == userID)
            {
                return new JsonResult(new { Error = "To see your account info, use personal (main) history!" });
            }

            int data = registeredUser.ID;

            Block? existingBlock1 = _db.Blocks
                .Where(u => u.BlockedID == userID && u.BlockerID == data)
                .SingleOrDefault();

            if (existingBlock1 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocked by them!" });
            }

            Block? existingBlock2 = _db.Blocks
                .Where(u => u.BlockerID == userID && u.BlockedID == data)
                .SingleOrDefault();

            if (existingBlock2 is not null)
            {
                return new JsonResult(new { Error = "You can not search info about this user, as you are blocking them!" });
            }

            else
            {
                return new JsonResult(new
                {
                    Message = "User's account:",
                    registeredUser.ID,
                    registeredUser.Username,
                    registeredUser.Description,
                    registeredUser.Country,
                    registeredUser.City,
                    registeredUser.Gender,
                    birthdate = DateOnly.FromDateTime(registeredUser.Birthdate),
                    registeredUser.Creation
                });
            }
        }


        [HttpPost("SearchUserName")]//search users' accounts by name (not an unique attribute)
        public ActionResult SearchUserName([FromBody] SearchUserDTO payload)
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

            if (String.IsNullOrEmpty(payload.Data))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            List<string> opt = _db.Users.Where(u => u.Name.Equals(payload.Data)).OrderBy(u => u.Username).Select(u => u.Username).ToList();
            if (opt is null || opt.Count == 0)
            {
                return new JsonResult(new { Error = "There is no user with the specified name!" });
            }

            List<string> owne = _db.Users.Where(u => u.Name.Equals(payload.Data) && u.ID!=userID).OrderBy(u => u.Username).Select(u => u.Username).ToList();
            if (owne is null || owne.Count == 0)
            {
                return new JsonResult(new { Error = "To see your account info, use personal (main) history!" });
            }

            List <int> existingBlock1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List <int> existingBlock2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            List<string> userNames = _db.Users.Where(u => u.Name.Equals(payload.Data) && u.ID != userID && !existingBlock1.Contains(u.ID) && !existingBlock2.Contains(u.ID)).OrderBy(u => u.Username).Select(u => u.Username).ToList();

            if (userNames is null || userNames.Count == 0)
            {
                return new JsonResult(new { Error = "The users with the specified name blocked you, or you blocked them!" });
            }

            else
            {
                return new JsonResult(new { Message = "User accounts:", user_count = userNames.Count, userNames } );
            }
        }


        [HttpPost("SearchUserCountry")]//search users' accounts by country (not an unique attribute)
        public ActionResult SearchUserCountry([FromBody] SearchUserDTO payload)
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

            if (String.IsNullOrEmpty(payload.Data))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            List<string> opt = _db.Users.Where(u => u.Country.Equals(payload.Data)).OrderBy(u => u.Username).Select(u => u.Username).ToList();
            if (opt is null || opt.Count == 0)
            {
                return new JsonResult(new { Error = "There is no user with the specified country!" });
            }

            List<string> owne = _db.Users.Where(u => u.Country.Equals(payload.Data) && u.ID != userID).OrderBy(u => u.Username).Select(u => u.Username).ToList();
            if (owne is null || owne.Count == 0)
            {
                return new JsonResult(new { Error = "To see your account info, use personal (main) history!" });
            }

            List<int> existingBlock1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> existingBlock2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            List<string> userNames = _db.Users.Where(u => u.Country.Equals(payload.Data) && u.ID != userID && !existingBlock1.Contains(u.ID) && !existingBlock2.Contains(u.ID)).OrderBy(u => u.Username).Select(u => u.Username).ToList();

            if (userNames is null || userNames.Count == 0)
            {
                return new JsonResult(new { Error = "The users with the specified country blocked you, or you blocked them!" });
            }

            else
            {
                return new JsonResult(new { Message = "User accounts:", user_count = userNames.Count, userNames });
            }
        }


        [HttpPost("SearchUserCity")]//search users' accounts by city (not an unique attribute)
        public ActionResult SearchUserCity([FromBody] SearchUserDTO payload)
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

            if (String.IsNullOrEmpty(payload.Data))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            List<string> opt = _db.Users.Where(u => u.City.Equals(payload.Data)).OrderBy(u => u.Username).Select(u => u.Username).ToList();
            if (opt is null || opt.Count == 0)
            {
                return new JsonResult(new { Error = "There is no user with the specified city!" });
            }

            List<string> owne = _db.Users.Where(u => u.City.Equals(payload.Data) && u.ID != userID).OrderBy(u => u.Username).Select(u => u.Username).ToList();
            if (owne is null || owne.Count == 0)
            {
                return new JsonResult(new { Error = "To see your account info, use personal (main) history!" });
            }

            List<int> existingBlock1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> existingBlock2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            List<string> userNames = _db.Users.Where(u => u.City.Equals(payload.Data) && u.ID != userID && !existingBlock1.Contains(u.ID) && !existingBlock2.Contains(u.ID)).OrderBy(u => u.Username).Select(u => u.Username).ToList();

            if (userNames is null || userNames.Count == 0)
            {
                return new JsonResult(new { Error = "The users with the specified city blocked you, or you blocked them!" });
            }

            else
            {
                return new JsonResult(new { Message = "User accounts:", user_count = userNames.Count, userNames });
            }
        }


        [HttpPost("SearchUserGender")]//search users' accounts by gender (not an unique attribute)
        public ActionResult SearchUserGender([FromBody] SearchUserDTO payload)
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

            if (String.IsNullOrEmpty(payload.Data))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            if (!payload.Data.Equals("Male") && !payload.Data.Equals("Female"))
            {
                return new JsonResult(new { Error = "Gender must be written as: 'Male' or 'Female'!" });
            }

            List<string> opt = _db.Users.Where(u => u.Gender.Equals(payload.Data)).OrderBy(u => u.Username).Select(u => u.Username).ToList();
            if (opt is null || opt.Count == 0)
            {
                return new JsonResult(new { Error = "There is no user with the specified gender!" });
            }

            List<string> owne = _db.Users.Where(u => u.Gender.Equals(payload.Data) && u.ID != userID).OrderBy(u => u.Username).Select(u => u.Username).ToList();
            if (owne is null || owne.Count == 0)
            {
                return new JsonResult(new { Error = "To see your account info, use personal (main) history!" });
            }

            List<int> existingBlock1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> existingBlock2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            List<string> userNames = _db.Users.Where(u => u.Gender.Equals(payload.Data) && u.ID != userID && !existingBlock1.Contains(u.ID) && !existingBlock2.Contains(u.ID)).OrderBy(u => u.Username).Select(u => u.Username).ToList();

            if (userNames is null || userNames.Count == 0)
            {
                return new JsonResult(new { Error = "The users with the specified gender blocked you, or you blocked them!" });
            }

            else
            {
                return new JsonResult(new { Message = "User accounts:", user_count = userNames.Count, userNames });
            }
        }


        [HttpPost("SearchUserBirthdate")]//search users' accounts by birthdate (not an unique attribute)
        public ActionResult SearchUserBirthdate([FromBody] SearchUserDateDTO payload)
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

            DateTime bd;
            try
            {
                bd = new(payload.Data.Year, payload.Data.Month, payload.Data.Day, 0, 0, 0);
            }
            catch (FormatException e) { return new JsonResult(new { Error = e.Message }); }

            DateOnly dex = DateOnly.FromDateTime(DateTime.Now);

            if ((dex.Year - bd.Year < 10) || (dex.Year - bd.Year == 10 && dex.Month - bd.Month < 0) || (dex.Year - bd.Year == 10 && dex.Month - bd.Month == 0 && dex.Day - bd.Day < 0))
                return new JsonResult(new { Error = "There can not be any user with this birthdate! You need to be at least 10 years old in order to use this website!" });

            List<string> opt = _db.Users.Where(u => u.Birthdate.Equals(bd)).OrderBy(u => u.Username).Select(u => u.Username).ToList();
            if (opt is null || opt.Count == 0)
            {
                return new JsonResult(new { Error = "There is no user with the specified birthdate!" });
            }

            List<string> owne = _db.Users.Where(u => u.Birthdate.Equals(bd) && u.ID != userID).OrderBy(u => u.Username).Select(u => u.Username).ToList();
            if (owne is null || owne.Count == 0)
            {
                return new JsonResult(new { Error = "To see your account info, use personal (main) history!" });
            }

            List<int> existingBlock1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> existingBlock2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            List<string> userNames = _db.Users.Where(u => u.Birthdate.Equals(bd) && u.ID != userID && !existingBlock1.Contains(u.ID) && !existingBlock2.Contains(u.ID)).OrderBy(u => u.Username).Select(u => u.Username).ToList();

            if (userNames is null || userNames.Count == 0)
            {
                return new JsonResult(new { Error = "The users with the specified birthdate blocked you, or you blocked them!" });
            }

            else
            {
                return new JsonResult(new { Message = "User accounts:", user_count = userNames.Count, userNames });
            }
        }


        [HttpPost("SearchUserCreation")]//search users' accounts by account creation date (not an unique attribute)
        public ActionResult SearchUserCreation([FromBody] SearchUserDateDTO payload)
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

            DateTime cd;
            try
            {
                cd = new(payload.Data.Year, payload.Data.Month, payload.Data.Day, 0, 0, 0);
            }
            catch (FormatException e) { return new JsonResult(new { Error = e.Message }); }

            List<string> opt = _db.Users.Where(u => u.Creation.Date.Equals(cd)).OrderBy(u => u.Username).Select(u => u.Username).ToList();
            if (opt is null || opt.Count == 0)
            {
                return new JsonResult(new { Error = "There is no user with the specified account creation date!" });
            }

            List<string> owne = _db.Users.Where(u => u.Creation.Date.Equals(cd) && u.ID != userID).OrderBy(u => u.Username).Select(u => u.Username).ToList();
            if (owne is null || owne.Count == 0)
            {
                return new JsonResult(new { Error = "To see your account info, use personal (main) history!" });
            }

            List<int> existingBlock1 = _db.Blocks.Where(u => u.BlockerID == userID).Select(u => u.BlockedID).ToList();
            List<int> existingBlock2 = _db.Blocks.Where(u => u.BlockedID == userID).Select(u => u.BlockerID).ToList();

            List<string> userNames = _db.Users.Where(u => u.Creation.Date.Equals(cd) && u.ID != userID && !existingBlock1.Contains(u.ID) && !existingBlock2.Contains(u.ID)).OrderBy(u => u.Username).Select(u => u.Username).ToList();

            if (userNames is null || userNames.Count == 0)
            {
                return new JsonResult(new { Error = "The users with the specified account creation date blocked you, or you blocked them!" });
            }

            else
            {
                return new JsonResult(new { Message = "User accounts:", user_count = userNames.Count, userNames });
            }
        }
    }
}
