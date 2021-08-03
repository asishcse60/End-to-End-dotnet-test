using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Api.Controllers;
using Catalog.Contracts.Models;
using Catalog.Database.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Catalog.UnitTest
{
    public class CatalogItemsControllerTest
    {
        private readonly Mock<IItemRepository> _repositoryMock;
        private readonly Mock<ILogger<CatalogItemsController>>_loggerMock;
        private readonly Random _rand = new Random();
        public CatalogItemsControllerTest()
        {
            _repositoryMock = new Mock<IItemRepository>();
            _loggerMock = new Mock<ILogger<CatalogItemsController>>();
        }
        //Arrange + Act + Assert
        [Fact]
        public async Task GetItemAsync_NotFound()
        {
            //Arrange
            _repositoryMock.Setup(repo => repo.GetItemAsync(It.IsAny<string>())).ReturnsAsync((Item) null);
            var controller = new CatalogItemsController(_repositoryMock.Object, _loggerMock.Object);
            //Act
            var result = await controller.GetItemAsync(Guid.NewGuid().ToString());
            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
            Assert.Null(result.Value);//optional
        }
        [Fact]
        public async Task GetItemAsync_ExistingItemFound()
        {
            var expectedItem = GetItem();
            //Arrange
            _repositoryMock.Setup(repo => repo.GetItemAsync(It.IsAny<string>())).ReturnsAsync(expectedItem);
            var controller = new CatalogItemsController(_repositoryMock.Object, _loggerMock.Object);
            //Act
            var result = await controller.GetItemAsync(Guid.NewGuid().ToString());
            //Assert
            result.Value.Should().BeEquivalentTo(expectedItem);
            Assert.Equal(expectedItem.Id,result.Value.Id);//optional
        }
        [Fact]
        public async Task GetItemsAsync_ExistingAllItemsFound()
        {
            var expectedItems = new List<Item>{ GetItem(),GetItem(),GetItem() };
            //Arrange
            _repositoryMock.Setup(repo=>repo.GetItemsAsync()).ReturnsAsync(expectedItems);
            var controller = new CatalogItemsController(_repositoryMock.Object, _loggerMock.Object);
            //Act
            var result = await controller.GetItemsAsync();
            //Assert
            result.Should().BeEquivalentTo(expectedItems);
            Assert.Equal(3, result.Count);//optional
        }
        [Fact]
        public async Task GetItemsAsync_ExistingAllMatchingItemsFound()
        {
            //Arrange
            var expectedItems = new List<Item>
            {
                new Item() {Name = "Potion"}, new Item() {Name = "Antidote"}, new Item() {Name = "Hi-Potion"}
            };
            var nameToMatch = "Potion";
            _repositoryMock.Setup(repo => repo.GetItemsAsync()).ReturnsAsync(expectedItems);
            var controller = new CatalogItemsController(_repositoryMock.Object, _loggerMock.Object);
            
            //Act
            var result = await controller.GetItemsAsync(nameToMatch);
            
            //Assert
            result.Should().OnlyContain(im=>im.Name == expectedItems[0].Name ||  im.Name == expectedItems[2].Name);
        }

        [Fact]
        public async Task CreateItemTestAsync()
        {
            //Arrange
            var item = GetItem();
            var controller = new CatalogItemsController(_repositoryMock.Object, _loggerMock.Object);
            //Act
            var result = await controller.CreateItemAsync(item);

            //Assert
            var createdItem = (result.Result as CreatedAtActionResult)?.Value as Item;
            item.Should().BeEquivalentTo(createdItem, options=>options.ComparingByMembers<Item>().ExcludingMissingMembers());
            createdItem?.Id.Should().NotBeEmpty();
            createdItem?.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
        }
        private Item GetItem()
        {
            return new Item()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Price = _rand.Next(1000),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
