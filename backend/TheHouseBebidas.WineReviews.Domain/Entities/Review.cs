namespace TheHouseBebidas.WineReviews.Domain.Entities;

public sealed class Review
{
    private Review()
    {
        Comment = string.Empty;
        AuthorName = string.Empty;
    }

    public const int MinimumRating = 1;
    public const int MaximumRating = 5;
    public const int MaximumCommentLength = 400;
    public const int MaximumAuthorNameLength = 80;

    public Guid Id { get; private set; }
    public Guid WineId { get; private set; }
    public string Comment { get; private set; }
    public string AuthorName { get; private set; }
    public int Rating { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsVisible { get; private set; }

    public Review(
        Guid id,
        Guid wineId,
        string comment,
        string authorName,
        int rating,
        bool isVisible = true,
        DateTime? createdAt = null)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        WineId = wineId == Guid.Empty
            ? throw new ArgumentException("WineId is required.", nameof(wineId))
            : wineId;
        Comment = ValidateComment(comment);
        AuthorName = ValidateAuthorName(authorName);
        Rating = ValidateRating(rating);
        IsVisible = isVisible;
        CreatedAt = createdAt ?? DateTime.UtcNow;
    }

    public void UpdateContent(string comment, int rating)
    {
        Comment = ValidateComment(comment);
        Rating = ValidateRating(rating);
    }

    public void UpdateAuthorName(string authorName)
    {
        AuthorName = ValidateAuthorName(authorName);
    }

    public void Hide() => IsVisible = false;

    public void Show() => IsVisible = true;

    private static string ValidateComment(string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new ArgumentException("Comment is required.", nameof(comment));
        }

        var sanitizedComment = comment.Trim();

        if (sanitizedComment.Length > MaximumCommentLength)
        {
            throw new ArgumentOutOfRangeException(nameof(comment), $"Comment cannot exceed {MaximumCommentLength} characters.");
        }

        return sanitizedComment;
    }

    private static int ValidateRating(int rating)
    {
        if (rating < MinimumRating || rating > MaximumRating)
        {
            throw new ArgumentOutOfRangeException(nameof(rating), $"Rating must be between {MinimumRating} and {MaximumRating}.");
        }

        return rating;
    }

    private static string ValidateAuthorName(string authorName)
    {
        if (string.IsNullOrWhiteSpace(authorName))
        {
            throw new ArgumentException("Author name is required.", nameof(authorName));
        }

        var sanitizedAuthorName = authorName.Trim();

        if (sanitizedAuthorName.Length > MaximumAuthorNameLength)
        {
            throw new ArgumentOutOfRangeException(nameof(authorName), $"Author name cannot exceed {MaximumAuthorNameLength} characters.");
        }

        return sanitizedAuthorName;
    }
}
