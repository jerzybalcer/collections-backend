using Collections.Application.Commands;
using Collections.Application.Interfaces;
using Collections.Domain.Entities;
using Collections.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Collections.Tests;

public class EditCategoryTests
{
    private readonly CollectionsDbContext _dbContext;

    public EditCategoryTests()
    {
        var options = new DbContextOptionsBuilder<CollectionsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new CollectionsDbContext(options);
    }

    [Fact]
    public async void ShouldEditCategory()
    {
        var category = Category.Create("category", "color");

        var tag1 = Tag.Create("tag1", category);
        var tag2 = Tag.Create("tag2", category);

        category.AddTags(new List<Tag> { tag1, tag2 });

        _dbContext.Categories.Add(category);

        await _dbContext.SaveChangesAsync();

        var command = new EditCategoryCommand 
        { 
            CategoryId = category.Id, 
            EditedCategory = new EditedCategory 
            { 
                Name = "newName", Color = "newColor", Tags = new List<string> { "tag1", "tag2" }
            } 
        };

        var handler = new EditCategoryCommand.EditCategoryCommandHandler(_dbContext);

        await handler.Handle(command, CancellationToken.None);

        var editedCategory = await _dbContext.Categories
            .Include(i => i.Tags)
            .SingleOrDefaultAsync(c => c.Id == category.Id);

        Assert.NotNull(editedCategory);
        Assert.Equal("newName", editedCategory.Name);
        Assert.Equal(2, editedCategory.Tags.Count);
        Assert.True(editedCategory.Tags.Any(t => t.Name == "tag1"));
        Assert.True(editedCategory.Tags.Any(t => t.Name == "tag2"));
    }

    [Fact]
    public async void ShouldAddNewTagToCategory()
    {
        var category = Category.Create("category", "color");

        var tag1 = Tag.Create("tag1", category);
        var tag2 = Tag.Create("tag2", category);

        category.AddTags(new List<Tag> { tag1, tag2 });

        _dbContext.Categories.Add(category);

        await _dbContext.SaveChangesAsync();

        var command = new EditCategoryCommand
        {
            CategoryId = category.Id,
            EditedCategory = new EditedCategory
            {
                Name = "newName",
                Color = "newColor",
                Tags = new List<string> { "tag1", "tag2", "tag3" }
            }
        };

        var handler = new EditCategoryCommand.EditCategoryCommandHandler(_dbContext);

        await handler.Handle(command, CancellationToken.None);

        var editedCategory = await _dbContext.Categories
            .Include(i => i.Tags)
            .SingleOrDefaultAsync(c => c.Id == category.Id);

        Assert.NotNull(editedCategory);
        Assert.Equal("newName", editedCategory.Name);
        Assert.Equal(3, editedCategory.Tags.Count);
        Assert.True(editedCategory.Tags.Any(t => t.Name == "tag1"));
        Assert.True(editedCategory.Tags.Any(t => t.Name == "tag2"));
        Assert.True(editedCategory.Tags.Any(t => t.Name == "tag3"));

        var newTag = await _dbContext.Tags.SingleOrDefaultAsync(t => t.Name == "tag3");

        Assert.NotNull(newTag);
    }

    [Fact]
    public async void ShouldAddExistingTagToCategory()
    {
        var category = Category.Create("category", "color");

        var tag1 = Tag.Create("tag1", category);
        var tag2 = Tag.Create("tag2", category);
        var tag3 = Tag.Create("tag3", category);

        category.AddTags(new List<Tag> { tag1, tag2 });

        _dbContext.Categories.Add(category);
        _dbContext.Tags.Add(tag3);

        await _dbContext.SaveChangesAsync();

        var command = new EditCategoryCommand
        {
            CategoryId = category.Id,
            EditedCategory = new EditedCategory
            {
                Name = "newName",
                Color = "newColor",
                Tags = new List<string> { "tag1", "tag2", "tag3" }
            }
        };

        var handler = new EditCategoryCommand.EditCategoryCommandHandler(_dbContext);

        await handler.Handle(command, CancellationToken.None);

        var editedCategory = await _dbContext.Categories
            .Include(i => i.Tags)
            .SingleOrDefaultAsync(c => c.Id == category.Id);

        Assert.NotNull(editedCategory);
        Assert.Equal("newName", editedCategory.Name);
        Assert.Equal(3, editedCategory.Tags.Count);
        Assert.True(editedCategory.Tags.Any(t => t.Name == "tag1"));
        Assert.True(editedCategory.Tags.Any(t => t.Name == "tag2"));
        Assert.True(editedCategory.Tags.Any(t => t.Name == "tag3" && t.Id == tag3.Id));
    }

    [Fact]
    public async void ShouldRemoveTagFromCategory()
    {
        var category = Category.Create("category", "color");

        var tag1 = Tag.Create("tag1", category);
        var tag2 = Tag.Create("tag2", category);

        category.AddTags(new List<Tag> { tag1, tag2 });

        var category2 = Category.Create("category2", "color");

        category2.AddTags(new List<Tag> { tag2 });

        _dbContext.Categories.Add(category);
        _dbContext.Categories.Add(category2);

        await _dbContext.SaveChangesAsync();

        var command = new EditCategoryCommand
        {
            CategoryId = category.Id,
            EditedCategory = new EditedCategory
            {
                Name = "newName",
                Color = "newColor",
                Tags = new List<string> { "tag1" }
            }
        };

        var handler = new EditCategoryCommand.EditCategoryCommandHandler(_dbContext);

        await handler.Handle(command, CancellationToken.None);

        var editedCategory = await _dbContext.Categories
            .Include(i => i.Tags)
            .SingleOrDefaultAsync(c => c.Id == category.Id);

        Assert.NotNull(editedCategory);
        Assert.Equal("newName", editedCategory.Name);
        Assert.Equal(1, editedCategory.Tags.Count);
        Assert.True(editedCategory.Tags.Any(t => t.Name == "tag1"));
        Assert.False(editedCategory.Tags.Any(t => t.Name == "tag2"));

        var notCompletelyDeletedTag = await _dbContext.Tags.SingleOrDefaultAsync(t => t.Name == "tag2");

        Assert.NotNull(notCompletelyDeletedTag);
    }

    [Fact]
    public async void ShouldRemoveTagCompletely()
    {
        var category = Category.Create("category", "color");

        var tag1 = Tag.Create("tag1", category);
        var tag2 = Tag.Create("tag2", category);

        category.AddTags(new List<Tag> { tag1, tag2 });

        _dbContext.Categories.Add(category);

        await _dbContext.SaveChangesAsync();

        var command = new EditCategoryCommand
        {
            CategoryId = category.Id,
            EditedCategory = new EditedCategory
            {
                Name = "newName",
                Color = "newColor",
                Tags = new List<string> { "tag1" }
            }
        };

        var handler = new EditCategoryCommand.EditCategoryCommandHandler(_dbContext);

        await handler.Handle(command, CancellationToken.None);

        var editedCategory = await _dbContext.Categories
            .Include(i => i.Tags)
            .SingleOrDefaultAsync(c => c.Id == category.Id);

        Assert.NotNull(editedCategory);
        Assert.Equal("newName", editedCategory.Name);
        Assert.Equal(1, editedCategory.Tags.Count);
        Assert.True(editedCategory.Tags.Any(t => t.Name == "tag1"));
        Assert.False(editedCategory.Tags.Any(t => t.Name == "tag2"));

        var completelyDeletedTag = await _dbContext.Tags.SingleOrDefaultAsync(t => t.Name == "tag2");

        Assert.Null(completelyDeletedTag);
    }
}
