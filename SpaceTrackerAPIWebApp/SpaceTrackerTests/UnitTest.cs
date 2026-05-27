using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceTrackerApp.Controllers;
using SpaceTrackerApp.Models;
using System.Linq;
using Xunit;

namespace SpaceTrackerTests
{
    public static class TestDbHelper
    {
        public static SpaceTrackerContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<SpaceTrackerContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new SpaceTrackerContext(options);
        }
    }

    //FavoritesController
    public class FavoritesControllerTests
    {
        // GET /api/favorites
        [Fact]
        public async Task GetAll_ReturnsFavorites_WhenExist()
        {
            using var context = TestDbHelper.CreateContext("GetAll_WithData");
            context.Favorites.Add(new Favorite
            {
                Title = "Test Photo",
                UserId = 1,
                NasaDate = "2026-01-01",
                ImageUrl = "https://test.jpg",
                SavedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var controller = new FavoritesController(context);
            var result = await controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<Favorite>>(ok.Value);
            Assert.Single(list);
            Assert.Equal("Test Photo", list[0].Title);
        }

        // GET /api/favorites/1
        [Fact]
        public async Task Get_ReturnsFavorite_WhenExists()
        {
            using var context = TestDbHelper.CreateContext("Get_ById");
            var fav = new Favorite
            {
                Title = "Nebula Photo",
                UserId = 1,
                NasaDate = "2026-02-01",
                ImageUrl = "https://nebula.jpg",
                SavedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Favorites.Add(fav);
            await context.SaveChangesAsync();

            var controller = new FavoritesController(context);
            var result = await controller.Get(fav.Id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<Favorite>(ok.Value);
            Assert.Equal("Nebula Photo", returned.Title);
        }

        // GET /api/favorites/999 (повертає 404)
        [Fact]
        public async Task Get_ReturnsNotFound_WhenNotExists()
        {
            using var context = TestDbHelper.CreateContext("Get_NotFound");
            var controller = new FavoritesController(context);

            var result = await controller.Get(999);

            Assert.IsType<NotFoundResult>(result);
        }

        // POST /api/favorites (додає нове фото в улюблене)
        [Fact]
        public async Task Add_CreatesFavorite_AndReturnsOk()
        {
            using var context = TestDbHelper.CreateContext("Add_Favorite");
            var controller = new FavoritesController(context);

            var favorite = new Favorite
            {
                Title = "Galaxy Photo",
                UserId = 1,
                NasaDate = "2026-03-01",
                ImageUrl = "https://galaxy.jpg"
            };

            var result = await controller.Add(favorite);

            var ok = Assert.IsType<OkObjectResult>(result);
            var created = Assert.IsType<Favorite>(ok.Value);
            Assert.Equal("Galaxy Photo", created.Title);
            Assert.Equal(1, await context.Favorites.CountAsync());
        }

        // DELETE /api/favorites/1 (видаляє з улюблених)
        [Fact]
        public async Task Delete_RemovesFavorite_WhenExists()
        {
            using var context = TestDbHelper.CreateContext("Delete_Favorite");
            var fav = new Favorite
            {
                Title = "To Delete",
                UserId = 1,
                NasaDate = "2026-06-01",
                ImageUrl = "https://delete.jpg",
                SavedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Favorites.Add(fav);
            await context.SaveChangesAsync();

            var controller = new FavoritesController(context);
            var result = await controller.Delete(fav.Id);

            Assert.IsType<OkResult>(result);
            Assert.Equal(0, await context.Favorites.CountAsync());
        }

        // DELETE /api/favorites/999 (повертає 404)
        [Fact]
        public async Task Delete_ReturnsNotFound_WhenNotExists()
        {
            using var context = TestDbHelper.CreateContext("Delete_NotFound");
            var controller = new FavoritesController(context);

            var result = await controller.Delete(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
    //AuthController
    public class AuthControllerTests
    {
        // POST /api/auth/register (реєстрація)
        [Fact]
        public async Task Register_ReturnsOk_WithValidData()
        {
            using var context = TestDbHelper.CreateContext("Register_Valid");
            var controller = new AuthController(context);

            var result = await controller.Register(new AuthRequest
            {
                Username = "TestUser",
                Email = "test@test.com",
                Password = "123456"
            });

            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(1, await context.Users.CountAsync());
        }

        // POST /api/auth/register (порожні поля)
        [Fact]
        public async Task Register_ReturnsBadRequest_WhenFieldsEmpty()
        {
            using var context = TestDbHelper.CreateContext("Register_Empty");
            var controller = new AuthController(context);

            var result = await controller.Register(new AuthRequest
            {
                Username = "",
                Email = "",
                Password = ""
            });

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Заповніть всі поля", bad.Value);
        }

        // POST /api/auth/register (існуючий email)
        [Fact]
        public async Task Register_ReturnsBadRequest_WhenEmailExists()
        {
            using var context = TestDbHelper.CreateContext("Register_Duplicate");
            context.Users.Add(new User
            {
                Username = "Existing",
                Email = "exist@test.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var controller = new AuthController(context);
            var result = await controller.Register(new AuthRequest
            {
                Username = "NewUser",
                Email = "exist@test.com",
                Password = "123456"
            });

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Користувач з таким email вже існує", bad.Value);
        }

        // POST /api/auth/login (вхід)
        [Fact]
        public async Task Login_ReturnsOk_WithCorrectCredentials()
        {
            using var context = TestDbHelper.CreateContext("Login_Valid");
            var controller = new AuthController(context);

            await controller.Register(new AuthRequest
            {
                Username = "LoginUser",
                Email = "login@test.com",
                Password = "password123"
            });

            var result = await controller.Login(new AuthRequest
            {
                Email = "login@test.com",
                Password = "password123"
            });

            Assert.IsType<OkObjectResult>(result);
        }

        // POST /api/auth/login  (невірний пароль)
        [Fact]
        public async Task Login_ReturnsUnauthorized_WithWrongPassword()
        {
            using var context = TestDbHelper.CreateContext("Login_WrongPassword");
            var controller = new AuthController(context);

            await controller.Register(new AuthRequest
            {
                Username = "WrongPass",
                Email = "wrong@test.com",
                Password = "correct"
            });

            var result = await controller.Login(new AuthRequest
            {
                Email = "wrong@test.com",
                Password = "incorrect"
            });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        // POST /api/auth/login (неіснуючий користувач)
        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenUserNotFound()
        {
            using var context = TestDbHelper.CreateContext("Login_NotFound");
            var controller = new AuthController(context);

            var result = await controller.Login(new AuthRequest
            {
                Email = "nobody@test.com",
                Password = "password"
            });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}