using Collections.Application.Commands;
using Collections.Domain.Entities;
using Collections.Domain.Exceptions;
using Collections.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Collections.Tests
{
    public class DeleteCategoryTests
    {
        private readonly CollectionsDbContext _dbContext;

        public DeleteCategoryTests()
        {
            var options = new DbContextOptionsBuilder<CollectionsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new CollectionsDbContext(options);
        }

        [Fact]
        public async void ShouldNotDeleteCategoryIfHasItems()
        {
            var category = Category.Create("category", "color");

            var item = Item.Create("item", "description", DateTime.Now, DateTime.Now, true, category);

            _dbContext.Items.Add(item);
            _dbContext.Categories.Add(category);

            await _dbContext.SaveChangesAsync();

            var command = new DeleteCategoryCommand { CategoryId = category.Id };

            var handler = new DeleteCategoryCommand.DeleteCategoryCommandHandler(_dbContext);

            await Assert.ThrowsAsync<CategoryNotEmptyException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async void ShouldDeleteCategoryWithoutItems()
        {
            var category = Category.Create("category", "color");

            _dbContext.Categories.Add(category);

            await _dbContext.SaveChangesAsync();

            var command = new DeleteCategoryCommand { CategoryId = category.Id };

            var handler = new DeleteCategoryCommand.DeleteCategoryCommandHandler(_dbContext);

            await handler.Handle(command, CancellationToken.None);

            var categoriesCount = await _dbContext.Categories.CountAsync();

            Assert.Equal(0, categoriesCount);
        }

        [Fact]
        public async void ShouldDeleteTagsWithCategoryWhenItsTheirOnlyCategory()
        {
            var category = Category.Create("category", "color");

            var tag1 = Tag.Create("tag1", category);
            var tag2 = Tag.Create("tag2", category);

            category.AddTags(new List<Tag> { tag1, tag2 });

            _dbContext.Categories.Add(category);

            await _dbContext.SaveChangesAsync();

            var command = new DeleteCategoryCommand { CategoryId = category.Id };

            var handler = new DeleteCategoryCommand.DeleteCategoryCommandHandler(_dbContext);

            await handler.Handle(command, CancellationToken.None);

            var categoriesCount = await _dbContext.Categories.CountAsync();
            var tagsCount = await _dbContext.Tags.CountAsync();

            Assert.Equal(0, categoriesCount);
            Assert.Equal(0, tagsCount);
        }

        [Fact]
        public async void ShouldNotDeleteTagsWithCategoryWhenTheyAreInAnotherCategory()
        {
            var category = Category.Create("category", "color");

            var tag1 = Tag.Create("tag1", category);
            var tag2 = Tag.Create("tag2", category);

            category.AddTags(new List<Tag> { tag1, tag2 });

            var anotherCategory = Category.Create("anotherCategory", "color2");

            anotherCategory.AddTags(new List<Tag> { tag1 });

            _dbContext.Categories.Add(category);
            _dbContext.Categories.Add(anotherCategory);

            await _dbContext.SaveChangesAsync();

            var command = new DeleteCategoryCommand { CategoryId = category.Id };

            var handler = new DeleteCategoryCommand.DeleteCategoryCommandHandler(_dbContext);

            await handler.Handle(command, CancellationToken.None);

            var categoriesCount = await _dbContext.Categories.CountAsync();
            var tagsCount = await _dbContext.Tags.CountAsync();

            Assert.Equal(1, categoriesCount);
            Assert.Equal(1, tagsCount);
        }
    }
}
