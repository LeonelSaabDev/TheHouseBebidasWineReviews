using System.ComponentModel.DataAnnotations;
using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Application.DTOs.Reviews;

public sealed record CreateReviewRequestDto(
    [param: Required(AllowEmptyStrings = false)]
    [param: StringLength(Review.MaximumAuthorNameLength)]
    string AuthorName,
    [param: Required(AllowEmptyStrings = false)]
    [param: StringLength(Review.MaximumCommentLength)]
    string Comment,
    [param: Range(Review.MinimumRating, Review.MaximumRating)]
    int Rating);
