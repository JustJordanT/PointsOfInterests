using System.Security.Claims;
using CityInfo.API.Entities;
using CityInfo.API.Models;

namespace CityInfo.API.Services.Interfaces;

public partial interface ICityInfoRepository
{
    Task<IEnumerable<City>> GetCitiesAsync(CancellationToken cancellationToken);
    Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(
        string? name,
        string? searchQuery,
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken);

    Task<City?> GetCityByIdAsync(
        int cityId,
        bool includePointsOfInterest,
        CancellationToken cancellationToken);
    // Task<City> GetCityByNameAsync(string name, CancellationToken cancellationToken);

    public Task<bool> CityExistsAsync(int cityId);

    Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId, CancellationToken cancellationToken);
    Task<PointOfInterest> GetPointOfInterestForCityByIdAsync(int cityId, int pointOfInterestId,
        CancellationToken cancellationToken);
    
    Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest, CancellationToken cancellationToken);

    Task AddCityAsync(CityCreationDto city, CancellationToken cancellationToken);
    
    Task<bool> CityNameMatchesCityId(string cityName, int cityId);
    
    Task<bool> SaveChangesAsync();

    public Task SaveDBChangesAsync(CancellationToken cancellationToken);

    void DeletePointOfInterest(PointOfInterest pointOfInterest);
    // TODO Clean up this overload method we could just have the cancellationToken be nullable
    Task<PointOfInterest?> GetPointOfInterestForCityByIdAsync(int cityId, int pointOfInterestId);
}