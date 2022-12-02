using Collections.Application.Interfaces;
using Collections.Domain.Entities;
using Collections.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Commands;

public class ToggleIsFavouriteCommand : IRequest<Unit>
{
    public Guid ItemId { get; set; }

    internal class ToggleIsFavouriteCommandHandler : IRequestHandler<ToggleIsFavouriteCommand, Unit>
    {
        private readonly ICollectionsDbContext _dbContext;

        public ToggleIsFavouriteCommandHandler(ICollectionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(ToggleIsFavouriteCommand request, CancellationToken cancellationToken)
        {
            var item = await _dbContext.Items.SingleOrDefaultAsync(item => item.Id == request.ItemId);

            if (item == null)
            {
                throw new NotFoundException<Item>(request.ItemId);
            }

            item.ToggleIsFavourite();
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
