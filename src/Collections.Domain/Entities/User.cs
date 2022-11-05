using Collections.Domain.Exceptions;

namespace Collections.Domain.Entities;

public class User: Entity
{
    private List<Item> _items = new List<Item>();

    public string Name { get; private set; }
    public string Email { get; private set; }
    public IReadOnlyCollection<Item> Items => _items.AsReadOnly();

    private User(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public static User Create(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new NoPropertyValueException<User>(nameof(name));
        }
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new NoPropertyValueException<User>(nameof(name));
        }

        return new User(name, email);
    }
}
