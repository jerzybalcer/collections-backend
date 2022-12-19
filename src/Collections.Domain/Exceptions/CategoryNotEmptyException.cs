namespace Collections.Domain.Exceptions;

public class CategoryNotEmptyException : Exception
{
    public CategoryNotEmptyException(Guid id) : base($"Category with id: {id} has items assigned to itself. You can only delete empty category.")
    {
    }
}
