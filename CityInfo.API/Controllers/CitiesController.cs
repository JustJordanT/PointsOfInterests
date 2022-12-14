using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CityInfo.API.Controllers;

[ApiController]
// [Authorize]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/cities")]
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
    public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(
        string? name,
        string? searchQuery,
        CancellationToken cancellationToken,
        int pageSize = maxCitiersPageSize,
        int pageNumber = 1)
    {
        if (pageSize > maxCitiersPageSize)
            pageSize = maxCitiersPageSize;
        
        var (citiesEntities, paginationMetadata) = await _cityInfoRepository.GetCitiesAsync(
            name,
            searchQuery,
            pageSize,
            pageNumber,
            cancellationToken);
        // TODO look into this further, HEADERS not showing up.
        Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(paginationMetadata));
        
        return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(citiesEntities));
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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<CityDto>> CreateCity(CityCreationDto city, CancellationToken cancellationToken)
    {
        var finalCity = _mapper.Map<CityCreationDto>(city);

        await _cityInfoRepository.AddCityAsync(finalCity, cancellationToken);
        // TODO do we want to return the id of the city?
        return CreatedAtAction(nameof(GetCities), new { name = finalCity.Name }, finalCity );
    }
}