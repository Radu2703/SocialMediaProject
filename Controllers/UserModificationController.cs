using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMediaProject.Data;
using SocialMediaProject.Data.Entities;
using SocialMediaProject.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace SocialMediaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserModificationController : ControllerBase
    {
        private readonly SocialMediaProjectDb _db;
        private readonly IConfiguration _config;

        public UserModificationController(SocialMediaProjectDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("ModifyUsername")]//modify username
        public ActionResult ModifyUsername([FromBody] ModifyUserDTO payload)
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

            if (String.IsNullOrEmpty(payload.NewData))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            User? loggedUser = _db.Users
                .Where(u => u.ID == userID)
                .SingleOrDefault();

            if (loggedUser.Username.Equals(payload.NewData))
            {
                return new JsonResult(new { Error = "There is no difference between the old username and the new username!" });
            }

            User? newUser = _db.Users
                .Where(u => u.Username.Equals(payload.NewData) && u.ID != userID)
                .SingleOrDefault();

            if (newUser is not null)
            {
                return new JsonResult(new { Error = "The username is already registered in the database by someone else! Please choose something else." });
            }

            loggedUser.Username = payload.NewData;

            _db.SaveChanges();

            User? registeredUser = _db.Users
                .Where(u => u.Username == payload.NewData && u.ID == userID)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You have successfully changed your username!", UserID = userID, newUsername = payload.NewData });
            }
        }


        [HttpPost("ModifyPassword")]//modify password
        public ActionResult ModifyPassword([FromBody] ModifyUserDTO payload)
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

            if (String.IsNullOrEmpty(payload.NewData))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            string base64HashedPasswordBytes;
            byte[] passwordBytes = Encoding.UTF8.GetBytes(payload.NewData);
            byte[] hashedPasswordBytes = SHA256.HashData(passwordBytes);
            base64HashedPasswordBytes = Convert.ToBase64String(hashedPasswordBytes);

            User? loggedUser = _db.Users
                .Where(u => u.ID == userID)
                .SingleOrDefault();

            if (loggedUser.HashedPassword.Equals(base64HashedPasswordBytes))
            {
                return new JsonResult(new { Error = "There is no difference between the old password and the new password!" });
            }

            User? newUser = _db.Users
                .Where(u => u.HashedPassword.Equals(base64HashedPasswordBytes) && u.ID != userID)
                .SingleOrDefault();

            if (newUser is not null)
            {
                return new JsonResult(new { Error = "The password is already registered in the database by someone else! Please choose something else." });
            }

            loggedUser.HashedPassword = base64HashedPasswordBytes;

            _db.SaveChanges();

            User? registeredUser = _db.Users
                .Where(u => u.HashedPassword == base64HashedPasswordBytes && u.ID == userID)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You have successfully changed your password!", UserID = userID, newPassword = payload.NewData });
            }
        }


        [HttpPost("ModifyEmail")]//modify email
        public ActionResult ModifyEmail([FromBody] ModifyUserDTO payload)
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

            if (String.IsNullOrEmpty(payload.NewData))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            if (payload.NewData.StartsWith('@') || payload.NewData.EndsWith('@') || !payload.NewData.Contains('@'))
            {
                return new JsonResult(new { Error = "The new email must contain a name, the @ character, and a web domain!" });
            }

            User? loggedUser = _db.Users
                .Where(u => u.ID == userID)
                .SingleOrDefault();

            if (loggedUser.Email.Equals(payload.NewData))
            {
                return new JsonResult(new { Error = "There is no difference between the old email and the new email!" });
            }

            User? newUser = _db.Users
                .Where(u => u.Email.Equals(payload.NewData) && u.ID != userID)
                .SingleOrDefault();

            if (newUser is not null)
            {
                return new JsonResult(new { Error = "The email is already registered in the database by someone else! Please choose something else." });
            }

            loggedUser.Email = payload.NewData;

            _db.SaveChanges();

            User? registeredUser = _db.Users
                .Where(u => u.Email == payload.NewData && u.ID == userID)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You have successfully changed your email!", UserID = userID, newEmail = payload.NewData });
            }
        }


        [HttpPost("ModifyPhoneNumber")]//modify phone number
        public ActionResult ModifyPhoneNumber([FromBody] ModifyUserDTO payload)
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

            if (String.IsNullOrEmpty(payload.NewData))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            try
            {
                ulong pn = Convert.ToUInt64(payload.NewData);
            }
            catch (OverflowException)
            {
                return new JsonResult(new { Error = "The new phone number is too big!" });
            }
            catch (FormatException)
            {
                return new JsonResult(new { Error = "The new phone number must be a positive number!" });
            }

            if (payload.NewData.Length != 10)
            {
                return new JsonResult(new { Error = "The new phone number must contain exactly 10 digits (without anything inbetween)!" });
            }

            User? loggedUser = _db.Users
                .Where(u => u.ID == userID)
                .SingleOrDefault();

            if (loggedUser.PhoneNumber.Equals(payload.NewData))
            {
                return new JsonResult(new { Error = "There is no difference between the old phone number and the new phone number!" });
            }

            User? newUser = _db.Users
                .Where(u => u.PhoneNumber.Equals(payload.NewData) && u.ID != userID)
                .SingleOrDefault();

            if (newUser is not null)
            {
                return new JsonResult(new { Error = "The phone number is already registered in the database by someone else! Please correct it." });
            }

            loggedUser.PhoneNumber = payload.NewData;

            _db.SaveChanges();

            User? registeredUser = _db.Users
                .Where(u => u.PhoneNumber == payload.NewData && u.ID == userID)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You have successfully changed your phone number!", UserID = userID, newPhoneNumber = payload.NewData });
            }
        }


        [HttpPost("ModifyDescription")]//modify description
        public ActionResult ModifyDescription([FromBody] ModifyUserDTO payload)
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

            if (String.IsNullOrEmpty(payload.NewData))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            User? loggedUser = _db.Users
                .Where(u => u.ID == userID)
                .SingleOrDefault();

            if (loggedUser.Description.Equals(payload.NewData))
            {
                return new JsonResult(new { Error = "There is no difference between the old description and the new description!" });
            }

            User? newUser = _db.Users
                .Where(u => u.Description.Equals(payload.NewData) && u.ID != userID)
                .SingleOrDefault();

            if (newUser is not null)
            {
                return new JsonResult(new { Error = "The description is already registered in the database by someone else! Please change it." });
            }

            loggedUser.Description = payload.NewData;

            _db.SaveChanges();

            User? registeredUser = _db.Users
                .Where(u => u.Description == payload.NewData && u.ID == userID)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You have successfully changed your description!", UserID = userID, newDescription = payload.NewData });
            }
        }


        [HttpPost("ModifyName")]//modify name
        public ActionResult ModifyName([FromBody] ModifyUserDTO payload)
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

            if (String.IsNullOrEmpty(payload.NewData))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            User? loggedUser = _db.Users
                .Where(u => u.ID == userID)
                .SingleOrDefault();

            if (loggedUser.Name.Equals(payload.NewData))
            {
                return new JsonResult(new { Error = "There is no difference between the old name and the new name!" });
            }

            loggedUser.Name = payload.NewData;

            _db.SaveChanges();

            User? registeredUser = _db.Users
                .Where(u => u.Name == payload.NewData && u.ID == userID)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You have successfully changed your name!", UserID = userID, newName = payload.NewData });
            }
        }


        [HttpPost("ModifyCountry")]//modify country
        public ActionResult ModifyCountry([FromBody] ModifyUserDTO payload)
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

            if (String.IsNullOrEmpty(payload.NewData))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            User? loggedUser = _db.Users
                .Where(u => u.ID == userID)
                .SingleOrDefault();

            if (loggedUser.Country.Equals(payload.NewData))
            {
                return new JsonResult(new { Error = "There is no difference between the old country and the new country!" });
            }

            loggedUser.Country = payload.NewData;

            _db.SaveChanges();

            User? registeredUser = _db.Users
                .Where(u => u.Country == payload.NewData && u.ID == userID)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You have successfully changed your country!", UserID = userID, newCountry = payload.NewData });
            }
        }


        [HttpPost("ModifyCity")]//modify city
        public ActionResult ModifyCity([FromBody] ModifyUserDTO payload)
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

            if (String.IsNullOrEmpty(payload.NewData))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            User? loggedUser = _db.Users
                .Where(u => u.ID == userID)
                .SingleOrDefault();

            if (loggedUser.City.Equals(payload.NewData))
            {
                return new JsonResult(new { Error = "There is no difference between the old city and the new city!" });
            }

            loggedUser.City = payload.NewData;

            _db.SaveChanges();

            User? registeredUser = _db.Users
                .Where(u => u.City == payload.NewData && u.ID == userID)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You have successfully changed your city!", UserID = userID, newCity = payload.NewData });
            }
        }


        [HttpGet("ModifyGender")]//modify gender
        public ActionResult ModifyGender()
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

            User? loggedUser = _db.Users
                .Where(u => u.ID == userID)
                .SingleOrDefault();

            if (loggedUser.Gender.Equals("Male"))
            {
                loggedUser.Gender = "Female";
            }
            else if (loggedUser.Gender.Equals("Female"))
            {
                loggedUser.Gender = "Male";
            }

            _db.SaveChanges();

            User? registeredUser = _db.Users
                .Where(u => u.ID == userID)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You have successfully changed your gender!", UserID = userID, newGender = registeredUser.Gender });
            }
        }


        [HttpPost("ModifyBirthdate")]//modify birthdate
        public ActionResult ModifyBirthdate([FromBody] ModifyUserBirthdateDTO payload)
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

            if (String.IsNullOrEmpty(payload.NewData.ToString()))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            DateTime bd;
            try
            {
                bd = new(payload.NewData.Year, payload.NewData.Month, payload.NewData.Day, 0, 0, 0);
            }
            catch (Exception ex) { return new JsonResult(new { Error = ex.Message }); }

            User? loggedUser = _db.Users
                .Where(u => u.ID == userID)
                .SingleOrDefault();

            if (DateOnly.FromDateTime(loggedUser.Birthdate).Equals(payload.NewData))
            {
                return new JsonResult(new { Error = "There is no difference between the old birthdate and the new birthdate!" });
            }

            DateOnly dex = DateOnly.FromDateTime(DateTime.Now);

            if ((dex.Year - bd.Year < 10) || (dex.Year - bd.Year == 10 && dex.Month - bd.Month < 0) || (dex.Year - bd.Year == 10 && dex.Month - bd.Month == 0 && dex.Day - bd.Day < 0))
                return new JsonResult(new { Error = "You need to be at least 10 years old in order to use this app!" });

            loggedUser.Birthdate = bd;

            _db.SaveChanges();

            User? registeredUser = _db.Users
                .Where(u => u.ID == userID)
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You have successfully changed your birthdate!", UserID = userID, newBirthdate = DateOnly.FromDateTime(registeredUser.Birthdate) });
            }
        }
    }
}
