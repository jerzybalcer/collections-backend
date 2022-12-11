using Collections.Application.Interfaces;
using Collections.Domain.Entities;
using Collections.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Queries;

public class GetUserFavouriteItemsQuery : IRequest<List<UserItemDto>>
{
    public Guid UserId { get; set; }

    internal class GetUserFavouriteItemsQueryHandler : IRequestHandler<GetUserFavouriteItemsQuery, List<UserItemDto>>
    {
        private readonly ICollectionsDbContext _dbContext;

        public GetUserFavouriteItemsQueryHandler(ICollectionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UserItemDto>> Handle(GetUserFavouriteItemsQuery request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(user => user.Id == request.UserId);

            if (user == null)
            {
                throw new NotFoundException<User>(request.UserId);
            }

            var items = await _dbContext.Items
                .Include(i => i.Category)
                .Where(item => item.User.Id == user.Id && item.IsFavourite)
                .Select(item => new UserItemDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    AddedDate = item.AddedDate,
                    AcquiredDate = item.AcquiredDate,
                    IsFavourite = item.IsFavourite,
                    Category = new UserItemCategory { Name = item.Category.Name, Color = item.Category.Color },
                }
                )
                .OrderByDescending(item => item.AddedDate)
                .ToListAsync();

            return items;
        }
    }
}
