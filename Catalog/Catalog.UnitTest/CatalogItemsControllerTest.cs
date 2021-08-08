using System;
using System.Collections.Generic;
using System.Net;
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
        //don't test multiple logic in one test
        private readonly Mock<IItemRepository> _repositoryMock;
        private readonly Mock<ILogger<CatalogItemsController>>_loggerMock;
        private readonly Random _rand = new Random();
        private readonly CatalogItemsController _catalogItemsController;
        public CatalogItemsControllerTest()
        {
            _repositoryMock = new Mock<IItemRepository>();
            _loggerMock = new Mock<ILogger<CatalogItemsController>>();
            _catalogItemsController = new CatalogItemsController(_repositoryMock.Object, _loggerMock.Object);
        }
        //Arrange + Act + Assert
        [Fact]
        public async Task GetItemAsync_NotFound()
        {
            //Arrange
            _repositoryMock.Setup(repo => repo.GetItemAsync(It.IsAny<string>())).ReturnsAsync((Item) null);
            //Act
            var result = await _catalogItemsController.GetItemAsync(Guid.NewGuid().ToString());
            var actionResult = result as OkObjectResult;
          
            Assert.NotNull(actionResult);
            var value = (NotFoundResult)actionResult.Value;
            //Assert
            value.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task GetItemAsync_ExistingItemFound()
        {
            var expectedItem = GetItem();
            //Arrange
            _repositoryMock.Setup(repo => repo.GetItemAsync(It.IsAny<string>())).ReturnsAsync(expectedItem);
            //Act
            var result = await _catalogItemsController.GetItemAsync(Guid.NewGuid().ToString());
            var actionResult = result as OkObjectResult;
            //Assert
            Assert.NotNull(actionResult);
            var value = (Item)actionResult.Value;
            value.Should().BeEquivalentTo(expectedItem);
            Assert.Equal(expectedItem.Id,value.Id);//optional
        }
        [Fact]
        public async Task GetItemAsync_OkStatus()
        {
            var expectedItem = GetItem();
            //Arrange
            _repositoryMock.Setup(repo => repo.GetItemAsync(It.IsAny<string>())).ReturnsAsync(expectedItem);

            //Act
            var result = await _catalogItemsController.GetItemAsync(Guid.NewGuid().ToString());
            var actionResult = result as OkObjectResult;

            //Assert
            Assert.NotNull(actionResult);
            Assert.Equal((int)HttpStatusCode.OK, actionResult.StatusCode);//optional
        }
        [Fact]
        public async Task GetItemsAsync_ExistingAllItemsFound()
        {
            var expectedItems = new List<Item>{ GetItem(),GetItem(),GetItem() };
            //Arrange
            _repositoryMock.Setup(repo=>repo.GetItemsAsync()).ReturnsAsync(expectedItems);

            //Act
            var result = await _catalogItemsController.GetItemsAsync();
            var actionResult = result as OkObjectResult;

            //Assert
            Assert.NotNull(actionResult);
            actionResult.Value.Should().BeEquivalentTo(expectedItems);
        }
        [Fact]
        public async Task GetItemsAsync_OkStatus()
        {
            var expectedItems = new List<Item> { GetItem(), GetItem(), GetItem() };
            //Arrange
            _repositoryMock.Setup(repo => repo.GetItemsAsync()).ReturnsAsync(expectedItems);

            //Act
            var result = await _catalogItemsController.GetItemsAsync();
            var actionResult = result as OkObjectResult;

            //Assert
            Assert.NotNull(actionResult);
            Assert.Equal((int)HttpStatusCode.OK, actionResult.StatusCode);//optional
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
            
            //Act
            var result = await _catalogItemsController.GetItemsAsync(nameToMatch);
            var actionResult = result as OkObjectResult;

            //Assert
            Assert.NotNull(actionResult);
            var value = (List<Item>)actionResult.Value;
            value.Should().OnlyContain(im=>im.Name == expectedItems[0].Name ||  im.Name == expectedItems[2].Name);
        }

        [Fact]
        public async Task CreateItemTestAsync()
        {
            //Arrange
            var item = GetItem();
            //Act
            var result = await _catalogItemsController.CreateItemAsync(item);
            var actionResult = result as OkObjectResult;
            Assert.NotNull(actionResult);

            //Assert
            var value = (HttpStatusCode)actionResult.Value;
            value.Should().BeEquivalentTo(HttpStatusCode.Created);
            //var createdItem = (result.Result as CreatedAtActionResult)?.Value as Item;
          //  item.Should().BeEquivalentTo(createdItem, options=>options.ComparingByMembers<Item>().ExcludingMissingMembers());
          //  createdItem?.Id.Should().NotBeEmpty();
          //  createdItem?.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
        }
        [Fact]
        public async Task UpdateAsync_OkStatus()
        {
            //Arrange
            var item = GetItem();
            _repositoryMock.Setup(repo => repo.GetItemAsync(It.IsAny<string>())).ReturnsAsync(item);
            var itemId = item.Id;
            var itemToUpdate = GetItem();

            //Act
            var result = await _catalogItemsController.UpdateItemAsync(itemId, itemToUpdate);
            var actionResult = result as OkObjectResult;

            //Assert
            Assert.NotNull(actionResult);
            Assert.Equal((int)HttpStatusCode.OK, actionResult.StatusCode);//optional
        }
        [Fact]
        public async Task UpdateItemTestAsync()
        {
            //Arrange
            var item = GetItem();
            _repositoryMock.Setup(repo => repo.GetItemAsync(It.IsAny<string>())).ReturnsAsync(item);
            var itemId = item.Id;
            var itemToUpdate = GetItem();

            //Act
            var result = await _catalogItemsController.UpdateItemAsync(itemId, itemToUpdate);
            var actionResult = result as OkObjectResult;

            //Assert
            Assert.NotNull(actionResult);
            var value = (NoContentResult)actionResult.Value;
            value.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteItemTestAsync()
        {
            //Arrange
            var item = GetItem();
            _repositoryMock.Setup(repo => repo.GetItemAsync(It.IsAny<string>())).ReturnsAsync(item);
            //Act
            var result = await _catalogItemsController.DeleteItemAsync(item.Id);
            var actionResult = result as OkObjectResult;
            Assert.NotNull(actionResult);
            var value = (NoContentResult)actionResult.Value;
            //Assert
            value.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteItemNotFoundTestAsync()
        {
            //Arrange
            var item = GetItem();
            _repositoryMock.Setup(repo => repo.GetItemAsync(It.IsAny<string>())).ReturnsAsync((Item) null);
            //Act
            var result = await _catalogItemsController.DeleteItemAsync(item.Id);
            var actionResult = result as OkObjectResult;
            Assert.NotNull(actionResult);
            var value = (NotFoundResult)actionResult.Value;
            //Assert
            value.Should().BeOfType<NotFoundResult>();
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
