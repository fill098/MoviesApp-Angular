using MapsterMapper;
using MovieApp.Common.Extensinons;
using MovieApp.DataAccess.Repositories.Abstractions;
using MovieApp.Domain.Entities;
using MovieApp.Dtos.Reviews;
using MovieApp.Services.Abstractions;

namespace MovieApp.Services.Implementations;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public ReviewService(IReviewRepository repository, IMapper mapper, ICurrentUser currentUser )
    {
        _repository = repository;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<List<ReviewReadDto>> GetAllReviewsAsync(CancellationToken cancellationToken)
    {
        var reviewsDb = await _repository.GetAllAsync(cancellationToken);

        return _mapper.Map<List<ReviewReadDto>>(reviewsDb);
    }

    public async Task<ReviewReadDto> GetReviewByIdAsync(int id, CancellationToken cancellationToken)
    {
        
        var reviewDb = await _repository.GetByIdAsync(id, cancellationToken);
        if (reviewDb is null)
        {
            throw new NotFoundException(nameof(Review), id);
        }

        return _mapper.Map<ReviewReadDto>(reviewDb);
    }
}
