using CityInfo.API.Entities;

namespace CityInfo.API.Services.Interfaces;

public interface ICityInfoRepository
{
    Task<IEnumerable<City>> GetCitiesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<City>> GetCitiesAsync(
        string? name,
        string? searchQuery,
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
    Task<bool> SaveChangesAsync();

    void DeletePointOfInterest(PointOfInterest pointOfInterest);
    // TODO Clean up this overload method we could just have the cancellationToken be nullable
    Task<PointOfInterest?> GetPointOfInterestForCityByIdAsync(int cityId, int pointOfInterestId);
}