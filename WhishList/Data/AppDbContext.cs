using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WhishList.Data;

public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) {}

    public DbSet<Wish> Wishes { get; set; }
    public DbSet<Friend> Friends { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Wish-User relationship
        builder.Entity<Wish>()
            .HasOne(w => w.User)
            .WithMany(u => u.Wishes)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Friend relationships
        builder.Entity<Friend>()
            .HasOne(f => f.User)
            .WithMany(u => u.SentFriendRequests)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Friend>()
            .HasOne(f => f.FriendUser)
            .WithMany(u => u.ReceivedFriendRequests)
            .HasForeignKey(f => f.FriendUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Optional: Rename Identity tables
        builder.Entity<User>().ToTable("Users");
        builder.Entity<IdentityRole<int>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
    }
}