using BussinessLayer.Products.Domain;

namespace BussinessLayer.Products.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(long productId, CancellationToken cancellationToken = default);
    Task<long> InsertAsync(Product product, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long productId, CancellationToken cancellationToken = default);
    Task<ProductSearchResult> SearchAsync(ProductSearchCondition condition, CancellationToken cancellationToken = default);
}
