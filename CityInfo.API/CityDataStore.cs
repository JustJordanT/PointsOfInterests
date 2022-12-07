using CityInfo.API.Models;

namespace CityInfo.API;

public class CityDataStore
{
    public List<CityDto> Cities { get; set; }

    // public static CityDataStore Current { get; } = new CityDataStore();

    public CityDataStore()
    {
        Cities = new List<CityDto>()
        {
            new CityDto()
            {
                Id = 1,
                Name = "London",
                Description = "Big Ben",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 1,
                        Name = "Big Ben",
                        Description = "Big ol clock"
                    },
                    new PointOfInterestDto()
                    {
                        Id = 2,
                        Name = "Big Ben 2",
                        Description = "Big ol clock 2"
                    },
                },
            },
            new CityDto()
            {
                Id = 2,
                Name = "Paris",
                Description = "Paris with Eiffel Tower",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 1,
                        Name = "Eiffel Tower",
                        Description = "The one with a Tower"
                    }
                },
            },
            new CityDto()
            {
                Id = 3,
                Name = "Moscow",
                Description = "Russian Federation",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 1,
                        Name = "Russian Federation Cool Stuff",
                        Description = "Russian Federation Cool Stuff"
                    }
                }
            }
        };
    }
}