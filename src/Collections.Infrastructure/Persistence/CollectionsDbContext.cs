using Collections.Application.Interfaces;
using Collections.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Collections.Infrastructure.Persistence;

public class CollectionsDbContext : DbContext, ICollectionsDbContext
{
    public DbSet<Item> Items { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TagValue> TagsValues { get; set; }
    public DbSet<Category> Categories { get; set; }

    public CollectionsDbContext(DbContextOptions<CollectionsDbContext> options)
    : base(options)
    {
    }
}
