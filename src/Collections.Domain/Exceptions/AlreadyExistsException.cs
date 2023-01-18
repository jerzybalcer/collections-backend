using Collections.Domain.Entities;

namespace Collections.Domain.Exceptions;

public class AlreadyExistsException<T> : Exception where T : Entity
{
    public AlreadyExistsException(string name) : base($"{typeof(T).Name} with name '{name}' already exists.")
    {
    }
}
