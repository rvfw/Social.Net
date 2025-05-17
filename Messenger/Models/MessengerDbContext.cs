using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Messenger.Models;
public class MessengerDbContext(DbContextOptions<MessengerDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>()
       .HasMany(p => p.Posts)
       .WithOne(c => c.User)
       .HasForeignKey(e => e.UserId)
       .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<User>()
       .HasMany(p => p.Comments)
       .WithOne(c => c.User)
       .HasForeignKey(e => e.UserId)
       .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Post>()
       .HasMany(p => p.Comments)
       .WithOne(c => c.Post)
       .HasForeignKey(e => e.PostId)
       .OnDelete(DeleteBehavior.Cascade);
    }
}
public class Post
{
    [Key]
    public int Id { get; init; }
    public string Text { get; set; }
    public int UserId { get; init; }
    [ForeignKey(nameof(UserId))]
    [JsonIgnore]
    public User User { get; init; }
    public int Likes { get { return LikesBy.Count; } }
    public List<int> LikesBy { get; init; }
    public ICollection<Comment> Comments;
    public Post(string text, int userId)
    {
        LikesBy = new List<int>();
        Text = text;
        UserId = userId;
    }
}
public class Comment
{
    [Key]
    public int Id { get; init; }
    public string Text { get; set; }
    public int UserId { get; init; }
    [ForeignKey(nameof(UserId))]
    [JsonIgnore]
    public virtual User User { get; set; }
    public int PostId {  get; init; }
    [ForeignKey(nameof(PostId))]
    [JsonIgnore]
    public virtual Post Post { get; init; }
    public Comment() { }
    public Comment(int userId,int postId, string comment)
    {
        PostId= postId;
        Text = comment;
        UserId = userId;
    }
}
public class User
{
    [Key]
    public int Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
    public ICollection<Post> Posts;
    public ICollection<Comment> Comments;
    public User(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }
}
