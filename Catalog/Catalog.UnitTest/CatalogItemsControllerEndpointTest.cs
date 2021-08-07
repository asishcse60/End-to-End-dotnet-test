using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Catalog.Api.Controllers;
using Catalog.Contracts.Models;
using Catalog.Database.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace Catalog.UnitTest
{
    public class CatalogItemsControllerEndpointTest
    {
        private readonly Mock<IItemRepository> _repositoryMock;
        private readonly Mock<ILogger<CatalogItemsController>> _loggerMock;
        private readonly string _urlConnection;
        private readonly Random _rand = new Random();
        public CatalogItemsControllerEndpointTest()
        {
            _repositoryMock = new Mock<IItemRepository>();
            _loggerMock = new Mock<ILogger<CatalogItemsController>>();
            _urlConnection = "http://localhost:5000";
        }
        [Fact]
        public async Task Test_GetItemAsync()
        {
            //Assert
            var item = GetMockItem();
            _repositoryMock.Setup(repo => repo.GetItemAsync(It.IsAny<string>())).ReturnsAsync(item);
            string callUri = _urlConnection + $"/api/CatalogItems/Item?id={item.Id}";

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(callUri).Respond("application/json", "{'id':'21324465','Name':'Coffee'}");
            var httpClient = new HttpClient(mockHttp);

            httpClient.BaseAddress = new Uri(_urlConnection);
            //Act
            var response = await httpClient.GetAsync(callUri);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            //var json = await response.Content.ReadAsStringAsync();

        }
        [Fact]
        public async Task Test_GetItemsAsync()
        {
            //Assert
            var item = GetMockItem();
            _repositoryMock.Setup(repo => repo.GetItemAsync(It.IsAny<string>())).ReturnsAsync(item);
            string callUri = _urlConnection + $"/api/CatalogItems/Items";
            
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(callUri).Respond("application/json", "[{'id':'21324465','Name':'Coffee'},{'id':'76565777','Name':'Tea'}]");
            var httpClient = new HttpClient(mockHttp);

            httpClient.BaseAddress = new Uri(_urlConnection);
            //Act
            var response = await httpClient.GetAsync(callUri);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            //var json = await response.Content.ReadAsStringAsync();

        }
        [Fact]
        public async Task Test_CreateItemAsync()
        {
            //Assert
            var item = GetMockItem();
            _repositoryMock.Setup(repo => repo.GetItemAsync(It.IsAny<string>())).ReturnsAsync(item);
            string callUri = _urlConnection + $"/api/CatalogItems/Create";

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(callUri).Respond("application/json", "{'id':'21324465','Name':'Coffee'}");

            var httpClient = new HttpClient(mockHttp);
            httpClient.BaseAddress = new Uri(_urlConnection);
            var httpRequest = GetHttpMessageRequest(HttpMethod.Post, callUri, item);
            //Act
            var response = await httpClient.SendAsync(httpRequest);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            //var json = await response.Content.ReadAsStringAsync();

        }

        [Fact]
        public async Task Test_UpdateItemAsync()
        {
            //Assert
            var item = GetMockItem();
            _repositoryMock.Setup(repo => repo.GetItemAsync(It.IsAny<string>())).ReturnsAsync(item);
            string callUri = _urlConnection + $"/api/CatalogItems/Update?id={item.Id}";

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(callUri).Respond("application/json", "");
            var httpClient = new HttpClient(mockHttp);

            httpClient.BaseAddress = new Uri(_urlConnection);
            var httpRequest = GetHttpMessageRequest(HttpMethod.Put, callUri, item);
            //Act
            if (httpRequest.Content != null)
            {
                var response = await httpClient.PutAsync(callUri, httpRequest.Content);

                //Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
            else
            {
                Assert.False(false);
            }
            //var json = await response.Content.ReadAsStringAsync();

        }
        [Fact]
        public async Task Test_DeleteItemAsync()
        {
            //Assert
            var item = GetMockItem();
            _repositoryMock.Setup(repo => repo.GetItemAsync(It.IsAny<string>())).ReturnsAsync(item);
            string callUri = _urlConnection + $"/api/CatalogItems/Delete?id={item.Id}";

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(callUri).Respond("application/json", "");
            var httpClient = new HttpClient(mockHttp);

            httpClient.BaseAddress = new Uri(_urlConnection);
            //Act
            var response = await httpClient.DeleteAsync(callUri);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            //var json = await response.Content.ReadAsStringAsync();

        }

        #region Private Method

        private HttpRequestMessage GetHttpMessageRequest(HttpMethod httpMethod, string callUri, object obj)
        {
            var request = new HttpRequestMessage();
            request.Method = httpMethod;
            request.Content = JsonContent.Create(obj);
            request.RequestUri = new Uri(callUri);
            return request;
        }
        private Item GetMockItem()
        {
            var item = new Item
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Coffee",
                Price = _rand.Next(1000),
                Description = "Coffee is very test",
                CreatedDate = DateTimeOffset.UtcNow
            };
            return item;
        }
        #endregion

    }
}
