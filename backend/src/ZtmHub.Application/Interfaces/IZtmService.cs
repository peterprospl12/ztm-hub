using ZtmHub.Application.DTOs;

namespace ZtmHub.Application.Interfaces;

public interface IZtmService
{
    Task<IEnumerable<StopDto>> GetAllStopsAsync(CancellationToken ct = default);
    Task<StopDeparturesDto?> GetDeparturesAsync(int stopId, CancellationToken ct = default);
    Task<StopDto?> GetStopInfoAsync(int stopId, CancellationToken ct = default);
}