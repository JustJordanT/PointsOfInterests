using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using CityInfo.API.Services.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/cities/{cityId}/pointsofinterest")]
[ApiController]
public class PointsOfInterestController : ControllerBase
{
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;
    // private readonly CityDataStore _dataStore;
    private readonly IMapper _mapper;
    private readonly ICityInfoRepository _cityInfoRepository;

    public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
        IMailService mailService,
        IMapper mapper,
        ICityInfoRepository cityInfoRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService?? throw new ArgumentNullException(nameof(mailService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));

        // HttpContext.RequestServices.GetService(typeof(ILogger<PointsOfInterestController>));
    }
    
    
    // GET
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId, CancellationToken cancellationToken)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
            return NotFound();
        }
        var pointsofinterestForCity =
            await _cityInfoRepository
                .GetPointsOfInterestForCityAsync(cityId, cancellationToken);
        return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsofinterestForCity));
    }
    
    // GET
    [HttpGet("{pointsofinterestid}", Name = "GetPointsOfInterest")]
    public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(
        int cityId,
        int pointsofInterestId,
        CancellationToken cancellationToken)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of'");
            return NotFound();
        }
        
        var pointsOfInterest = await _cityInfoRepository.GetPointOfInterestForCityByIdAsync(
            cityId,
            pointsofInterestId,
            cancellationToken);

        if (pointsOfInterest == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<PointOfInterestDto>(pointsOfInterest));
    }

    [HttpPost]
    public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId,
        PointOfInterestCreationDto pointOfInterest, CancellationToken cancellationToken)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
            return NotFound();


        var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

        await _cityInfoRepository.AddPointOfInterestForCityAsync(
            cityId,
            finalPointOfInterest,
            cancellationToken);

        await _cityInfoRepository.SaveChangesAsync();

        var createdPointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);
        
        return CreatedAtRoute("GetPointsOfInterest", new
            {
                cityId = cityId,
                pointsofinterestid = createdPointOfInterestToReturn.Id
            }, createdPointOfInterestToReturn
        );
    }
    
    [HttpPut("{pointsofinterestid}")]
    public async Task<ActionResult> UpdatePointOfInterest(
        int cityId,
        int pointsOfInterestId,
        PointOfInterestUpdateDto pointOfInterest,
        CancellationToken cancellationToken)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
            return NotFound();

        var pointOfInterestEntity = await _cityInfoRepository
            .GetPointOfInterestForCityByIdAsync(cityId, pointsOfInterestId, cancellationToken);
        if (pointOfInterestEntity == null)
            return NotFound();

        _mapper.Map(pointOfInterest, pointOfInterestEntity);

        await _cityInfoRepository.SaveChangesAsync();// find point of interest
    
       return NoContent();
    }
    
    [HttpPatch("{pointofinterestid}")]
    public async Task<ActionResult> PartialUpdatePointOfInterest(
        int cityId,
        int pointOfInterestId,
        JsonPatchDocument<PointOfInterestUpdateDto> patchDocument,
        CancellationToken cancellationToken)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
            return NotFound();

        var pointOfInterestEntity = await _cityInfoRepository
            .GetPointOfInterestForCityByIdAsync(cityId, pointOfInterestId, cancellationToken);
    
        if (pointOfInterestEntity == null)
            return NotFound();

        var pointOfInterestToPatch = _mapper.Map<PointOfInterestUpdateDto>(pointOfInterestEntity); 
        
        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
    
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
    
        if (!TryValidateModel(pointOfInterestToPatch))
        {
            return BadRequest(ModelState);
        }
    
        _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
        await _cityInfoRepository.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpDelete("{pointofinterestid}")]
    public async Task<ActionResult> DeletePointOfInterest(
        int cityId,
        int pointOfInterestId)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            return NotFound();
        }

        var pointOfInterestEntity = await _cityInfoRepository
            .GetPointOfInterestForCityByIdAsync(cityId, pointOfInterestId);
        if (pointOfInterestEntity == null)
        {
            return NotFound();
        }

        _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
        await _cityInfoRepository.SaveChangesAsync();

        _mailService.Send($"Point of interest was deleted from {pointOfInterestEntity.Name}",
            $"Point of interest was deleted from {pointOfInterestEntity.Name} with id {pointOfInterestId}");
        return NoContent();
    }
}