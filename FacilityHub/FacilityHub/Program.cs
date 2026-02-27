using FacilityHub.Core.Entities;
using FacilityHub.helper;
using FacilityHub.Infra;
using FacilityHub.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.AddInfraServices();
builder.Services.AddApplicationServices();
builder.Services.AddApiServices(builder.Configuration);




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()){
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();     
app.UseAuthorization();      
app.MapControllers();

app.Run();