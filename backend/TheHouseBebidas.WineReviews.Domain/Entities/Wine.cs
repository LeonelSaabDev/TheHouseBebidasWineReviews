namespace TheHouseBebidas.WineReviews.Domain.Entities;

public sealed class Wine
{
    private Wine()
    {
        Name = string.Empty;
        Winery = string.Empty;
        GrapeVariety = string.Empty;
        Description = string.Empty;
        ImageUrl = string.Empty;
        SecondaryImageUrl = null;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Winery { get; private set; }
    public int Year { get; private set; }
    public string GrapeVariety { get; private set; }
    public string Description { get; private set; }
    public string ImageUrl { get; private set; }
    public string? SecondaryImageUrl { get; private set; }
    public string? FeaturedReviewSummary { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Wine(
        Guid id,
        string name,
        string winery,
        int year,
        string grapeVariety,
        string description,
        string imageUrl,
        string? secondaryImageUrl = null,
        string? featuredReviewSummary = null,
        bool isActive = true,
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Name = ValidateRequired(name, nameof(name));
        Winery = ValidateRequired(winery, nameof(winery));
        Year = ValidateYear(year);
        GrapeVariety = ValidateRequired(grapeVariety, nameof(grapeVariety));
        Description = ValidateRequired(description, nameof(description));
        ImageUrl = ValidateRequired(imageUrl, nameof(imageUrl));
        SecondaryImageUrl = NormalizeOptional(secondaryImageUrl);
        FeaturedReviewSummary = NormalizeOptional(featuredReviewSummary);
        IsActive = isActive;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }

    public void UpdateDetails(
        string name,
        string winery,
        int year,
        string grapeVariety,
        string description,
        string imageUrl,
        string? secondaryImageUrl,
        bool isActive)
    {
        Name = ValidateRequired(name, nameof(name));
        Winery = ValidateRequired(winery, nameof(winery));
        Year = ValidateYear(year);
        GrapeVariety = ValidateRequired(grapeVariety, nameof(grapeVariety));
        Description = ValidateRequired(description, nameof(description));
        ImageUrl = ValidateRequired(imageUrl, nameof(imageUrl));
        SecondaryImageUrl = NormalizeOptional(secondaryImageUrl);
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetFeaturedReviewSummary(string? featuredReviewSummary)
    {
        FeaturedReviewSummary = NormalizeOptional(featuredReviewSummary);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
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

    private static int ValidateYear(int year)
    {
        var currentYear = DateTime.UtcNow.Year + 1;

        if (year < 1900 || year > currentYear)
        {
            throw new ArgumentOutOfRangeException(nameof(year), $"Year must be between 1900 and {currentYear}.");
        }

        return year;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
