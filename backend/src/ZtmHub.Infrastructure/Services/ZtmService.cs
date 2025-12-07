using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ZtmHub.Application.DTOs;
using ZtmHub.Application.Interfaces;
using ZtmHub.Infrastructure.Services.Models;

namespace ZtmHub.Infrastructure.Services;

public class ZtmService(
    IHttpClientFactory httpClientFactory,
    IMemoryCache cache,
    IConfiguration config) : IZtmService
{
    private const string CacheKey = "ZtmStops";

    public async Task<IEnumerable<StopDto>> GetAllStopsAsync(CancellationToken ct = default)
    {
        if (cache.TryGetValue(CacheKey, out IEnumerable<StopDto>? cachedStops) && cachedStops is not null)
            return cachedStops;

        var client = httpClientFactory.CreateClient("ZtmClient");
        var url = config["ZtmSettings:StopsUrl"];

        var dictResponse = await client.GetFromJsonAsync<Dictionary<string, ZtmStopsNode>>(url, ct);

        if (dictResponse is null || dictResponse.Count == 0)
            return [];

        var stopsNode = dictResponse.Values.FirstOrDefault();

        if (stopsNode is null || stopsNode.Stops.Count == 0)
            return [];

        var stopsDto = stopsNode.Stops.Select(s => new StopDto(
            s.StopId,
            s.StopName ?? s.StopDesc ?? "Unknown",
            s.StopCode ?? "",
            s.Lat,
            s.Lon
        )).ToList();

        var cacheHours = config["ZtmSettings:CacheDurationHours"];
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(int.TryParse(cacheHours, out var hours) ? hours : 24));

        cache.Set(CacheKey, stopsDto, cacheEntryOptions);

        return stopsDto;
    }

    public async Task<StopDeparturesDto?> GetDeparturesAsync(int stopId, CancellationToken ct = default)
    {
        var allStops = await GetAllStopsAsync(ct);
        var stopInfo = allStops.FirstOrDefault(s => s.Id == stopId);

        if (stopInfo is null) return null;

        var client = httpClientFactory.CreateClient("ZtmClient");
        var baseUrl = config["ZtmSettings:DelaysBaseUrl"];

        try
        {
            var response = await client.GetFromJsonAsync<ZtmDeparturesResponse>($"{baseUrl}{stopId}", ct);

            if (response is null || response.Departures is null)
                return new StopDeparturesDto(stopInfo.Id, stopInfo.Name, []);

            var departures = response.Departures.Select(d => new DepartureDto(
                d.RouteId.ToString(),
                d.Headsign,
                d.EstimatedTime.ToLocalTime(),
                d.TheoreticalTime.ToLocalTime().ToString("HH:mm"),
                d.DelayInSeconds ?? 0,
                d.Status
            ));

            return new StopDeparturesDto(stopInfo.Id, stopInfo.Name, departures);
        }
        catch (Exception)
        {
            return new StopDeparturesDto(stopInfo.Id, stopInfo.Name, []);
        }
    }

    public async Task<StopDto?> GetStopInfoAsync(int stopId, CancellationToken ct = default)
    {
        var allStops = await GetAllStopsAsync(ct);

        return allStops.FirstOrDefault(s => s.Id == stopId);
    }
}