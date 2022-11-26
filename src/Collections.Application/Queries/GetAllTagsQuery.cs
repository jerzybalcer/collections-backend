using Collections.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Queries;

public class GetAllTagsQuery : IRequest<List<string>>
{
    internal class GetAllTagsQueryHandler : IRequestHandler<GetAllTagsQuery, List<string>>
    {
        private readonly ICollectionsDbContext _dbContext;

        public GetAllTagsQueryHandler(ICollectionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<string>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Tags
                .OrderBy(t => t.Name)
                .Select(t => t.Name)
                .ToListAsync(cancellationToken);
        }
    }
}