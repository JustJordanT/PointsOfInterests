using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    private static ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;
    const int maxCitiersPageSize = 20;

    // Calling Local Datastore no ef core
    // private readonly CityDataStore _dataStore;
    //
    // public CitiesController(CityDataStore dataStore)
    // {
    //     _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
    // }

    public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
    {
        _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(string? name,
        string? searchQuery,
        CancellationToken cancellationToken,
        int pageSize = maxCitiersPageSize,
        int page = 1)
    {
        var cities = await _cityInfoRepository.GetCitiesAsync(name, searchQuery, cancellationToken);
        return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cities));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCities(int id, CancellationToken cancellationToken,
        bool includePointsOfInterest = false)
    {
        var city = await _cityInfoRepository.GetCityByIdAsync(id, includePointsOfInterest, cancellationToken);
        if (city == null)
            return NotFound();
       
        return includePointsOfInterest ? Ok(_mapper.Map<CityDto>(city)) : Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
    }
}