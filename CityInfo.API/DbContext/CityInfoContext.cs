using CityInfo.API.Entities;
using CityInfo.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContext;

public class CityInfoContext :Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<PointOfInterest> PointsOfInterest { get; set; } = null!;

    // This is how we are able to set our DB settings from the program.cs file via the builder services.
    public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options)
    {
        
    }

    //This is how we are able to set our DB settings from the here using the connection string that we want to pass through.
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseSqlite("");
    //     base.OnConfiguring(optionsBuilder);
    // }
    
    // This is how we are able to set DB things at runtime.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>().HasData(
            new City("New York")
            {
                Id = 1,
                Description = "The city with the big park!",
            },
            new City("Antwerp")
            {
                Id = 2,
                Description = "The building that was never finished",
            },
            new City("Paris")
            {
                Id = 3,
                Description = "The city with the Eiffel Tower",
            });
        modelBuilder.Entity<PointOfInterest>().HasData(
            new PointOfInterest("Central Park")
            {
                Id = 1,
                CityId = 1,
                Description = "The most visited urban park in the United States."
            },
            new PointOfInterest("Empire State Building")
            {
                Id = 2,
                CityId = 1,
                Description = "The Empire State building in the United States."
            },
            new PointOfInterest("Cathedral")
            {
                Id = 3,
                CityId = 2,
                Description = "The building the was never finished."
            },
            new PointOfInterest("Eiffel Tower")
            {
                Id = 4,
                CityId = 3,
                Description = "The city with the Eiffel Tower"
            },
            new PointOfInterest("Le Louvre")
            {
                Id = 5,
                CityId = 3,
                Description = "The world's largest museum."
            }
        );
        base.OnModelCreating(modelBuilder);
    }
}