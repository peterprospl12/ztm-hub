namespace ZtmHub.Application.DTOs;

public record UserStopDto(
    Guid Id,
    int StopId,
    string? DisplayName,
    DateTime AddedAt
);