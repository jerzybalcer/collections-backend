using Collections.Application.Interfaces;
using Collections.Domain.Entities;
using Collections.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Collections.Application.Commands;

public class DeleteCategoryCommand : IRequest<Unit>
{
    public Guid CategoryId { get; set; }

    internal class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
    {
        private readonly ICollectionsDbContext _dbContext;

        public DeleteCategoryCommandHandler(ICollectionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _dbContext.Categories
                .Include(c => c.Tags)
                .SingleOrDefaultAsync(category => category.Id == request.CategoryId);

            if (category == null)
            {
                throw new NotFoundException<Category>(request.CategoryId);
            }

            if (category.Items.Any())
            {
                throw new CategoryNotEmptyException(request.CategoryId);
            }

            foreach (var tag in category.Tags)
            {
                var categoriesCount = await _dbContext.Categories
                    .Include(c => c.Tags)
                    .Where(c => c.Tags.Any(t => t.Id == tag.Id))
                    .CountAsync();

                if(categoriesCount == 1)
                {
                    _dbContext.Tags.Remove(tag);
                }
            }

            _dbContext.Categories.Remove(category);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}