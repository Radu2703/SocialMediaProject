using Microsoft.EntityFrameworkCore;
using SocialMediaProject.Data.Entities;

namespace SocialMediaProject.Data
{
    public class SocialMediaProjectDb :DbContext
    {
        public SocialMediaProjectDb(DbContextOptions<SocialMediaProjectDb> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Like> Likes { get; set; }

        public DbSet<Dislike> Dislikes { get; set; }

        public DbSet<Share> Shares { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Block> Blocks { get; set; }

        public DbSet<Follow> Follows { get; set; }

        public DbSet<Message> Messages { get; set; }
    }
}
