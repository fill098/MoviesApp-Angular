using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Domain.Constants;
using MovieApp.Dtos.Reviews;
using MovieApp.Services.Abstractions;

namespace MovieApp.Api.Controllers;

public class ReviewsController : ApiControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    [Authorize(Roles = Roles.Admin)]

    public async Task<ActionResult<List<ReviewReadDto>>> GetAll(CancellationToken cancellationToken)
    {
        var reviews = await _reviewService.GetAllReviewsAsync(cancellationToken);
        return Ok(reviews);
    }

    [HttpGet("{id:int}")]

    public async Task<ActionResult<ReviewReadDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var review = await _reviewService.GetReviewByIdAsync(id, cancellationToken);
        return Ok(review);
    }

    [HttpPost]
    public async Task<ActionResult<ReviewReadDto>> Create(CreateReviewDto request, CancellationToken cancellationToken)
    {
        var newReview = await _reviewService.CreateReviewAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = newReview.Id }, newReview);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<ReviewReadDto>> Update(int id,UpdateReviewDto request, CancellationToken cancellationToken)
    {
        var updatedReview = await _reviewService.UpdateReviewAsync(id, request, cancellationToken);
        return Ok(updatedReview);
    }
}
