using Microsoft.AspNetCore.Mvc.Versioning;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;

builder.Services.AddTransient<ILookupNameValuePairRepository>(sp => new LookupNameValuePairRepository(configuration[Constants.AppSettingsKey.PasStorageConnectionString]));

//See - for basics: https://dotnetcoretutorials.com/2017/01/17/api-versioning-asp-net-core/
//See - to implement ApiVersionReader.Combine: https://github.com/dotnet/aspnet-api-versioning/wiki/API-Version-Reader
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("version"),
        new HeaderApiVersionReader("api-version"));
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1.0", //Note: This version value must match the json file version number below. ie: "/swagger/v1.0/swagger.json"
        new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "PasLookupData.Api",
            Description = "API for PAS Lookup Data",
            Version = "v1.0",
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (configuration[Constants.AppSettingsKey.Environment] != Constants.Environment.Production)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1.0/swagger.json", "PasLookupData.Api v1.0");
        }
    );
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
