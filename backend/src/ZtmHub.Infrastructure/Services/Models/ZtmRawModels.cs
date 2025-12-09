using System.Text.Json.Serialization;

namespace ZtmHub.Infrastructure.Services.Models;

internal record ZtmStopJson(
    [property: JsonPropertyName("stopId")] int StopId,
    [property: JsonPropertyName("stopCode")]
    string? StopCode,
    [property: JsonPropertyName("stopName")]
    string? StopName,
    [property: JsonPropertyName("stopDesc")]
    string? StopDesc,
    [property: JsonPropertyName("stopLat")]
    double Lat,
    [property: JsonPropertyName("stopLon")]
    double Lon
);

internal record ZtmStopsNode(
    [property: JsonPropertyName("lastUpdate")]
    string LastUpdate,
    [property: JsonPropertyName("stops")] List<ZtmStopJson> Stops
);

internal record ZtmVehicleJson(
    [property: JsonPropertyName("routeId")]
    int RouteId,
    [property: JsonPropertyName("headsign")]
    string Headsign,
    [property: JsonPropertyName("delayInSeconds")]
    int? DelayInSeconds,
    [property: JsonPropertyName("estimatedTime")]
    DateTime EstimatedTime,
    [property: JsonPropertyName("theoreticalTime")]
    DateTime TheoreticalTime,
    [property: JsonPropertyName("status")] string Status
);

internal record ZtmDeparturesResponse(
    [property: JsonPropertyName("lastUpdate")]
    DateTime LastUpdate,
    [property: JsonPropertyName("departures")]
    List<ZtmVehicleJson> Departures
);