using CleanArch.Domain.Products;
using CleanArch.Infrastructure.Context;
using CleanArch.Infrastructure.Repositories.Base;

namespace CleanArch.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }
}
