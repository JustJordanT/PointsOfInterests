using AutoMapper;

namespace CityInfo.API.Mappers;

public class PointsOfInterestProfile : Profile
{
    public PointsOfInterestProfile()
    {
        CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
        CreateMap<Models.PointOfInterestCreationDto, Entities.PointOfInterest>();
        CreateMap<Models.PointOfInterestUpdateDto, Entities.PointOfInterest>();
        CreateMap<Entities.PointOfInterest, Models.PointOfInterestUpdateDto>();    
    }
}