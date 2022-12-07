using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CityInfo.API.Models;
using Newtonsoft.Json;

namespace CityInfo.API.Entities;

public class City
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    // [JsonProperty("Id")]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } 
    
    [MaxLength(200)]
    public string? Description { get; set; } 
    
    public ICollection<PointOfInterest> PointsOfInterest { get; set; } = new List<PointOfInterest>();

    public City(string name)
    {
        Name = name;
    }
}