using Collections.Application.Interfaces;
using Collections.Domain.Entities;
using Collections.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Commands;

public class CreateItemCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public NewItemDto NewItemData { get; set; }

    internal class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, Guid>
    {
        private readonly ICollectionsDbContext _dbContext;

        public CreateItemCommandHandler(ICollectionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(user => user.Id == request.UserId);

            if (user == null)
            {
                throw new NotFoundException<User>(request.UserId);
            }

            //var item = Item.Create(request.NewItemData.Name, request.NewItemData.Description, DateTime.Now, request.NewItemData.AcquiredDate, "imgUrl", request.NewItemData.IsFavourite, user);

            //_dbContext.Items.Add(item);
            //await _dbContext.SaveChangesAsync(cancellationToken);

            //return item.Id;

            return Guid.NewGuid();
        }
    }
}

public class NewItemDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime AcquiredDate { get; set; }
    public bool IsFavourite { get; set; }
    public List<AttachedTagDto> Tags { get; set; }

}

public class AttachedTagDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public string Color { get; set; }
}
