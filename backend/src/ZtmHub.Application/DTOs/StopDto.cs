namespace ZtmHub.Application.DTOs;

public record StopDto(
    int Id,
    string Name,
    string Code,
    double Lat,
    double Lon
);