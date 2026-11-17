using MovieApp.Dtos.Directors;

namespace MovieApp.Services.Abstractions;

public interface IDirectorService
{
    Task<List<DirectorReadDto>> GetAllDirectorsAsync(CancellationToken cancellationToken);
    Task<DirectorReadDto> GetDirectorByIdAsync(int id, CancellationToken cancellationToken);
    Task<DirectorReadDto> CreateDirectorAsync(CreateDirectorDto request, CancellationToken cancellationToken);
    Task<DirectorReadDto> UpdateDirectorAsync(int id, UpdateDirectorDto request, CancellationToken cancellationToken);
    Task DeleteDirectorAsync(int id, CancellationToken cancellationToken);
}
