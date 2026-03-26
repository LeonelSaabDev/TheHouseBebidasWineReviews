using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Tests.Domain;

public sealed class ReviewDomainTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Constructor_ShouldRejectRatingOutsideAllowedRange(int invalidRating)
    {
        var wineId = Guid.NewGuid();

        var action = () => new Review(
            id: Guid.Empty,
            wineId: wineId,
            comment: "Excelente vino",
            authorName: "Lucia",
            rating: invalidRating);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Theory]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_ShouldRejectEmptyComment(string? invalidComment)
    {
        var wineId = Guid.NewGuid();

        var action = () => new Review(
            id: Guid.Empty,
            wineId: wineId,
            comment: invalidComment!,
            authorName: "Lucia",
            rating: 5);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Constructor_ShouldRejectCommentLongerThan400Characters()
    {
        var wineId = Guid.NewGuid();
        var tooLongComment = new string('a', Review.MaximumCommentLength + 1);

        var action = () => new Review(
            id: Guid.Empty,
            wineId: wineId,
            comment: tooLongComment,
            authorName: "Lucia",
            rating: 5);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_ShouldRejectEmptyAuthorName(string? invalidAuthorName)
    {
        var wineId = Guid.NewGuid();

        var action = () => new Review(
            id: Guid.Empty,
            wineId: wineId,
            comment: "Excelente vino",
            authorName: invalidAuthorName!,
            rating: 5);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Constructor_ShouldRejectAuthorNameLongerThan80Characters()
    {
        var wineId = Guid.NewGuid();
        var tooLongAuthorName = new string('a', Review.MaximumAuthorNameLength + 1);

        var action = () => new Review(
            id: Guid.Empty,
            wineId: wineId,
            comment: "Excelente vino",
            authorName: tooLongAuthorName,
            rating: 5);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }
}
