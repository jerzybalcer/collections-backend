using Collections.Application.Commands;
using Collections.Application.Interfaces;
using Collections.Domain.Entities;
using Collections.Domain.Exceptions;
using Collections.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Collections.Tests;

public class EditItemTests
{
    private readonly CollectionsDbContext _dbContext;

    public EditItemTests()
    {
        var options = new DbContextOptionsBuilder<CollectionsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new CollectionsDbContext(options);
    }

    [Fact]
    public async void ShouldEditItem()
    {
        var category = Category.Create("category", "color");

        var tag1 = Tag.Create("tag1", category);
        var tag2 = Tag.Create("tag2", category);

        category.AddTags(new List<Tag> { tag1, tag2 });

        var item = Item.Create("item", "description", DateTime.Now, DateTime.Parse("01-01-2023"), true, category);

        var tagValue1 = TagValue.Create(tag1, item, "value1");
        var tagValue2 = TagValue.Create(tag2, item, "value2");

        item.AddTagValues(new List<TagValue> { tagValue1, tagValue2 } );

        _dbContext.Items.Add(item);
        _dbContext.Categories.Add(category);

        await _dbContext.SaveChangesAsync();

        var command = new EditItemCommand
        {
            ItemId = item.Id,
            EditedItem = new EditedItem
            {
                Name = "newName",
                Description = "newDescription",
                AcquiredDate = DateTime.Parse("02-02-2023"),
                ImageBase64 = "newImage",
                Tags = new List<NewTagValue> { new NewTagValue { Name = "tag1", Value = "value1" }, new NewTagValue { Name = "tag2", Value = "newValue2" } }
            }
        };

        var imageStorageSubstitute = Substitute.For<IImageStorageService>();

        var handler = new EditItemCommand.EditItemCommandHandler(_dbContext, imageStorageSubstitute);

        await handler.Handle(command, CancellationToken.None);

        var editedItem = await _dbContext.Items
            .Include(i => i.TagsValues)
            .ThenInclude(tv => tv.Tag)
            .SingleOrDefaultAsync(i => i.Id == item.Id);

        Assert.NotNull(editedItem);
        Assert.Equal("newName", editedItem.Name);
        Assert.Equal("newDescription", editedItem.Description);
        Assert.Equal(DateTime.Parse("02-02-2023"), editedItem.AcquiredDate);
        Assert.Equal(DateTime.Parse("02-02-2023"), editedItem.AcquiredDate);
        Assert.Equal(2, editedItem.TagsValues.Count);
        Assert.True(editedItem.TagsValues.Any(tv => tv.Tag.Name == "tag1" && tv.Value == "value1"));
        Assert.True(editedItem.TagsValues.Any(tv => tv.Tag.Name == "tag2" && tv.Value == "newValue2"));
    }
}
