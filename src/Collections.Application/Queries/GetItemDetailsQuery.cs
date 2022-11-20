using Collections.Application.Interfaces;
using Collections.Domain.Entities;
using Collections.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Queries;

public class GetItemDetailsQuery : IRequest<ItemDetailsDto>
{
    public Guid ItemId { get; set; }

    internal class GetItemDetailsQueryHandler : IRequestHandler<GetItemDetailsQuery, ItemDetailsDto>
    {
        private readonly ICollectionsDbContext _dbContext;

        public GetItemDetailsQueryHandler(ICollectionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ItemDetailsDto> Handle(GetItemDetailsQuery request, CancellationToken cancellationToken)
        {
            var item = await _dbContext.Items.SingleOrDefaultAsync(item => item.Id == request.ItemId);

            if(item == null)
            {
                throw new NotFoundException<Item>(request.ItemId);
            }

            var itemDetails = new ItemDetailsDto
            {
                Name = item.Name,
                Description = item.Description ?? string.Empty,
                AddedDate = item.AddedDate,
                AcquiredDate = item.AcquiredDate,
                IsFavourite = item.IsFavourite,
                ImageUrl = item.ImageUrl,
            };

            return itemDetails;
        }
    }
}

public class ItemDetailsDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime AddedDate { get; set; }
    public DateTime AcquiredDate { get; set; }
    public bool IsFavourite { get; set; }
    public string ImageUrl { get; set; }
}