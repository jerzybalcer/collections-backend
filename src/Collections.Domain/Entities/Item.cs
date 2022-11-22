namespace Collections.Domain.Entities;

public class Item : Entity
{
    private List<TagValue> _tagsValues = new List<TagValue>();

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTime AddedDate { get; private set; }
    public DateTime AcquiredDate { get; private set; }
    public bool IsFavourite { get; private set; }
    public string ImageUrl { get; private set; }
    public User User { get; private set; }
    public Category Category { get; private set; }
    public IReadOnlyCollection<TagValue> TagsValues => _tagsValues.AsReadOnly();

    private Item(string name, string description, DateTime addedDate, DateTime acquiredDate, string imageUrl, bool isFavourite)
    {
        Name = name;
        Description = description;
        AddedDate = addedDate;
        AcquiredDate = acquiredDate;
        IsFavourite = isFavourite;
        ImageUrl = imageUrl;
    }

    public static Item Create(string name, string description, DateTime addedDate, DateTime acquiredDate, string imageUrl, bool isFavourite, Category category, User user)
    {
        return new Item(name, description, addedDate, acquiredDate, imageUrl, isFavourite) { Category = category, User = user };
    }

    public void SetIsFavourite(bool isFavourite)
    {
        IsFavourite = isFavourite;
    }
}
