using Collections.Domain.Entities;

namespace Collections.Domain.Exceptions;

public class NoPropertyValueException<T>: Exception where T : Entity
{
    public NoPropertyValueException(string propertyName): base($"Property '{propertyName}' of {nameof(T)} is required.")
    {
    }
}
