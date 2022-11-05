using Collections.Domain.Exceptions;

namespace Collections.Domain.Entities;

public class Tag: Entity
{
    public string Name { get; private set; }
    public string Value { get; private set; }
    public string Color { get; private set; }

    private Tag(string name, string value, string color)
    {
        Name = name;
        Value = value;
        Color = color;
    }

    public static Tag Create(string name, string value, string color)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new NoPropertyValueException<Tag>(nameof(name));
        }
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new NoPropertyValueException<Tag>(nameof(value));
        }
        if (string.IsNullOrWhiteSpace(color))
        {
            throw new NoPropertyValueException<Tag>(nameof(color));
        }

        return new Tag(name, value, color);
    }
}
