using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Trucks.Controllers;
using Trucks.Data;
using Trucks.Models;
using Xunit;

namespace Trucks.Test
{
    public class DatabaseFixture : IDisposable
    {
        private readonly Lazy<DbContextOptions<TrucksDbContext>> _options = new Lazy<DbContextOptions<TrucksDbContext>>(() =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<TrucksDbContext>();
            optionsBuilder.UseInMemoryDatabase("TrucksDb");
            return optionsBuilder.Options;
        });

        public DbContextOptions<TrucksDbContext> Options => _options.Value;

        public DatabaseFixture()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TrucksDbContext>();
            optionsBuilder.UseInMemoryDatabase("TrucksDb");

            // add sample data to test
            using (var context = new TrucksDbContext(Options))
            {
                context.Truck.Add(new Truck { Id = 1, Title = "Truck 1", Model = TruckModel.FH, ManufacturingYear = 2022, ModelYear = 2023, CreationDate = DateTime.Now, CreatedBy = "Guest" });
                context.Truck.Add(new Truck { Id = 2, Title = "Truck 2", Model = TruckModel.FM, ManufacturingYear = 2022, ModelYear = 2023, CreationDate = DateTime.Now, CreatedBy = "Guest" });
                context.Truck.Add(new Truck { Id = 3, Title = "Truck 3", Model = TruckModel.FM, ManufacturingYear = 2022, ModelYear = 2023, CreationDate = DateTime.Now, CreatedBy = "Guest" });

                context.SaveChanges();
            }
        }

        public void Dispose()
        { }
    }

    public class TrucksControllerTest : IClassFixture<DatabaseFixture>
    {
        DatabaseFixture Fixture;

        public TrucksControllerTest(DatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void Index_ReturnsAViewResult_WithAListOfTrucks()
        {
            using (var context = new TrucksDbContext(Fixture.Options))
            {
                // Arrange
                var controller = new TrucksController(context);
                var totalTrucks = context.Truck.Count();

                // Act
                var result = controller.Index();
                result.Wait();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result.Result);
                var model = Assert.IsAssignableFrom<IEnumerable<Truck>>(viewResult.ViewData.Model);
                Assert.Equal("Index", viewResult.ViewName);
                Assert.Equal(totalTrucks, model.Count());
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void Details_ReturnsANotFoundResult(int? id)
        {
            using (var context = new TrucksDbContext(Fixture.Options))
            {
                // Arrange
                var controller = new TrucksController(context);

                // Act
                var result = controller.Details(id);
                result.Wait();

                // Assert
                Assert.IsType<NotFoundResult>(result.Result);
            }
        }

        [Theory]
        [InlineData(1)]
        public void Details_ReturnsAViewResult_WithValidTruck(int id)
        {
            using (var context = new TrucksDbContext(Fixture.Options))
            {
                // Arrange
                var controller = new TrucksController(context);

                // Act
                var result = controller.Details(id);
                result.Wait();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result.Result);
                var model = Assert.IsAssignableFrom<Truck>(viewResult.ViewData.Model);

                Assert.Equal(id, model.Id);
            }
        }

        [Fact]
        public void Create_ReturnsAViewResult()
        {
            using (var context = new TrucksDbContext(Fixture.Options))
            {
                // Arrange
                var controller = new TrucksController(context);

                // Act
                var result = controller.Create();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Equal("Create", viewResult.ViewName);
            }
        }

        [Fact]
        public async void CreatePost_ReturnsAViewResult_WhenModelStateIsInvalid()
        {
            using (var context = new TrucksDbContext(Fixture.Options))
            {
                // Arrange
                var controller = new TrucksController(context);
                controller.ModelState.AddModelError("fakeError", "fakeError");
                var newTruck = new Truck();

                // Act
                var result = await controller.Create(newTruck);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.False(controller.ModelState.IsValid);
                var model = Assert.IsAssignableFrom<Truck>(viewResult.ViewData.Model);
                Assert.Equal(0, model.Id);
            }
        }

        [Fact]
        public async void CreatePost_ReturnsRedirectToIndex_WhenTruckSuccessfullyCreated()
        {
            using (var context = new TrucksDbContext(Fixture.Options))
            {
                // Arrange
                var controller = new TrucksController(context);
                var newTruck = new Truck() 
                {
                    Title = "New Truck", 
                    ManufacturingYear = DateTime.Now.Year, 
                    ModelYear = DateTime.Now.Year, Model = TruckModel.FH
                };

                // Act
                var result = await controller.Create(newTruck);

                // Assert
                Assert.NotNull(context.Truck.FirstOrDefault(x => x.Title == newTruck.Title));
                Assert.IsType<RedirectToActionResult>(result);
                var actionResult = Assert.IsAssignableFrom<RedirectToActionResult>(result);
                Assert.Equal("Index", actionResult.ActionName);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void Edit_ReturnsANotFoundResult(int? id)
        {
            using (var context = new TrucksDbContext(Fixture.Options))
            {
                // Arrange
                var controller = new TrucksController(context);

                // Act
                var result = controller.Edit(id);
                result.Wait();

                // Assert
                Assert.IsType<NotFoundResult>(result.Result);
            }
        }

        [Theory]
        [InlineData(1)]
        public void Edit_ReturnsAViewResult_WithValidTruck(int id)
        {
            using (var context = new TrucksDbContext(Fixture.Options))
            {
                // Arrange
                var controller = new TrucksController(context);

                // Act
                var result = controller.Edit(id);
                result.Wait();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result.Result);
                var model = Assert.IsAssignableFrom<Truck>(viewResult.ViewData.Model);

                Assert.Equal(id, model.Id);
            }
        }

        [Theory]
        [InlineData(0)]
        public async void EditPost_ReturnsANotFoundResult_WhenIdIsInvalid(int id)
        {
            using (var context = new TrucksDbContext(Fixture.Options))
            {
                // Arrange
                var controller = new TrucksController(context);
                var truckToChange = new Truck() { Id = 1 }; // fake item with specific Id

                // Act
                var result = await controller.Edit(id, truckToChange); // id specified to Edit and item not match

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Theory]
        [InlineData(1)]
        public async void EditPost_ReturnsAViewResult_WhenModelStateIsInvalid(int id)
        {
            using (var context = new TrucksDbContext(Fixture.Options))
            {
                // Arrange
                var controller = new TrucksController(context);
                controller.ModelState.AddModelError("fakeError", "fakeError");
                var truckToChange = context.Truck.FirstOrDefault(x => x.Id == id);

                // Act
                var result = await controller.Edit(id, truckToChange); // id specified to Edit and item not match

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.False(controller.ModelState.IsValid);
                var model = Assert.IsAssignableFrom<Truck>(viewResult.ViewData.Model);
                Assert.Equal(id, model.Id);
            }
        }

        [Theory]
        [InlineData(2)]
        public async void EditPost_ReturnsRedirectToIndex_WhenTruckSuccessfullyChanged(int id)
        {
            using (var context = new TrucksDbContext(Fixture.Options))
            {
                // Arrange
                var controller = new TrucksController(context);
                var truckToChange = new Truck 
                {
                    Id = id,
                    CreatedBy = "Fake User", // example property that cannot be updated
                    Title = "Changed Truck"  // the property to change
                };

                // Act
                var result = await controller.Edit(id, truckToChange);
                var changedTruck = context.Truck.FirstOrDefault(x => x.Id == id);

                // Assert
                Assert.NotNull(changedTruck);
                Assert.IsType<RedirectToActionResult>(result);
                var actionResult = Assert.IsAssignableFrom<RedirectToActionResult>(result);
                Assert.Equal("Index", actionResult.ActionName);
                Assert.Equal(truckToChange.Title, changedTruck.Title);
                Assert.NotEqual(truckToChange.CreatedBy, changedTruck.CreatedBy);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void Delete_ReturnsANotFoundResult(int? id)
        {
            using (var context = new TrucksDbContext(Fixture.Options))
            {
                // Arrange
                var controller = new TrucksController(context);

                // Act
                var result = controller.Delete(id);
                result.Wait();

                // Assert
                Assert.IsType<NotFoundResult>(result.Result);
            }
        }

        [Theory]
        [InlineData(3)]
        public async void DeleteConfirmedPost_ReturnsRedirectToIndex_WhenTruckSuccessfullyDeleted(int id)
        {
            using (var context = new TrucksDbContext(Fixture.Options))
            {
                // Arrange
                var controller = new TrucksController(context);
                var newTruck = context.Truck.FirstOrDefault(x => x.Id == id);

                // Act
                var result = await controller.DeleteConfirmed(id);

                // Assert
                Assert.IsType<RedirectToActionResult>(result);
                var actionResult = Assert.IsAssignableFrom<RedirectToActionResult>(result);
                Assert.Equal("Index", actionResult.ActionName);
                Assert.False(context.Truck.Any(e => e.Id == id));
            }
        }
    }
}
