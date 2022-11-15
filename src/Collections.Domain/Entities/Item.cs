namespace Collections.Domain.Entities;

public class Item : Entity
{
    private List<Tag> _tags = new List<Tag>();

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTime AddedDate { get; private set; }
    public DateTime AcquiredDate { get; private set; }
    public bool IsFavourite { get; private set; }
    public string ImageUrl { get; private set; }
    public User User { get; private set; }
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

    private Item(string name, string description, DateTime addedDate, DateTime acquiredDate, string imageUrl)
    {
        Name = name;
        Description = description;
        AddedDate = addedDate;
        AcquiredDate = acquiredDate;
        IsFavourite = false;
    }

    public static Item Create(string name, string description, DateTime addedDate, DateTime acquiredDate, string imageUrl, User user)
    {
        return new Item(name, description, addedDate, acquiredDate, imageUrl) { User = user };
    }

    public void SetIsFavourite(bool isFavourite)
    {
        IsFavourite = isFavourite;
    }

    public void AddTag(Tag tag)
    {
        _tags.Add(tag);
    }
}
