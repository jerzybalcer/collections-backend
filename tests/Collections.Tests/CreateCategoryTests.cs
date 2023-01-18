using Collections.Application.Commands;
using Collections.Domain.Entities;
using Collections.Domain.Exceptions;
using Collections.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Collections.Tests;

public class CreateCategoryTests
{
    private readonly CollectionsDbContext _dbContext;

    public CreateCategoryTests()
    {
        var options = new DbContextOptionsBuilder<CollectionsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new CollectionsDbContext(options);
    }

    [Fact]
    public async void ShouldNotCreateCategoryWithDuplicateName()
    {
        var category1 = Category.Create("category1", "color");

        _dbContext.Categories.Add(category1);

        await _dbContext.SaveChangesAsync();

        var command = new CreateCategoryCommand
        {
            NewCategory = new NewCategory
            {
                Name = "category1",
                Color = "color",
                Tags = new List<string>()
            }
        };

        var handler = new CreateCategoryCommand.CreateCategoryCommandHandler(_dbContext);

        var exception = await Assert.ThrowsAsync<AlreadyExistsException<Category>>(() => handler.Handle(command, CancellationToken.None));
    }
}
