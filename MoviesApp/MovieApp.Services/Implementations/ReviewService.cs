using MapsterMapper;
using MovieApp.Common.Extensinons;
using MovieApp.DataAccess.Repositories.Abstractions;
using MovieApp.Domain.Entities;
using MovieApp.Dtos.Reviews;
using MovieApp.Services.Abstractions;

namespace MovieApp.Services.Implementations;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    private readonly IMovieRepository _movieRepository;

    public ReviewService(IReviewRepository repository, IMapper mapper, ICurrentUser currentUser, IMovieRepository movieRepository)
    {
        _reviewRepository = repository;
        _mapper = mapper;
        _currentUser = currentUser;
        _movieRepository = movieRepository;
    }


    public async Task<List<ReviewReadDto>> GetAllReviewsAsync(CancellationToken cancellationToken)
    {
        var reviewsDb = await _reviewRepository.GetAllAsync(cancellationToken);

        return _mapper.Map<List<ReviewReadDto>>(reviewsDb);
    }

    public async Task<ReviewReadDto> GetReviewByIdAsync(int id, CancellationToken cancellationToken)
    {
        
        var reviewDb = await _reviewRepository.GetByIdAsync(id, cancellationToken);
        if (reviewDb is null)
        {
            throw new NotFoundException(nameof(Review), id);
        }

        return _mapper.Map<ReviewReadDto>(reviewDb);
    }
    public async Task<ReviewReadDto> CreateReviewAsync(CreateReviewDto createReview, CancellationToken cancellationToken)
    {
        if(!await _movieRepository.ExistsMovieAsync(createReview.MovieId, cancellationToken))
        {
            throw new BadRequestException($"Unknown movie id: {createReview.MovieId}");
        }

        var userId = _currentUser.Id;

        if(await _reviewRepository.ExistsForUserAndMovieAsync(userId, createReview.MovieId, cancellationToken))
        {
            throw new ConflictException("You have already reviewed this movie.");
        }

        var review = _mapper.Map<Review>(createReview);

        review.UserId = userId;

        await _reviewRepository.AddAsync(review, cancellationToken);

        var created = await _reviewRepository.GetByIdAsync(review.Id, cancellationToken);

        return _mapper.Map<ReviewReadDto>(created!);
    }

    public async Task<ReviewReadDto> UpdateReviewAsync(int id, UpdateReviewDto updateReview, CancellationToken cancellationToken)
    {
        var reviewDb = await _reviewRepository.GetByIdTrackedAsync(id, cancellationToken);
        if (reviewDb is null)
        {
            throw new NotFoundException(nameof(Review), id);
        }
        if (reviewDb.UserId != _currentUser.Id && !_currentUser.IsAdmin)
        {
            throw new ForbiddenException("You can only change your own reviews.");
        }

        _mapper.Map(updateReview, reviewDb);

        await _reviewRepository.UpdateAsync(reviewDb, cancellationToken);

        var updated = await _reviewRepository.GetByIdAsync(reviewDb.Id, cancellationToken);

        return _mapper.Map<ReviewReadDto>(updated!);
    }
}   
