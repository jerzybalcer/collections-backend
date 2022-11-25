using Collections.Application.Interfaces;
using Collections.Domain.Entities;
using Collections.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Queries;

public class GetTagsForCategoryQuery : IRequest<List<string>>
{
    public Guid CategoryId { get; set; }

    internal class GetTagsForCategoryQueryHandler : IRequestHandler<GetTagsForCategoryQuery, List<string>>
    {
        private readonly ICollectionsDbContext _dbContext;

        public GetTagsForCategoryQueryHandler(ICollectionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<string>> Handle(GetTagsForCategoryQuery request, CancellationToken cancellationToken)
        {
            var category = await _dbContext.Categories
                .Include(c => c.Tags)
                .SingleOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

            if(category == null)
            {
                throw new NotFoundException<Category>(request.CategoryId);
            }

            return category.Tags.Select(t => t.Name).ToList();
        }
    }
}