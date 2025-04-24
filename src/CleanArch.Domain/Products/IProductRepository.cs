using CleanArch.Domain.Abstractions;

namespace CleanArch.Domain.Products;

public interface IProductRepository : IGenericRepository<Product>
{
    // Métodos adicionales específicos para productos (si son necesarios)
}
