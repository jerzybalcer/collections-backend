using Collections.Domain.Exceptions;

namespace Collections.Domain.Entities;

public class Category : Entity
{
    private List<Item> _items = new List<Item>();
    private List<Tag> _tags = new List<Tag>();

    public string Name { get; private set; }
    public string Color { get; private set; }

    public IReadOnlyCollection<Item> Items => _items.AsReadOnly();
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

    private Category(string name, string color)
    {
        Name = name;
        Color = color;
    }

    public static Category Create(string name, string color)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new NoPropertyValueException<Tag>(nameof(name));
        }

        if (string.IsNullOrWhiteSpace(color))
        {
            throw new NoPropertyValueException<Tag>(nameof(color));
        }

        var category = new Category(name, color);

        return category;
    }

    public void AddTags(List<Tag> tags)
    {
        foreach(var tag in tags)
        {
            if(!_tags.Any(t => t.Name == tag.Name))
            {
                _tags.Add(tag);
            }
        }
    }

    public void RemoveTags(List<Tag> tags)
    {
        foreach (var tag in tags)
        {
            _tags.Remove(tag);
        }
    }

    public void Edit(string? name, string? color)
    {
        if(name != null)
        {
            Name = name;
        }

        if (color != null)
        {
            Color = color;
        }
    }
}