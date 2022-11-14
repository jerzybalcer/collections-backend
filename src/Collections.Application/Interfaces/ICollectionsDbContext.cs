using Collections.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Interfaces;

public interface ICollectionsDbContext
{
    DbSet<Item> Items { get; }
    DbSet<Tag> Tags { get; }
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
