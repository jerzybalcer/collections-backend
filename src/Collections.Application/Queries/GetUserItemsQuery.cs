using Collections.Application.Interfaces;
using Collections.Domain.Entities;
using Collections.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Queries;

public class GetUserItemsQuery : IRequest<List<UserItemDto>>
{
    public Guid UserId { get; set; }

    internal class GetUserItemsQueryHandler : IRequestHandler<GetUserItemsQuery, List<UserItemDto>>
    {
        private readonly ICollectionsDbContext _dbContext;

        public GetUserItemsQueryHandler(ICollectionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UserItemDto>> Handle(GetUserItemsQuery request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(user => user.Id == request.UserId);

            if(user == null)
            {
                throw new NotFoundException<User>(request.UserId);
            }

            var items = await _dbContext.Items
                .Where(item => item.User.Id == user.Id)
                .Select(item => new UserItemDto 
                    { 
                        Id = item.Id,
                        Name = item.Name,
                        AddedDate = item.AddedDate,
                        AcquiredDate = item.AcquiredDate,
                    }
                )
                .OrderByDescending(item => item.AddedDate)
                .ToListAsync();

            return items;
        }
    }
}

public class UserItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime AddedDate { get; set; }
    public DateTime AcquiredDate { get; set; }
}
