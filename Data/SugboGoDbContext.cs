using Microsoft.EntityFrameworkCore;
using SugboGo.Models;

namespace SugboGo.Data;

public sealed class SugboGoDbContext : DbContext
{
    public SugboGoDbContext(DbContextOptions<SugboGoDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<TravelPreferenceRecord> TravelPreferences => Set<TravelPreferenceRecord>();
    public DbSet<DestinationPost> DestinationPosts => Set<DestinationPost>();
    public DbSet<SavedGem> SavedGems => Set<SavedGem>();
    public DbSet<AdminGem> AdminGems => Set<AdminGem>();
    public DbSet<ItineraryTemplate> ItineraryTemplates => Set<ItineraryTemplate>();
    public DbSet<AdminPartner> AdminPartners => Set<AdminPartner>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure DestinationPost -> PostComment as owned types or a separate table.
        // For simplicity and since they are closely tied, I'll use a separate table with a relationship.
        modelBuilder.Entity<DestinationPost>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.OwnsMany(e => e.CommentsList, comments =>
            {
                comments.WithOwner().HasForeignKey("DestinationPostId");
                comments.HasKey("Id");
            });
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<TravelPreferenceRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
        });

        modelBuilder.Entity<SavedGem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
        });

        modelBuilder.Entity<AdminGem>().HasKey(e => e.Id);
        modelBuilder.Entity<ItineraryTemplate>().HasKey(e => e.Id);
        modelBuilder.Entity<AdminPartner>().HasKey(e => e.Id);
    }
}
