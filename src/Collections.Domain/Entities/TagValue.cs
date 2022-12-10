using Collections.Domain.Exceptions;

namespace Collections.Domain.Entities;

public class TagValue : Entity
{
    public Tag Tag { get; private set; }
    public string Value { get; private set; }
    public Item Item { get; private set; }

    private TagValue(string value)
    {
        Value = value;
    }

    public static TagValue Create(Tag tag, Item item, string value)
    {
        if (tag == null)
        {
            throw new NoPropertyValueException<Tag>(nameof(tag));
        }

        if (item == null)
        {
            throw new NoPropertyValueException<Tag>(nameof(item));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new NoPropertyValueException<Tag>(nameof(value));
        }

        return new TagValue(value) { Tag = tag, Item = item };
    }

    public void EditValue(string newValue)
    {
        Value = newValue;
    }
}
