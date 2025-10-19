using System;
using System.IO;
using System.Linq;
using Xunit;
using DataServiceLayer;
using Microsoft.Extensions.Configuration;

namespace Assignment4TestSuite
{
    public class DataServiceTests
    {
        private readonly DataService _service;

        string? FindWebServiceLayer()
        {
            var candidates = new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory };
            foreach (var start in candidates)
            {
                var dir = new DirectoryInfo(start);
                while (dir != null)
                {
                    var maybe = Path.Combine(dir.FullName, "WebServiceLayer");
                    if (Directory.Exists(maybe)) return maybe;
                    dir = dir.Parent;
                }
            }
            return null;
        }

        public DataServiceTests()
        {
            var basePath = FindWebServiceLayer();
            if (basePath == null)
            {
                throw new InvalidOperationException("Could not locate WebServiceLayer/config.json. Expected to find WebServiceLayer folder in repo parents.");
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = config.GetConnectionString("ConnectionString");

            _service = new DataService(connectionString);
        }

        /* Categories */

        [Fact]
        public void Category_Object_HasIdNameAndDescription()
        {
            var category = new Category();
            Assert.Equal(0, category.Id);
            Assert.Null(category.Name);
            Assert.Null(category.Description);
        }

        [Fact]
        public void GetAllCategories_NoArgument_ReturnsAllCategories()
        {
            var categories = _service.GetCategories();
            Assert.Equal(8, categories.Count);
            Assert.Equal("Beverages", categories.First().Name);
        }

        [Fact]
        public void GetCategory_ValidId_ReturnsCategoryObject()
        {
            var category = _service.GetCategory(1);
            Assert.Equal("Beverages", category.Name);
        }

        [Fact]
        public void CreateCategory_ValidData_CreteCategoryAndRetunsNewObject()
        {
            var category = _service.CreateCategory("Test", "CreateCategory_ValidData_CreteCategoryAndRetunsNewObject");
            Assert.True(category.Id > 0);
            Assert.Equal("Test", category.Name);
            Assert.Equal("CreateCategory_ValidData_CreteCategoryAndRetunsNewObject", category.Description);

            // cleanup
            _service.DeleteCategory(category.Id);
        }

        [Fact]
        public void DeleteCategory_ValidId_RemoveTheCategory()
        {
            var category = _service.CreateCategory("Test", "DeleteCategory_ValidId_RemoveTheCategory");
            var result = _service.DeleteCategory(category.Id);
            Assert.True(result);
            category = _service.GetCategory(category.Id);
            Assert.Null(category);
        }

        [Fact]
        public void DeleteCategory_InvalidId_ReturnsFalse()
        {
            var result = _service.DeleteCategory(-1);
            Assert.False(result);
        }

        [Fact]
        public void UpdateCategory_NewNameAndDescription_UpdateWithNewValues()
        {
            var category = _service.CreateCategory("TestingUpdate", "UpdateCategory_NewNameAndDescription_UpdateWithNewValues");

            var result = _service.UpdateCategory(category.Id, "UpdatedName", "UpdatedDescription");
            Assert.True(result);

            category = _service.GetCategory(category.Id);
            Assert.Equal("UpdatedName", category.Name);
            Assert.Equal("UpdatedDescription", category.Description);

            // cleanup
            _service.DeleteCategory(category.Id);
        }

        [Fact]
        public void UpdateCategory_InvalidID_ReturnsFalse()
        {
            var result = _service.UpdateCategory(-1, "UpdatedName", "UpdatedDescription");
            Assert.False(result);
        }

         /* products */

        [Fact]
        public void Product_Object_HasIdNameUnitPriceQuantityPerUnitAndUnitsInStock()
        {
            var product = new Product();
            Assert.Equal(0, product.Id);
            Assert.Null(product.Name);
            Assert.Equal(0.0, product.UnitPrice);
            Assert.Null(product.QuantityPerUnit);
            Assert.Equal(0, product.UnitsInStock);
        }

        [Fact]
        public void GetProduct_ValidId_ReturnsProductWithCategory()
        {
            var product = _service.GetProduct(1);
            Assert.Equal("Chai", product.Name);
            Assert.Equal("Beverages", product.Category.Name);
        }

        [Fact]
        public void GetProductsByCategory_ValidId_ReturnsProductWithCategory()
        {
            var products = _service.GetProductsByCategory(1);
            Assert.Equal(12, products.Count);
            Assert.Equal("Chai", products.First().Name);
            Assert.Equal("Beverages", products.First().Category.Name);
            Assert.Equal("Lakkalikööri", products.Last().Name);
        }

        [Fact]
        public void GetProduct_NameSubString_ReturnsProductsThatMachesTheSubString()
        {
            var products = _service.GetProductsByName("em");
            Assert.Equal(4, products.Count);
            Assert.Equal("NuNuCa Nuß-Nougat-Creme", products.First().ProductName);
            Assert.Equal("Flotemysost", products.Last().ProductName);
        }

        /* orders */

        [Fact]
        public void Order_Object_HasIdDatesAndOrderDetails()
        {
            var order = new Order();
            Assert.Equal(0, order.Id);
            Assert.Equal(new DateTime(), order.Date);
            Assert.Equal(new DateTime(), order.Required);
            Assert.Null(order.OrderDetails);
            Assert.Null(order.ShipName);
            Assert.Null(order.ShipCity);
        }

        [Fact]
        public void GetOrder_ValidId_ReturnsCompleteOrder()
        {
            var order = _service.GetOrder(10248);
            Assert.Equal(3, order.OrderDetails.Count);
            Assert.Equal("Queso Cabrales", order.OrderDetails.First().Product.Name);
            Assert.Equal("Dairy Products", order.OrderDetails.First().Product.Category.Name);
        }

        [Fact]
        public void GetOrders()
        {
            var orders = _service.GetAllOrders();
            Assert.Equal(830, orders.Count);
        }

        /* orderdetails */
        [Fact]
        public void OrderDetails_Object_HasOrderProductUnitPriceQuantityAndDiscount()
        {
            var orderDetails = new OrderDetail();
            Assert.Equal(0, orderDetails.OrderId);
            Assert.Null(orderDetails.Order);
            Assert.Equal(0, orderDetails.ProductId);
            Assert.Null(orderDetails.Product);
            Assert.Equal(0.0, orderDetails.UnitPrice);
            Assert.Equal(0.0, orderDetails.Quantity);
            Assert.Equal(0.0, orderDetails.Discount);
        }

        [Fact]
        public void GetOrderDetailByOrderId_ValidId_ReturnsProductNameUnitPriceAndQuantity()
        {
            var orderDetails = _service.GetOrderDetailsByOrderId(10248);
            Assert.Equal(3, orderDetails.Count);
            Assert.Equal("Queso Cabrales", orderDetails.First().Product.Name);
            Assert.Equal(14, orderDetails.First().UnitPrice);
            Assert.Equal(12, orderDetails.First().Quantity);
        }

        [Fact]
        public void GetOrderDetailByProductId_ValidId_ReturnsOrderDateUnitPriceAndQuantity()
        {
            var orderDetails = _service.GetOrderDetailsByProductId(11);
            Assert.Equal(38, orderDetails.Count);
            Assert.Equal("1997-05-06", orderDetails.First().Order.Date.ToString("yyyy-MM-dd"));
            Assert.Equal(21, orderDetails.First().UnitPrice);
            Assert.Equal(3, orderDetails.First().Quantity);
        }
    }
   
}