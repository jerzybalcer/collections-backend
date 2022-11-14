using Collections.Domain.Entities;

namespace Collections.Domain.Exceptions;

public class NotFoundException<T>: Exception where T: Entity
{
    public NotFoundException(Guid id) : base($"{typeof(T)} with id: {id} not found.")
    {
    }
}
