using System.Data;
using AutoMapper;
using CleanArch.Application.Products;
using CleanArch.Domain.Abstractions;
using CleanArch.Domain.Products;
using CleanArch.Infrastructure.Context;
using CleanArch.Infrastructure.Messaging.Contract;
using CleanArch.Infrastructure.Messaging.Publisher;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Application.Tests
{
    public class ProductServiceTests
    {
        private readonly IUnitOfWork<AppDbContext> _mockUnitOfWork;
        private readonly IMapper _mockMapper;
        private readonly IProductRepository _mockProductRepository;
        private readonly ILogger<ProductService> _mockLogger;
        private readonly IEventPublisher _mockEventPublisher;
        private readonly IProductService _productService;

        public ProductServiceTests()
        {
            _mockUnitOfWork = Substitute.For<IUnitOfWork<AppDbContext>>();
            _mockMapper = Substitute.For<IMapper>();
            _mockProductRepository = Substitute.For<IProductRepository>();
            _mockLogger = Substitute.For<ILogger<ProductService>>();
            _mockEventPublisher = Substitute.For<IEventPublisher>();
            _productService = new ProductService(_mockUnitOfWork, _mockMapper, _mockProductRepository, _mockLogger, _mockEventPublisher);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsEmptyList()
        {
            _mockProductRepository.GetAllAsync().Returns(new List<Product>());

            var result = await _productService.GetAllProducts();

            Assert.True(result.IsSuccess);
            Assert.Equal(ProductMessages.NoProductsFound, result.Message);
        }

        [Fact]
        public async Task GetProductById_ReturnsProduct()
        {
            var product = new Product { Id = 1, Name = "Test Product", Price = 100, Stock = 5 };
            var productDto = new ProductDto { Id = 1, Name = "Test Product", Price = 100, Stock = 5 };

            _mockProductRepository.GetByIdAsync(1).Returns(product);
            _mockMapper.Map<ProductDto>(product).Returns(productDto);

            var result = await _productService.GetProductById(1);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(productDto.Name, result.Data?.Name);
        }

        [Fact]
        public async Task CreateProduct_ReturnsSuccess()
        {
            var productDto = new ProductDto { Name = "Test Product", Price = 100, Stock = 5 };
            var product = new Product { Id = 1, Name = "Test Product", Price = 100, Stock = 5 };

            var mockTransaction = Substitute.For<IDbContextTransaction>();

            _mockUnitOfWork.BeginTransactionAsync().Returns(Task.FromResult(mockTransaction));
            _mockMapper.Map<Product>(productDto).Returns(product);
            _mockProductRepository.AddAsync(product).Returns(Task.CompletedTask);
            _mockUnitOfWork.SaveAsync().Returns(1);
            _mockMapper.Map<ProductDto>(product).Returns(ci => {
                productDto.Id = 1;
                return productDto;
            });

            var result = await _productService.CreateProduct(productDto);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Id);
            Assert.Equal(ProductMessages.ProductCreated, result.Message);

            await _mockUnitOfWork.Received(1).BeginTransactionAsync();
            await mockTransaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());

            await _mockEventPublisher.Received(1).Publish(Arg.Any<LogCreatedEvent>());
        }
    }
}