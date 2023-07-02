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
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly SocialMediaProjectDb _db;
        private readonly IConfiguration _config;

        public AccountController(SocialMediaProjectDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("Login")] //log into an account
        public ActionResult Login([FromBody] LoginDTO payload)
        {
            string base64HashedPasswordBytes;
            byte[] passwordBytes = Encoding.UTF8.GetBytes(payload.Password);
            byte[] hashedPasswordBytes = SHA256.HashData(passwordBytes);
            base64HashedPasswordBytes = Convert.ToBase64String(hashedPasswordBytes);

            User? existingUser = _db.Users
                .Where(u => (u.Username.Equals(payload.Userdata) && u.HashedPassword.Equals(base64HashedPasswordBytes)) ||
                            (u.Email.Equals(payload.Userdata) && u.HashedPassword.Equals(base64HashedPasswordBytes)) ||
                            (u.PhoneNumber.Equals(payload.Userdata) && u.HashedPassword.Equals(base64HashedPasswordBytes)))
                .SingleOrDefault();

            if (existingUser is null)
            {
                try
                {
                    existingUser = _db.Users
                    .Where(u => (u.ID == int.Parse(payload.Userdata) && u.HashedPassword.Equals(base64HashedPasswordBytes))).SingleOrDefault();
                }
                catch (Exception) { return NotFound(); }
            }

            if (existingUser is null)
            {
                return NotFound();
            }
            else
            {
                var jwt = GenerateJSONWebToken(existingUser);

                return new JsonResult(new { Message = "You have successfully logged in!", jwt });
            }
        }

        private string GenerateJSONWebToken(User userInfo)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new[] {
                new Claim("ID", userInfo.ID.ToString()),
                new Claim("Username", userInfo.Username),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            JwtSecurityToken token = new JwtSecurityToken(_config["JWT:Issuer"],
                _config["JWT:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("Register")] //create an account
        public ActionResult Register([FromBody] RegisterDTO payload)
        {
            if (String.IsNullOrEmpty(payload.Username) || String.IsNullOrEmpty(payload.Password) || String.IsNullOrEmpty(payload.Email)
                || String.IsNullOrEmpty(payload.PhoneNumber) || String.IsNullOrEmpty(payload.Description) || String.IsNullOrEmpty(payload.Name)
                || String.IsNullOrEmpty(payload.Country) || String.IsNullOrEmpty(payload.City) || String.IsNullOrEmpty(payload.Gender) || String.IsNullOrEmpty(payload.Birthdate.ToString()))
            {
                return new JsonResult(new { Error = "All of the fields must be filled!" });
            }

            try {
                ulong pn = Convert.ToUInt64(payload.PhoneNumber);
            }
            catch (OverflowException)
            {
                return new JsonResult(new { Error = "Phone number is too big!" });
            }
            catch (FormatException)
            {
                return new JsonResult(new { Error = "Phone number must be a positive number!" });
            }

            if (payload.PhoneNumber.Length != 10)
            {
                return new JsonResult(new { Error = "Phone number must contain exactly 10 digits (without anything inbetween)!" });
            }

            if (payload.Email.StartsWith('@') || payload.Email.EndsWith('@') || !payload.Email.Contains('@'))
            {
                return new JsonResult(new { Error = "Email must contain a name, the @ character, and a web domain!" });
            }

            if (!payload.Gender.Equals("Male") && !payload.Gender.Equals("Female"))
            {
                return new JsonResult(new { Error = "Gender must be written as: 'Male' or 'Female'!" });
            }

            string base64HashedPasswordBytes;
            byte[] passwordBytes = Encoding.UTF8.GetBytes(payload.Password);
            byte[] hashedPasswordBytes = SHA256.HashData(passwordBytes);
            base64HashedPasswordBytes = Convert.ToBase64String(hashedPasswordBytes);

            User? existingUser1 = _db.Users
                .Where(u => u.Username == payload.Username)
                .SingleOrDefault();

            User? existingUser2 = _db.Users
                .Where(u => u.HashedPassword == base64HashedPasswordBytes)
                .SingleOrDefault();

            User? existingUser3 = _db.Users
                .Where(u => u.Email == payload.Email)
                .SingleOrDefault();

            User? existingUser4 = _db.Users
                .Where(u => u.PhoneNumber == payload.PhoneNumber)
                .SingleOrDefault();

            User? existingUser5 = _db.Users
                .Where(u => u.Description == payload.Description)
                .SingleOrDefault();

            if (existingUser1 is not null)
            {
                return new JsonResult(new { Error = "Username is already registered in the database!" });
            }

            if (existingUser2 is not null)
            {
                return new JsonResult(new { Error = "Password is already registered in the database!" });
            }

            if (existingUser3 is not null)
            {
                return new JsonResult(new { Error = "Email is already registered in the database!" });
            }

            if (existingUser4 is not null)
            {
                return new JsonResult(new { Error = "Phone number is already registered in the database!" });
            }

            if (existingUser5 is not null)
            {
                return new JsonResult(new { Error = "Description is already registered in the database! Please write something else!" });
            }

            DateTime bd,ct;
            try
            {
                bd = new(payload.Birthdate.Year, payload.Birthdate.Month, payload.Birthdate.Day, 0, 0, 0);
                ct = DateTime.Now;
            }
            catch (Exception ex) { return new JsonResult(new { Error = ex.Message }); }

            DateOnly dex = DateOnly.FromDateTime(ct);
           
            if ((dex.Year-bd.Year<10)||(dex.Year-bd.Year==10 && dex.Month-bd.Month<0)||(dex.Year - bd.Year == 10 && dex.Month - bd.Month == 0 && dex.Day - bd.Day < 0))
                return new JsonResult(new { Error = "You need to be at least 10 years old in order to use this app!" });

            _db.Users.Add(new Data.Entities.User { Username = payload.Username, HashedPassword = base64HashedPasswordBytes, Email = payload.Email,
                PhoneNumber = payload.PhoneNumber, Description = payload.Description, Name = payload.Name, Country = payload.Country,
                City = payload.City, Gender = payload.Gender, Birthdate = bd, Creation = ct
            });

            _db.SaveChanges();


            User? registeredUser = _db.Users
                .Where(u => u.Username.Equals(payload.Username) && u.HashedPassword.Equals(base64HashedPasswordBytes) && u.Email.Equals(payload.Email)
                && u.PhoneNumber.Equals(payload.PhoneNumber) && u.Description.Equals(payload.Description) && u.Name.Equals(payload.Name)
                && u.Country.Equals(payload.Country) && u.City.Equals(payload.City) && u.Gender.Equals(payload.Gender) && u.Birthdate.Equals(bd)
                && u.Creation.Equals(ct))
                .SingleOrDefault();

            if (registeredUser is null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(new { Message = "You have successfully registered!", registeredUser });
            }
        }
    }
}
