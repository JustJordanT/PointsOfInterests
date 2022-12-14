using System.Security.Claims;
using System.Text;
using CityInfo.API;
using CityInfo.API.DbContext;
using CityInfo.API.Services;
using CityInfo.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
// TODO ADD XML Documentation for swagger
builder.Services.AddSwaggerGen();

// Adds file types
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddSingleton<CityDataStore>();

// Ef Core Services
// TODO Docker container, not working for API Calls since db context is not available. ie sqlite database is not configured.
builder.Services.AddDbContext<CityInfoContext>(optionsBuilder =>
    optionsBuilder.UseSqlite(builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"]));

// Repository Pattern
builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

//Auto Mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(optionsBuilder =>
{
    optionsBuilder.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
        
    };
});

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeFromUSA", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireClaim(ClaimTypes.GivenName, "jordan");
    });
});

// Versioning
builder.Services.AddApiVersioning(action =>
{
    action.AssumeDefaultVersionWhenUnspecified = true;
    action.DefaultApiVersion = new ApiVersion(1, 0);
    action.ReportApiVersions = true;
});

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
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// One way to configure
// app.MapControllers();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

// app.Run(async (context) =>
// {
//     await context.Response.WriteAsync("Hello World!");
// });

app.Run();