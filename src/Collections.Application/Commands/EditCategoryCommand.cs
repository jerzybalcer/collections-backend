using Collections.Application.Interfaces;
using Collections.Domain.Entities;
using Collections.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Commands;

public class EditCategoryCommand : IRequest<Guid>
{
    public Guid CategoryId { get; set; }
    public EditedCategory EditedCategory { get; set; }

    internal class EditCategoryCommandHandler : IRequestHandler<EditCategoryCommand, Guid>
    {
        private readonly ICollectionsDbContext _dbContext;

        public EditCategoryCommandHandler(ICollectionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> Handle(EditCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _dbContext.Categories
                .Include(i => i.Tags)
                .SingleOrDefaultAsync(category => category.Id == request.CategoryId, cancellationToken);

            if (category == null)
            {
                throw new NotFoundException<Category>(request.CategoryId);
            }

            category.Edit(request.EditedCategory.Name, request.EditedCategory.Color);

            DeleteTags(request.EditedCategory.Tags, category);
            await AddTags(request.EditedCategory.Tags, category);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return category.Id;
        }

        private async Task AddTags(List<string> addedTags, Category category)
        {
            foreach (var addedTag in addedTags)
            {
                var existingTag = await _dbContext.Tags.SingleOrDefaultAsync(t => t.Name == addedTag);

                if (existingTag != null)
                {
                    if(!existingTag.Categories.Contains(category))
                        category.AddTags(new List<Tag> { existingTag });
                }
                else
                {
                    var newTag = Tag.Create(addedTag, category);
                    category.AddTags(new List<Tag> { newTag });
                }
            }
        }

        private async void DeleteTags(List<string> editedTags, Category category)
        {
            var tagsToRemove = new List<Tag>();
            var tagsWihoutCategory = new List<Tag>();

            foreach (var existingTag in category.Tags)
            {
                if(!editedTags.Any(t => t == existingTag.Name))
                {
                    tagsToRemove.Add(existingTag);

                    var categoriesCount = await _dbContext.Categories
                        .Include(c => c.Tags)
                        .Where(c => c.Tags.Any(t => t.Name == existingTag.Name))
                        .CountAsync();

                    if (categoriesCount == 1)
                    {
                        tagsWihoutCategory.Add(existingTag);
                    }
                }
            }

            category.RemoveTags(tagsToRemove);
            _dbContext.Tags.RemoveRange(tagsWihoutCategory);
        }
    }
}

public class EditedCategory
{
    public string? Name { get; set; }
    public string? Color { get; set; }
    public List<string> Tags { get; set; }
}
