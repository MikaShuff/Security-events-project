//OfficerListDto.cs

namespace SecurityEvents.Api.Dtos;

public record OfficerListDto(
    int OfficerId,
    string OfficerName,
    string ZoneName
);
