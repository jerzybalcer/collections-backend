using Collections.Application.Interfaces;
using Collections.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Commands;

public class CreateItemCommand : IRequest<Guid>
{
    public NewItem NewItemData { get; set; }

    internal class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, Guid>
    {
        private readonly ICollectionsDbContext _dbContext;
        private readonly IImageStorageService _imageStorageService;

        public CreateItemCommandHandler(ICollectionsDbContext dbContext, IImageStorageService imageStorageService)
        {
            _dbContext = dbContext;
            _imageStorageService = imageStorageService;
        }

        public async Task<Guid> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            var category = await _dbContext.Categories.SingleOrDefaultAsync(category => category.Id == request.NewItemData.Category.Id);

            if (category == null)
            {
                category = Category.Create(request.NewItemData.Category.Name!, request.NewItemData.Category.Color!);
            }

            var tags = await GetTags(request.NewItemData.Tags.Select(t => t.Name).ToList(), category, cancellationToken);

            category.AddTags(tags);

            var item = Item.Create(request.NewItemData.Name, request.NewItemData.Description, DateTime.Now, request.NewItemData.AcquiredDate, (bool)request.NewItemData.IsFavourite!, category);

            var tagsValues = tags.Select(tag => 
                TagValue.Create(tag, item, request.NewItemData.Tags.Single(t => t.Name == tag.Name).Value)
            ).ToList();

            item.AddTagValues(tagsValues);

            _dbContext.Items.Add(item);

            await _dbContext.SaveChangesAsync(cancellationToken);

            if(request.NewItemData.ImageBase64 != null)
            {
                await _imageStorageService.UploadImageAsync(item.Id, request.NewItemData.ImageBase64);
            }

            return item.Id;
        }

        private async Task<List<Tag>> GetTags(List<string> names, Category category, CancellationToken cancellationToken)
        {
            var tags = new List<Tag>();

            foreach (var name in names)
            {
                var tag = await _dbContext.Tags.SingleOrDefaultAsync(tag => tag.Name == name, cancellationToken);

                if (tag == null)
                {
                    tag = Tag.Create(name, category);
                }

                tags.Add(tag);
            }

            return tags;
        }
    }
}

public class NewItem
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime AcquiredDate { get; set; }
    public bool? IsFavourite { get; set; } = false;
    public string? ImageBase64 { get; set; }
    public List<NewTagValue> Tags { get; set; }
    public NewItemCategory Category { get; set; }

}

public class NewTagValue
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public class NewItemCategory
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }
}
