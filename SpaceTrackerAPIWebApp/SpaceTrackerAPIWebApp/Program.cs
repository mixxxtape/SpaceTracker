using Microsoft.EntityFrameworkCore;
using SpaceTrackerAPIWebApp.Models;
using SpaceTrackerAPIWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SpaceTrackerContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddHttpClient<NasaService>();
builder.Services.AddHttpClient<IssService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SpaceTrackerContext>();
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();