namespace ZtmHub.Application.DTOs;

public record StopDeparturesDto(
    int StopId,
    string StopName,
    IEnumerable<DepartureDto> Departures
);

public record DepartureDto(
    string Line,
    string Direction,
    DateTime EstimatedTime,
    string ScheduledTime,
    int DelaySeconds,
    string Status
);