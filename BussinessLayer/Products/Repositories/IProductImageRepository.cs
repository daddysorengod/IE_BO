using BussinessLayer.Products.Domain;

namespace BussinessLayer.Products.Repositories;

public interface IProductImageRepository
{
    Task<ProductImage?> GetByIdAsync(long productImageId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductImage>> GetByProductIdAsync(long productId, CancellationToken cancellationToken = default);
    Task<long> InsertAsync(ProductImage productImage, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(ProductImage productImage, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long productImageId, CancellationToken cancellationToken = default);
}
