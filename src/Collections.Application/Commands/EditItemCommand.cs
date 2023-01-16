using Collections.Application.Interfaces;
using Collections.Domain.Entities;
using Collections.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Commands;

public class EditItemCommand : IRequest<Guid>
{
    public Guid ItemId { get; set; }
    public EditedItem EditedItem { get; set; }

    public class EditItemCommandHandler : IRequestHandler<EditItemCommand, Guid>
    {
        private readonly ICollectionsDbContext _dbContext;
        private readonly IImageStorageService _imageStorageService;

        public EditItemCommandHandler(ICollectionsDbContext dbContext, IImageStorageService imageStorageService)
        {
            _dbContext = dbContext;
            _imageStorageService = imageStorageService;
        }

        public async Task<Guid> Handle(EditItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _dbContext.Items
                .Include(i => i.Category)
                    .ThenInclude(c => c.Tags)
                .SingleOrDefaultAsync(item => item.Id == request.ItemId);

            if (item == null)
            {
                throw new NotFoundException<Item>(request.ItemId);
            }

            var tagsValues = await _dbContext.TagsValues
                .Include(tv => tv.Item)
                .Include(tv => tv.Tag)
                .Where(tv => tv.Item.Id == item.Id)
                .ToListAsync();

            var createdTagValues = new List<TagValue>();

            foreach (var newTagValue in request.EditedItem.Tags)
            {
                var editedTagValue = tagsValues.SingleOrDefault(tv => tv.Tag.Name == newTagValue.Name);

                if(editedTagValue != null)
                {
                    editedTagValue.EditValue(newTagValue.Value);
                }
                else
                {
                    var tag = item.Category.Tags.Single(t => t.Name == newTagValue.Name);
                    createdTagValues.Add(TagValue.Create(tag, item, newTagValue.Value));
                }
            }

            item.AddTagValues(createdTagValues);

            item.Edit(request.EditedItem.Name, request.EditedItem.Description, request.EditedItem.AcquiredDate);

            await _dbContext.SaveChangesAsync(cancellationToken);

            if(request.EditedItem.ImageBase64 != null)
            {
                await _imageStorageService.DeleteImageAsync(item.Id);
                await _imageStorageService.UploadImageAsync(item.Id, request.EditedItem.ImageBase64);
            }

            return item.Id;
        }
    }
}

public class EditedItem
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? AcquiredDate { get; set; }
    public string? ImageBase64 { get; set; }
    public List<NewTagValue> Tags { get; set; }
}
