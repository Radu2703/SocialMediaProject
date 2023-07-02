using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialMediaProject.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };
    });

builder.Services.AddDbContext<SocialMediaProjectDb>(optionsAction 
   => optionsAction.UseSqlServer(connectionString: builder.Configuration.GetConnectionString("SocialMediaProjectDb")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//-----------------------------------------------------------------------------------------------------------------------------------------------------

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

/* You can't search for activity of the people you blocked, or those that blocked you. To see your interactions with them, use the 'specific' controller methods.
 
   You can't see an user's real name, unless you search them by a personal data, like email or phone number. Also, you can't find their email or phone number, to prevent harrasment.

   Liking a post will remove the dislike on the post, if you made one, and vice-versa. Blocking someone, or being blocked by someone will remove any existing follow relationship between the users.

   On top of everything, a block prevents either party from liking, disliking, sharing, or commenting on a post. They also can't send messages, nor follow each other.

   An user can't like, dislike, or share their own posts, but they can comment on them. An user can only like or dislike a post once, but they can comment and share them as many times as they want.

   An user's ID (by default), username, password, email, phone number and description are all unique in the database, while the rest is not.

   The attribute 'creation' refers to the date and time something was created, whether it is an account, a post, etc. */
