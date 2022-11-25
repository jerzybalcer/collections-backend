using Collections.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Queries;

public class GetAllCategoriesQuery : IRequest<List<CategoryDto>>
{
    internal class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
    {
        private readonly ICollectionsDbContext _dbContext;

        public GetAllCategoriesQueryHandler(ICollectionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Categories
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDto 
                    { 
                        Id = c.Id,
                        Name = c.Name,
                        Color = c.Color,
                    }
                )
                .ToListAsync(cancellationToken);
        }
    }
}

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
}