using DotNetTask.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Add DbContext configuration for Cosmos DB
//builder.Services.AddDbContext<DotNetTaskDbContext>(options =>
//    options.UseCosmos(
//        builder.Configuration["CosmosDb:AccountEndpoint"],
//        builder.Configuration["CosmosDb:AccountKey"],
//        builder.Configuration["CosmosDb:DatabaseName"]
//    ));

builder.Services.AddDbContext<DotNetTaskDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
