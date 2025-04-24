namespace CleanArch.Domain.Products;

public static class ProductMessages
{
    public const string NoProductsFound = "No se encontraron productos.";
    public const string ProductNotFound = "El producto no existe.";
    public const string ProductCreated = "Producto creado exitosamente.";
    public const string ProductUpdated = "Producto actualizado exitosamente.";
    public const string ProductDeleted = "Producto eliminado exitosamente.";
    public const string ProductNoMatch = "Error, el id no coincide.";
    public const string ProductCreatedError = "Error al crear el producto.";
    public const string ProductUpdatedError = "Error al actualizar el producto con ID {0}";
    public const string ProductDeletedError = "Error al eliminar el producto con ID {0}";
}
