using AutoMapper;
using CleanArch.Domain.Abstractions;
using CleanArch.Domain.Products;
using CleanArch.Domain.Shared;
using CleanArch.Infrastructure.Context;
using CleanArch.Infrastructure.Messaging.Contract;
using CleanArch.Infrastructure.Messaging.Publisher;
using Microsoft.Extensions.Logging;


namespace CleanArch.Application.Products;

public class ProductService : IProductService
{
    private readonly IUnitOfWork<AppDbContext> _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductService> _logger;
    private readonly IEventPublisher _eventPublisher;

    public ProductService(IUnitOfWork<AppDbContext> unitOfWork, IMapper mapper, IProductRepository productRepository, ILogger<ProductService> logger, IEventPublisher eventPublisher)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _productRepository = productRepository;
        _logger = logger;
        _eventPublisher = eventPublisher;
    }

    public async Task<Response<IEnumerable<ProductDto>>> GetAllProducts()
    {
        var products = await _productRepository.GetAllAsync();
        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

        if (!productDtos.Any())
            return Response<IEnumerable<ProductDto>>.Success(productDtos, ProductMessages.NoProductsFound);

        return Response<IEnumerable<ProductDto>>.Success(productDtos);
    }

    public async Task<Response<ProductDto>> GetProductById(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product == null
            ? Response<ProductDto>.Failure(ProductMessages.ProductNotFound)
            : Response<ProductDto>.Success(_mapper.Map<ProductDto>(product));
    }

    public async Task<Response<ProductDto>> CreateProduct(ProductDto dto)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var product = _mapper.Map<Product>(dto);
            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            await _eventPublisher.Publish(new LogCreatedEvent
            {
                MachineName = "TEST",
                Logged = DateTime.UtcNow,
                Level = "INFORMATION",
                Message = ProductMessages.ProductCreated,
                Logger = "jvaldez",
                Properties = $"Product Created with id {product.Id}",
                Callsite = nameof(ProductService),
                Exception = null,
                ApplicationId = 1
            });

            return Response<ProductDto>.Success(_mapper.Map<ProductDto>(product), ProductMessages.ProductCreated);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(string.Format(Messages.UnexpectedError, ex.Message), ProductMessages.ProductCreatedError);
            return Response<ProductDto>.Failure(string.Format(Messages.UnexpectedError, ex.Message));
        }
    }

    public async Task<Response<ProductDto>> UpdateProduct(int id, ProductDto dto)
    {
        if (id != dto.Id)
            return Response<ProductDto>.Failure(ProductMessages.ProductNoMatch);

        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var product = await _productRepository.GetByIdAsync(dto.Id);
            if (product == null)
                return Response<ProductDto>.Failure(ProductMessages.ProductNotFound);

            _mapper.Map(dto, product);

            _productRepository.Update(product);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            return Response<ProductDto>.Success(_mapper.Map<ProductDto>(product), ProductMessages.ProductUpdated);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(string.Format(Messages.UnexpectedError, ex.Message), string.Format(ProductMessages.ProductUpdatedError, id));
            return Response<ProductDto>.Failure(string.Format(Messages.UnexpectedError, ex.Message));
        }
    }

    public async Task<Response<bool>> DeleteProduct(int id)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return Response<bool>.Failure(ProductMessages.ProductNotFound);

            _productRepository.Delete(product);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            return Response<bool>.Success(true, ProductMessages.ProductDeleted);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(string.Format(Messages.UnexpectedError, ex.Message), string.Format(ProductMessages.ProductDeletedError, id));
            return Response<bool>.Failure(string.Format(Messages.UnexpectedError, ex.Message));
        }
    }
}