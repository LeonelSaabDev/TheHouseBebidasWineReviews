namespace TheHouseBebidas.WineReviews.Domain.Entities;

public sealed class AdminUser
{
    private AdminUser()
    {
        Username = string.Empty;
        PasswordHash = string.Empty;
        PasswordSalt = string.Empty;
    }

    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }
    public string PasswordSalt { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public AdminUser(
        Guid id,
        string username,
        string passwordHash,
        string passwordSalt,
        bool isActive = true,
        DateTime? createdAt = null)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Username = ValidateRequired(username, nameof(username));
        PasswordHash = ValidateRequired(passwordHash, nameof(passwordHash));
        PasswordSalt = ValidateRequired(passwordSalt, nameof(passwordSalt));
        IsActive = isActive;
        CreatedAt = createdAt ?? DateTime.UtcNow;
    }

    public void UpdateCredentials(string passwordHash, string passwordSalt)
    {
        PasswordHash = ValidateRequired(passwordHash, nameof(passwordHash));
        PasswordSalt = ValidateRequired(passwordSalt, nameof(passwordSalt));
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static string ValidateRequired(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }
}
