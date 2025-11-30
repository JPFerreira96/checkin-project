namespace CheckinProjectBackend.Application.DTOs;

public sealed record LoginRequestDto(string Email, string Password);
public sealed record LoginResponseDto(int EmployeeId, string Name, string Role);
