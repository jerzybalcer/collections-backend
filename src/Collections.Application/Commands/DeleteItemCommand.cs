using Collections.Application.Interfaces;
using Collections.Domain.Entities;
using Collections.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Commands;

public class DeleteItemCommand : IRequest<Unit>
{
    public Guid ItemId { get; set; }

    internal class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, Unit>
    {
        private readonly ICollectionsDbContext _dbContext;

        public DeleteItemCommandHandler(ICollectionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _dbContext.Items.SingleOrDefaultAsync(item => item.Id == request.ItemId);

            if (item == null)
            {
                throw new NotFoundException<Item>(request.ItemId);
            }

            _dbContext.Items.Remove(item);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}