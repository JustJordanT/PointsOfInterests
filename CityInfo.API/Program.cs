using CityInfo.API;
using CityInfo.API.DbContext;
using CityInfo.API.Services;
using CityInfo.API.Services.Interfaces;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
// builder.Logging.ClearProviders();
// builder.Logging.AddConsole();
builder.Host.UseSerilog();


// Add services to the container.

builder.Services.AddControllers(options => { options.ReturnHttpNotAcceptable = true; }).AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at
// https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adds file types
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddSingleton<CityDataStore>();

// Ef Core Services
builder.Services.AddDbContext<CityInfoContext>(optionsBuilder =>
    optionsBuilder.UseSqlite(builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"]));

// Repository Pattern
builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

//Auto Mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

var app = builder.Build();


// if (app.Environment.IsDevelopment())
//     builder.Services.AddDbContext<CityInfoContext>(optionsBuilder =>
//         optionsBuilder.UseSqlite(builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"]));

// builder.Services.AddDbContext<CityInfoContext>(optionsBuilder =>
//     optionsBuilder.UseSqlite(builder.Configuration["KEYSTRINGDB"]));


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

// One way to configure
// app.MapControllers();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

// app.Run(async (context) =>
// {
//     await context.Response.WriteAsync("Hello World!");
// });

app.Run();