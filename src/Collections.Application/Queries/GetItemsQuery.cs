using Collections.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Queries;

public class GetItemsQuery : IRequest<List<UserItemDto>>
{
    internal class GetUserItemsQueryHandler : IRequestHandler<GetItemsQuery, List<UserItemDto>>
    {
        private readonly ICollectionsDbContext _dbContext;

        public GetUserItemsQueryHandler(ICollectionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UserItemDto>> Handle(GetItemsQuery request, CancellationToken cancellationToken)
        {
            var items = await _dbContext.Items
                .Include(i => i.Category)
                .Include(i => i.TagsValues)
                    .ThenInclude(tv => tv.Tag)
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

public class UserItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime AddedDate { get; set; }
    public DateTime AcquiredDate { get; set; }
    public bool IsFavourite { get; set; }
    public UserItemCategory Category { get; set; }
    public List<string> Tags { get; set; }
}

public class UserItemCategory
{
    public string Name { get; set; }
    public string Color { get; set; }
}
