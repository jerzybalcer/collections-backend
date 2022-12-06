namespace Collections.Domain.Entities;

public class Item : Entity
{
    private List<TagValue> _tagsValues = new List<TagValue>();

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTime AddedDate { get; private set; }
    public DateTime AcquiredDate { get; private set; }
    public bool IsFavourite { get; private set; }
    public User User { get; private set; }
    public Category Category { get; private set; }
    public IReadOnlyCollection<TagValue> TagsValues => _tagsValues.AsReadOnly();

    private Item(string name, string description, DateTime addedDate, DateTime acquiredDate, bool isFavourite)
    {
        Name = name;
        Description = description;
        AddedDate = addedDate;
        AcquiredDate = acquiredDate;
        IsFavourite = isFavourite;
    }

    public static Item Create(string name, string description, DateTime addedDate, DateTime acquiredDate, bool isFavourite, Category category, User user)
    {
        return new Item(name, description, addedDate, acquiredDate, isFavourite) { Category = category, User = user };
    }

    public void ToggleIsFavourite()
    {
        IsFavourite = IsFavourite ? false : true;
    }

    public void AddTagValues(List<TagValue> tagValues)
    {
        _tagsValues.AddRange(tagValues);
    }
}
