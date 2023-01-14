using Collections.Application.Interfaces;
using Collections.Domain.Entities;
using Collections.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Queries;

public class GetFavouriteItemsQuery : IRequest<List<UserItemDto>>
{
    internal class GetUserFavouriteItemsQueryHandler : IRequestHandler<GetFavouriteItemsQuery, List<UserItemDto>>
    {
        private readonly ICollectionsDbContext _dbContext;

        public GetUserFavouriteItemsQueryHandler(ICollectionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UserItemDto>> Handle(GetFavouriteItemsQuery request, CancellationToken cancellationToken)
        {
            var items = await _dbContext.Items
                .Include(i => i.Category)
                .Include(i => i.TagsValues)
                    .ThenInclude(tv => tv.Tag)
                .Where(item => item.IsFavourite)
                .Select(item => new UserItemDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    AddedDate = item.AddedDate,
                    AcquiredDate = item.AcquiredDate,
                    IsFavourite = item.IsFavourite,
                    Category = new UserItemCategory { Name = item.Category.Name, Color = item.Category.Color },
                    Tags = item.TagsValues.Select(tv => tv.Tag.Name).ToList(),
                }
                )
                .OrderByDescending(item => item.AddedDate)
                .ToListAsync();

            return items;
        }
    }
}
