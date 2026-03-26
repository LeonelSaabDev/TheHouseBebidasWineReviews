namespace TheHouseBebidas.WineReviews.Domain.Entities;

public sealed class SiteContent
{
    private SiteContent()
    {
        Key = string.Empty;
        Title = string.Empty;
        Content = string.Empty;
    }

    public Guid Id { get; private set; }
    public string Key { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public SiteContent(
        Guid id,
        string key,
        string title,
        string content,
        DateTime? updatedAt = null)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Key = ValidateRequired(key, nameof(key));
        Title = ValidateRequired(title, nameof(title));
        Content = ValidateRequired(content, nameof(content));
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }

    public void Update(string title, string content)
    {
        Title = ValidateRequired(title, nameof(title));
        Content = ValidateRequired(content, nameof(content));
        UpdatedAt = DateTime.UtcNow;
    }

    private static string ValidateRequired(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }
}
