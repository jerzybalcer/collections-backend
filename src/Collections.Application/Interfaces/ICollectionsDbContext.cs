using Collections.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Interfaces;

public interface ICollectionsDbContext
{
    DbSet<Item> Items { get; }
    DbSet<Tag> Tags { get; }
    DbSet<TagValue> TagsValues { get; }
    DbSet<Category> Categories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
