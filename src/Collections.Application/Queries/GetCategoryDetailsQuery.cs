using Collections.Application.Interfaces;
using Collections.Domain.Entities;
using Collections.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Queries;

public class GetCategoryDetailsQuery : IRequest<CategoryDetailsDto>
{
    public Guid CategoryId { get; set; }

    internal class GetCategoryDetailsQueryHandler : IRequestHandler<GetCategoryDetailsQuery, CategoryDetailsDto>
    {
        private readonly ICollectionsDbContext _dbContext;

        public GetCategoryDetailsQueryHandler(ICollectionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CategoryDetailsDto> Handle(GetCategoryDetailsQuery request, CancellationToken cancellationToken)
        {
            var category = await _dbContext.Categories
                .Include(c => c.Tags)
                .Include(c => c.Items)
                .SingleOrDefaultAsync(c => c.Id == request.CategoryId);

            if (category == null)
            {
                throw new NotFoundException<Category>(request.CategoryId);
            }

            var categoryDetails = new CategoryDetailsDto
            {
                Name = category.Name,
                Color = category.Color,
                Tags = category.Tags.Select(t => t.Name).ToList(),
                CanBeDeleted = !category.Items.Any(),
            };

            return categoryDetails;
        }
    }
}

public class CategoryDetailsDto
{
    public string Name { get; set; }
    public string Color { get; set; }
    public List<string> Tags { get; set; }
    public bool CanBeDeleted { get; set; }
}