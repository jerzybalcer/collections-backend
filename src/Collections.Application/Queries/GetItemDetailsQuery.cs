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
        private readonly IImageStorageService _imageStorageService;

        public GetItemDetailsQueryHandler(ICollectionsDbContext dbContext, IImageStorageService imageStorageService)
        {
            _dbContext = dbContext;
            _imageStorageService = imageStorageService;
        }

        public async Task<ItemDetailsDto> Handle(GetItemDetailsQuery request, CancellationToken cancellationToken)
        {
            var item = await _dbContext.Items
                .Include(t => t.TagsValues)
                    .ThenInclude(tv => tv.Tag)
                .Include(i => i.Category)
                    .ThenInclude(c => c.Tags)
                .SingleOrDefaultAsync(item => item.Id == request.ItemId);

            if(item == null)
            {
                throw new NotFoundException<Item>(request.ItemId);
            }

            var imageUrl = await _imageStorageService.GetImageUrlAsync(request.ItemId);

            var itemDetails = new ItemDetailsDto
            {
                Name = item.Name,
                Description = item.Description ?? string.Empty,
                AddedDate = item.AddedDate,
                AcquiredDate = item.AcquiredDate,
                IsFavourite = item.IsFavourite,
                ImageUrl = imageUrl,
                TagValues = item.TagsValues.Select(tv => new TagValueDto { Name = tv.Tag.Name, Value = tv.Value }).ToList(),
                Category = new ItemDetailsCategoryDto { Name = item.Category.Name, Color = item.Category.Color, Tags = item.Category.Tags.Select(t => t.Name).ToList() },
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
    public List<TagValueDto> TagValues { get; set; }
    public ItemDetailsCategoryDto Category { get; set; }
}

public class ItemDetailsCategoryDto
{
    public string Name { get; set; }
    public string Color { get; set; }
    public List<string> Tags { get; set; }
}

public class TagValueDto
{
    public string Name { get; set; }
    public string Value { get; set; }
}