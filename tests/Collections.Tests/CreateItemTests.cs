using Collections.Application.Commands;
using Collections.Application.Interfaces;
using Collections.Domain.Entities;
using Collections.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Collections.Tests
{
    public class CreateItemTests
    {
        private readonly CollectionsDbContext _dbContext;

        public CreateItemTests()
        {
            var options = new DbContextOptionsBuilder<CollectionsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext= new CollectionsDbContext(options);
        }

        [Fact]
        public async void CreateItemUsingExistingCategory()
        {
            var category = Category.Create("category", "color");

            var tag = Tag.Create("tag", category);

            category.AddTags(new List<Tag> { tag });

            _dbContext.Categories.Add(category);

            await _dbContext.SaveChangesAsync();

            var command = new CreateItemCommand
            {
                NewItemData = new NewItem
                {
                    Name = "item",
                    Description = "description",
                    AcquiredDate = DateTime.Parse("01-01-2023"),
                    IsFavourite = false,
                    Category = new NewItemCategory { Id = category.Id },
                    Tags = new List<NewTagValue>
                    {
                        new NewTagValue { Name = tag.Name, Value = "tagValue"}
                    }
                } 
            };

            var imageStorageSubstitute = Substitute.For<IImageStorageService>();

            var handler = new CreateItemCommand.CreateItemCommandHandler(_dbContext, imageStorageSubstitute);

            var newItemId = await handler.Handle(command, CancellationToken.None);

            var newItem = await _dbContext.Items
                .Include(i => i.Category)
                .Include(i => i.TagsValues)
                .ThenInclude(tv => tv.Tag)
                .SingleOrDefaultAsync(i => i.Id == newItemId);

            Assert.NotNull(newItem);
            Assert.Equal("item", newItem.Name);
            Assert.Equal("description", newItem.Description);
            Assert.Equal(DateTime.Parse("01-01-2023"), newItem.AcquiredDate);
            Assert.False(newItem.IsFavourite);
            Assert.Equal("category", newItem.Category.Name);
            Assert.Equal("color", newItem.Category.Color);
            Assert.Equal(category.Id, newItem.Category.Id);
            Assert.Equal(1, newItem.TagsValues.Count);
            Assert.True(newItem.TagsValues.Any(tv => tv.Tag.Name == "tag" && tv.Value == "tagValue"));
        }

        [Fact]
        public async void CreateItemUsingNewCategory()
        {
            var command = new CreateItemCommand
            {
                NewItemData = new NewItem
                {
                    Name = "item",
                    Description = "description",
                    AcquiredDate = DateTime.Parse("01-01-2023"),
                    IsFavourite = false,
                    Category = new NewItemCategory { Name = "category", Color = "color" },
                    Tags = new List<NewTagValue>
                    {
                        new NewTagValue { Name = "tag", Value = "tagValue"}
                    }
                }
            };

            var imageStorageSubstitute = Substitute.For<IImageStorageService>();

            var handler = new CreateItemCommand.CreateItemCommandHandler(_dbContext, imageStorageSubstitute);

            var newItemId = await handler.Handle(command, CancellationToken.None);

            var newItem = await _dbContext.Items
                .Include(i => i.Category)
                .Include(i => i.TagsValues)
                .ThenInclude(tv => tv.Tag)
                .SingleOrDefaultAsync(i => i.Id == newItemId);

            Assert.NotNull(newItem);
            Assert.Equal("item", newItem.Name);
            Assert.Equal("description", newItem.Description);
            Assert.Equal(DateTime.Parse("01-01-2023"), newItem.AcquiredDate);
            Assert.False(newItem.IsFavourite);
            Assert.Equal("category", newItem.Category.Name);
            Assert.Equal("color", newItem.Category.Color);
            Assert.Equal(1, newItem.TagsValues.Count);
            Assert.True(newItem.TagsValues.Any(tv => tv.Tag.Name == "tag" && tv.Value == "tagValue"));
        }
    }
}