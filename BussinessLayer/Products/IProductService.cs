using BussinessLayer.Products.Models;

namespace BussinessLayer.Products;

public interface IProductService
{
    Task<GetProductByIdResponse?> GetByIdAsync(long productId, CancellationToken cancellationToken = default);
    Task<CreateProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<UpdateProductResponse> UpdateAsync(UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task<DeleteProductResponse> DeleteAsync(long productId, CancellationToken cancellationToken = default);
    Task<SearchProductResponse> SearchAsync(SearchProductRequest request, CancellationToken cancellationToken = default);
}
