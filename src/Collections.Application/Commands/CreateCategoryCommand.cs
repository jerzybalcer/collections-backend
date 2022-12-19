using Collections.Application.Interfaces;
using Collections.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Commands;

public class CreateCategoryCommand : IRequest<Guid>
{
    public NewCategory NewCategory { get; set; }

    internal class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
    {
        private readonly ICollectionsDbContext _dbContext;

        public CreateCategoryCommandHandler(ICollectionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = Category.Create(request.NewCategory.Name, request.NewCategory.Color);

            var tags = await GetTags(request.NewCategory.Tags, category, cancellationToken);

            category.AddTags(tags);

            _dbContext.Categories.Add(category);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return category.Id;
        }

        private async Task<List<Tag>> GetTags(List<string> names, Category category, CancellationToken cancellationToken)
        {
            var tags = new List<Tag>();

            foreach (var name in names)
            {
                var tag = await _dbContext.Tags.SingleOrDefaultAsync(tag => tag.Name == name, cancellationToken);

                if (tag == null)
                {
                    tag = Tag.Create(name, category);
                }

                tags.Add(tag);
            }

            return tags;
        }
    }
}

public class NewCategory
{
    public string Name { get; set; }
    public string Color { get; set; }
    public List<string> Tags { get; set; }
}
