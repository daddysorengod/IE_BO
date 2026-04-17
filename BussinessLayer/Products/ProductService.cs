using BussinessLayer.Products.Models;
using BussinessLayer.Products.Domain;
using BussinessLayer.Products.Repositories;
using Microsoft.Extensions.Logging;

namespace BussinessLayer.Products;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IProductImageRepository _productImageRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        IProductImageRepository productImageRepository,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _productImageRepository = productImageRepository;
        _logger = logger;
    }

    public async Task<GetProductByIdResponse?> GetByIdAsync(long productId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Id must be greater than 0.");
            }

            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product is null || product.Delt == "Y")
            {
                return null;
            }

            var images = await _productImageRepository.GetByProductIdAsync(productId, cancellationToken);

            return new GetProductByIdResponse
            {
                Id = product.Id,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                HsCode = product.HsCode,
                UnitOfMeasure = product.UnitOfMeasure,
                ListImage = images.Select(x => new ProductImageResponseItem
                {
                    ProductId = x.ProductId,
                    ImageUrl = x.ImageUrl,
                    IsDefault = x.IsDefault,
                    SortOrder = x.SortOrder
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByIdAsync failed for product id: {Id}", productId);
            throw;
        }
    }

    public async Task<CreateProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateRequest(request);

            var dto = new Product
            {
                ProductCode = request.ProductCode.Trim(),
                ProductName = request.ProductName.Trim(),
                HsCode = string.IsNullOrWhiteSpace(request.HsCode) ? null : request.HsCode.Trim(),
                UnitOfMeasure = request.UnitOfMeasure.Trim()
            };

            var productId = await _productRepository.InsertAsync(dto, cancellationToken);

            if (request.Images.Count > 0)
            {
                foreach (var image in request.Images)
                {
                    ValidateImage(image);
                    await _productImageRepository.InsertAsync(new ProductImage
                    {
                        Id = productId,
                        ImageUrl = image.ImageUrl.Trim(),
                        IsDefault = image.IsDefault,
                        SortOrder = image.SortOrder
                    }, cancellationToken);
                }
            }

            return new CreateProductResponse { Id = productId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateAsync failed for product code: {ProductCode}", request.ProductCode);
            throw;
        }
    }

    public async Task<UpdateProductResponse> UpdateAsync(UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateUpdateRequest(request);

            var dto = new Product
            {
                Id = request.Id,
                ProductCode = request.ProductCode.Trim(),
                ProductName = request.ProductName.Trim(),
                HsCode = string.IsNullOrWhiteSpace(request.HsCode) ? null : request.HsCode.Trim(),
                UnitOfMeasure = request.UnitOfMeasure.Trim()
            };

            var updated = await _productRepository.UpdateAsync(dto, cancellationToken);

            if (updated && request.Images.Count > 0)
            {
                foreach (var image in request.Images)
                {
                    ValidateImage(image);
                    if (image.ProductId > 0)
                    {
                        await _productImageRepository.UpdateAsync(new ProductImage
                        {
                            ProductId = image.ProductId,
                            Id = request.Id,
                            ImageUrl = image.ImageUrl.Trim(),
                            IsDefault = image.IsDefault,
                            SortOrder = image.SortOrder
                        }, cancellationToken);
                    }
                    else
                    {
                        await _productImageRepository.InsertAsync(new ProductImage
                        {
                            Id = request.Id,
                            ImageUrl = image.ImageUrl.Trim(),
                            IsDefault = image.IsDefault,
                            SortOrder = image.SortOrder
                        }, cancellationToken);
                    }
                }
            }

            return new UpdateProductResponse
            {
                Id = request.Id,
                IsUpdated = updated
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateAsync failed for product id: {Id}", request.Id);
            throw;
        }
    }

    public async Task<DeleteProductResponse> DeleteAsync(long productId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Id must be greater than 0.");
            }

            var deleted = await _productRepository.DeleteAsync(productId, cancellationToken);
            return new DeleteProductResponse
            {
                Id = productId,
                IsDeleted = deleted
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteAsync failed for product id: {Id}", productId);
            throw;
        }
    }

    public async Task<SearchProductResponse> SearchAsync(SearchProductRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateSearchRequest(request);

            var offset = (request.Page - 1) * request.PageSize;
            var condition = new ProductSearchCondition
            {
                Keyword = request.Keyword,
                ProductCode = request.ProductCode,
                ProductName = request.ProductName,
                HsCode = request.HsCode,
                UnitOfMeasure = request.UnitOfMeasure,
                Offset = offset,
                Limit = request.PageSize
            };

            var result = await _productRepository.SearchAsync(condition, cancellationToken);
            var items = result.Items.Select(x => new SearchProductItem
            {
                Id = x.Id,
                ProductCode = x.ProductCode,
                ProductName = x.ProductName,
                HsCode = x.HsCode,
                UnitOfMeasure = x.UnitOfMeasure,
                ImageUrl = x.ImageUrl
            }).ToList();

            var totalPages = result.Total == 0 ? 0 : (int)Math.Ceiling(result.Total / (double)request.PageSize);
            return new SearchProductResponse
            {
                Items = items,
                Total = result.Total,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SearchAsync failed.");
            throw;
        }
    }

    private static void ValidateRequest(CreateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProductCode))
        {
            throw new ArgumentException("ProductCode is required.");
        }

        if (string.IsNullOrWhiteSpace(request.ProductName))
        {
            throw new ArgumentException("ProductName is required.");
        }

        if (string.IsNullOrWhiteSpace(request.UnitOfMeasure))
        {
            throw new ArgumentException("UnitOfMeasure is required.");
        }
    }

    private static void ValidateUpdateRequest(UpdateProductRequest request)
    {
        if (request.Id <= 0)
        {
            throw new ArgumentException("Id must be greater than 0.");
        }

        if (string.IsNullOrWhiteSpace(request.ProductCode))
        {
            throw new ArgumentException("ProductCode is required.");
        }

        if (string.IsNullOrWhiteSpace(request.ProductName))
        {
            throw new ArgumentException("ProductName is required.");
        }

        if (string.IsNullOrWhiteSpace(request.UnitOfMeasure))
        {
            throw new ArgumentException("UnitOfMeasure is required.");
        }
    }

    private static void ValidateImage(ProductImageUpsertRequest image)
    {
        if (string.IsNullOrWhiteSpace(image.ImageUrl))
        {
            throw new ArgumentException("ImageUrl is required.");
        }
    }

    private static void ValidateSearchRequest(SearchProductRequest request)
    {
        if (request.Page <= 0)
        {
            throw new ArgumentException("Page must be greater than 0.");
        }

        if (request.PageSize <= 0)
        {
            throw new ArgumentException("PageSize must be greater than 0.");
        }

        if (request.PageSize > 200)
        {
            throw new ArgumentException("PageSize must be less than or equal to 200.");
        }
    }
}
