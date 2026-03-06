using FacilityHub.helper;
using FacilityHub.Infra;
using FacilityHub.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddInfraServices();
builder.Services.AddApplicationServices();
builder.Services.AddApiServices(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();