using Collections.Domain.Exceptions;

namespace Collections.Domain.Entities;

public class Tag : Entity
{
    private List<TagValue> _values = new List<TagValue>();
    private List<Category> _categories = new List<Category>();

    public string Name { get; private set; }
    public IReadOnlyCollection<TagValue> Values => _values.AsReadOnly();
    public IReadOnlyCollection<Category> Categories => _categories.AsReadOnly();

    private Tag(string name)
    {
        Name = name;
    }

    public static Tag Create(string name, Category category)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new NoPropertyValueException<Tag>(nameof(name));
        }

        if (category == null)
        {
            throw new NoPropertyValueException<Tag>(nameof(category));
        }

        var tag = new Tag(name);
        
        tag._categories.Add(category);

        return tag;
    }

    public void SetName(string name)
    {
        Name = name;
    }
}
