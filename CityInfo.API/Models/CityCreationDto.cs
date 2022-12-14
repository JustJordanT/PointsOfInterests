using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models;

public class CityCreationDto
{
    // FluentValidation can be used to validate this property
    [Required(ErrorMessage = "Invalid - You must provide a valid name")]
    [MaxLength(50)]
    public string Name { get; set; }
    [MaxLength(200)]
    public string? Description { get; set; } 
}