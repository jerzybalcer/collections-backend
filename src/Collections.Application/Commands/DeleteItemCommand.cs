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
        private readonly IImageStorageService _imageStorageService;

        public DeleteItemCommandHandler(ICollectionsDbContext dbContext, IImageStorageService imageStorageService)
        {
            _dbContext = dbContext;
            _imageStorageService = imageStorageService;
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

            await _imageStorageService.DeleteImageAsync(request.ItemId);

            return Unit.Value;
        }
    }
}