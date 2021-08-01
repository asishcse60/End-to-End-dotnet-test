using System;
using Catalog.Api.Controllers;
using Catalog.Database.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Catalog.UnitTest
{
    public class CatalogItemsControllerTest
    {
        private readonly Mock<IItemRepository> _repositoryMock;
        private readonly Mock<ILogger<CatalogItemsController>>_loggerMock;

        public CatalogItemsControllerTest()
        {
            _repositoryMock = new Mock<IItemRepository>();
            _loggerMock = new Mock<ILogger<CatalogItemsController>>();
        }
        //Arrange + Act + Assert
        [Fact]
        public void Test1()
        {

        }
    }
}
