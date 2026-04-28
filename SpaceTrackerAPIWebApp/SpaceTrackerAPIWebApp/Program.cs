using Microsoft.EntityFrameworkCore;
using SpaceTrackerApp.Models;
using SpaceTrackerApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Підключення БД
builder.Services.AddDbContext<SpaceTrackerContext>(option =>
    option.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Додаємо контролери
builder.Services.AddControllersWithViews();

// Додаємо HttpClient для NASA та ISS сервісів
builder.Services.AddHttpClient<NasaService>();
builder.Services.AddHttpClient<IssService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseDefaultFiles();
app.UseStaticFiles();
app.Run();