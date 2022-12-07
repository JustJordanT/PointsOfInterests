using CityInfo.API.DbContext;
using CityInfo.API.Entities;
using CityInfo.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services;

public class CityInfoRepository : ICityInfoRepository
{
    private readonly CityInfoContext _context;

    public CityInfoRepository(CityInfoContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<IEnumerable<City>> GetCitiesAsync(CancellationToken cancellationToken)
    {
        return await _context.Cities.OrderBy(c => c.Name).ToListAsync(cancellationToken: cancellationToken);
    }
    public async Task<IEnumerable<City>> GetCitiesAsync(
        string? name,
        string? searchQuery,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(name) && string.IsNullOrWhiteSpace(searchQuery))
            return await GetCitiesAsync(cancellationToken);

        // collection to start from
        var collection = _context.Cities as IQueryable<City>;


        if (!string.IsNullOrWhiteSpace(name))
        {
            name = name.Trim();
            collection = collection.Where(c => c.Name == name);
        }

        // TODO: How would be add lucine filtering with dotnet.
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            collection = collection
                .Where(c => c.Name.ToLower()
                                .Contains(searchQuery) ||
                            (c.Description != null &&
                             c.Description.Contains(searchQuery)));
        }

        return await collection.OrderBy(c => c.Name).ToListAsync(cancellationToken: cancellationToken);

    }

    public async Task<City?> GetCityByIdAsync(int cityId, bool includePointsOfInterest ,CancellationToken cancellationToken)
    {
        if (includePointsOfInterest)
        {
            return await _context.Cities.Include(c => c.PointsOfInterest)
                .Where(c => c.Id == cityId).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        return await _context.Cities
            .Where(c => c.Id == cityId).FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async  Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId, CancellationToken cancellationToken)
    {
        return await _context.PointsOfInterest
            .Where(p => p.CityId == cityId)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<bool> CityExistsAsync(int cityId)
    {
        return await _context.Cities.AnyAsync(c => c.Id == cityId);
    }

    public async Task<PointOfInterest?> GetPointOfInterestForCityByIdAsync(int cityId, int pointOfInterestId,
        CancellationToken cancellationToken)
    {
       return await _context.PointsOfInterest
           .Where(p => p.CityId == cityId && p.Id == pointOfInterestId)
           .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest, CancellationToken cancellationToken)
    {
        var city = await GetCityByIdAsync(cityId, false, cancellationToken);
        // If check
        // if (city != null)
            // city.PointsOfInterest.Add(pointOfInterest);
        // null check using null propagation
        city?.PointsOfInterest.Add(pointOfInterest);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }

    public void DeletePointOfInterest(PointOfInterest pointOfInterest)
    {
        _context.PointsOfInterest.Remove(pointOfInterest);
    }

    public async Task<PointOfInterest?> GetPointOfInterestForCityByIdAsync(int cityId, int pointOfInterestId)
    {
        return await _context.PointsOfInterest
            .Where(p => p.CityId == cityId && p.Id == pointOfInterestId)
            .FirstOrDefaultAsync();    }
}