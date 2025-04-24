using CleanArch.Domain.Products;
using CleanArch.Infrastructure.Context;
using CleanArch.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests
{
    public class ProductRepositoryTests
    {
        private readonly AppDbContext _dbContext;
        private readonly IProductRepository _repository;

        public ProductRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new AppDbContext(options);
            _repository = new ProductRepository(_dbContext);
        }

        [Fact]
        public async Task AddProduct_Success()
        {
            var product = new Product { Name = "Test Product", Price = 100, Stock = 5 };
            await _repository.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(product.Id);

            Assert.NotNull(result);
            Assert.Equal("Test Product", result.Name);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsList()
        {
            _dbContext.Product.Add(new Product { Name = "Product 1", Price = 50, Stock = 10 });
            _dbContext.Product.Add(new Product { Name = "Product 2", Price = 150, Stock = 20 });
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetAllAsync();

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count());
        }
    }
}