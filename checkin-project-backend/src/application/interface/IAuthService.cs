using CheckinProjectBackend.Application.DTOs;

namespace CheckinProjectBackend.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
