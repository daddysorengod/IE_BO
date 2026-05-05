using Application.Contracts.Models;
using Application.Contracts.Repositories;
using Application.Packages.Repositories;
using Application.Partners.Repositories;
using Application.Products.Repositories;
using Domain.Contracts;
using Microsoft.Extensions.Logging;

namespace Application.Contracts;

public class ContractService : IContractService
{
    private static readonly HashSet<string> AllowedContractTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ContractTypes.Export,
        ContractTypes.Import
    };

    private readonly IContractRepository _contractRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IPackageRepository _packageRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ContractService> _logger;

    public ContractService(
        IContractRepository contractRepository,
        IPartnerRepository partnerRepository,
        IPackageRepository packageRepository,
        IProductRepository productRepository,
        ILogger<ContractService> logger)
    {
        _contractRepository = contractRepository;
        _partnerRepository = partnerRepository;
        _packageRepository = packageRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<CreateContractResponse> CreateAsync(CreateContractRequest request, long currentUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateCreateExport(request);
            ValidateId(currentUserId, "Current user id must be greater than 0.");

            var contractNo = request.ContractNo.Trim();
            var existing = await _contractRepository.GetByContractNoAsync(contractNo, cancellationToken);
            if (existing is not null)
            {
                throw new InvalidOperationException("ContractNo already exists.");
            }

            var buyer = await _partnerRepository.GetByIdAsync(request.BuyerId, cancellationToken);
            if (buyer is null)
            {
                throw new KeyNotFoundException("Buyer partner not found.");
            }

            if (!string.Equals(buyer.PartnerType, "CUSTOMER", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Buyer partner must have PartnerType CUSTOMER.");
            }

            var importContractNo = request.ImportContractNo.Trim();
            var importContract = await _contractRepository.GetByContractNoAsync(importContractNo, cancellationToken);
            if (importContract is null)
            {
                throw new KeyNotFoundException("Import contract not found.");
            }

            if (!string.Equals(importContract.ContractType, ContractTypes.Import, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Linked contract must be IMPORT contract.");
            }

            var now = DateTime.Now;
            var contract = new Contract
            {
                ContractNo = contractNo,
                ContractDate = request.ContractDate,
                ContractType = ContractTypes.Export,
                BuyerId = request.BuyerId,
                ImportContractId = importContract.Id,
                Incoterm = TrimToNull(request.Incoterm),
                PaymentTerms = TrimToNull(request.PaymentTerms),
                ShipmentDatePlan = TrimToNull(request.ShipmentDatePlan),
                Note = TrimToNull(request.Note),
                CreatedBy = currentUserId,
                CreatedDate = now
            };

            var attachments = request.Attachments
                .Select(attachmentRequest => BuildContractAttachment(0, attachmentRequest, currentUserId, now))
                .ToList();

            var packages = await BuildExportContractPackagesAsync(request.Packages, currentUserId, now, cancellationToken);

            var contractId = await _contractRepository.InsertExportAggregateAsync(contract, attachments, packages, cancellationToken);
            return new CreateContractResponse { Id = contractId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create export contract failed for contract no: {ContractNo}", request.ContractNo);
            throw;
        }
    }

    public async Task<CreateContractResponse> CreateImportAsync(CreateImportContractRequest request, long currentUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateCreateImport(request);
            ValidateId(currentUserId, "Current user id must be greater than 0.");

            var contractNo = request.ContractNo.Trim();
            var existing = await _contractRepository.GetByContractNoAsync(contractNo, cancellationToken);
            if (existing is not null)
            {
                throw new InvalidOperationException("ContractNo already exists.");
            }

            var supplier = await _partnerRepository.GetByIdAsync(request.SupplierId, cancellationToken);
            if (supplier is null)
            {
                throw new KeyNotFoundException("Supplier partner not found.");
            }

            if (!string.Equals(supplier.PartnerType, "SUPPLIER", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Supplier partner must have PartnerType SUPPLIER.");
            }

            var now = DateTime.Now;
            var contract = new Contract
            {
                ContractNo = contractNo,
                ContractDate = request.ContractDate,
                ContractType = ContractTypes.Import,
                SupplierId = request.SupplierId,
                Incoterm = TrimToNull(request.Incoterm),
                PaymentTerms = TrimToNull(request.PaymentTerms),
                ShipmentDatePlan = TrimToNull(request.ShipmentDatePlan),
                Note = TrimToNull(request.Note),
                CreatedBy = currentUserId,
                CreatedDate = now
            };

            var attachments = request.Attachments
                .Select(attachmentRequest => BuildContractAttachment(0, attachmentRequest, currentUserId, now))
                .ToList();

            var items = await BuildImportContractItemsAsync(request.Items, request.SupplierId, currentUserId, now, cancellationToken);

            var contractId = await _contractRepository.InsertImportAggregateAsync(contract, attachments, items, cancellationToken);
            return new CreateContractResponse { Id = contractId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create import contract failed for contract no: {ContractNo}", request.ContractNo);
            throw;
        }
    }

    public async Task<ContractDetailResponse?> GetDetailAsync(long contractId, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateId(contractId);
            var detail = await _contractRepository.GetDetailAsync(contractId, cancellationToken);
            return detail is null ? null : MapDetail(detail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get contract detail failed for id: {Id}", contractId);
            throw;
        }
    }

    public async Task<AddContractAttachmentResponse> AddAttachmentAsync(long contractId, CreateContractAttachmentRequest request, long currentUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateId(contractId);
            ValidateContractAttachment(request);
            ValidateId(currentUserId, "Current user id must be greater than 0.");

            var contract = await _contractRepository.GetByIdAsync(contractId, cancellationToken);
            if (contract is null)
            {
                throw new KeyNotFoundException("Contract not found.");
            }

            var id = await _contractRepository.InsertAttachmentAsync(
                BuildContractAttachment(contractId, request, currentUserId, DateTime.Now),
                cancellationToken);
            return new AddContractAttachmentResponse { Id = id };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Add contract attachment failed for contract id: {Id}", contractId);
            throw;
        }
    }

    public async Task<SearchContractResponse> SearchAsync(SearchContractRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidatePaging(request.Page, request.PageSize);
            if (!string.IsNullOrWhiteSpace(request.ContractType))
            {
                ValidateContractType(request.ContractType);
            }

            var result = await _contractRepository.SearchAsync(new ContractSearchCondition
            {
                Keyword = request.Keyword,
                ContractType = TrimToNull(request.ContractType)?.ToUpperInvariant(),
                Offset = (request.Page - 1) * request.PageSize,
                Limit = request.PageSize
            }, cancellationToken);

            var totalPages = result.Total == 0 ? 0 : (int)Math.Ceiling(result.Total / (double)request.PageSize);
            return new SearchContractResponse
            {
                Items = result.Items.Select(contract => new SearchContractItem
                {
                    Id = contract.Id,
                    ContractNo = contract.ContractNo,
                    ContractDate = contract.ContractDate,
                    ContractType = contract.ContractType,
                    BuyerId = contract.BuyerId,
                    BuyerName = contract.BuyerName,
                    SupplierId = contract.SupplierId,
                    SupplierName = contract.SupplierName,
                    ImportContractId = contract.ImportContractId,
                    ImportContractNo = contract.ImportContractNo,
                    SalesOrderId = contract.SalesOrderId,
                    Incoterm = contract.Incoterm,
                    PaymentTerms = contract.PaymentTerms,
                    ShipmentDatePlan = contract.ShipmentDatePlan,
                    Note = contract.Note
                }).ToList(),
                Total = result.Total,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Search contract failed.");
            throw;
        }
    }

    private async Task<List<ContractPackage>> BuildExportContractPackagesAsync(
        IEnumerable<CreateContractPackageRequest> packageRequests,
        long currentUserId,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var packages = new List<ContractPackage>();

        foreach (var request in packageRequests)
        {
            ValidateContractPackage(request);

            var package = await _packageRepository.GetByIdAsync(request.PackageId, cancellationToken);
            if (package is null)
            {
                throw new KeyNotFoundException("Package not found.");
            }

            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product is null)
            {
                throw new KeyNotFoundException("Product not found.");
            }

            var quantity = package.ProductQuantity * request.PackageQuantity;
            packages.Add(new ContractPackage
            {
                PackageId = request.PackageId,
                PackageCode = package.PackageCode,
                PackageName = package.PackageName,
                ProductId = request.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                ProductQuantity = package.ProductQuantity,
                PackageQuantity = request.PackageQuantity,
                Quantity = quantity,
                SellPrice = request.SellPrice,
                TotalSellAmount = quantity * request.SellPrice,
                Length = package.Length,
                Width = package.Width,
                Height = package.Height,
                Note = TrimToNull(request.Note),
                CreatedBy = currentUserId,
                CreatedDate = now
            });
        }

        return packages;
    }

    private async Task<List<ContractItem>> BuildImportContractItemsAsync(
        IEnumerable<CreateImportContractItemRequest> itemRequests,
        long supplierId,
        long currentUserId,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var items = new List<ContractItem>();

        foreach (var request in itemRequests)
        {
            ValidateImportContractItem(request);

            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product is null)
            {
                throw new KeyNotFoundException("Product not found.");
            }

            items.Add(new ContractItem
            {
                ProductId = request.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                SupplierId = supplierId,
                Quantity = request.Quantity,
                ImportPrice = request.ImportPrice,
                SellPrice = 0,
                TotalImportAmount = request.Quantity * request.ImportPrice,
                TotalSellAmount = 0,
                CreatedBy = currentUserId,
                CreatedDate = now
            });
        }

        return items;
    }

    private static ContractAttachment BuildContractAttachment(long contractId, CreateContractAttachmentRequest request, long currentUserId, DateTime now)
    {
        ValidateContractAttachment(request);

        return new ContractAttachment
        {
            ContractId = contractId,
            FileName = request.FileName.Trim(),
            FileUrl = request.FileUrl.Trim(),
            FileExtension = TrimToNull(request.FileExtension),
            CreatedBy = currentUserId,
            CreatedDate = now
        };
    }

    private static ContractDetailResponse MapDetail(ContractDetailData detail)
    {
        return new ContractDetailResponse
        {
            Id = detail.Contract.Id,
            ContractNo = detail.Contract.ContractNo,
            ContractDate = detail.Contract.ContractDate,
            ContractType = detail.Contract.ContractType,
            BuyerId = detail.Contract.BuyerId,
            BuyerName = detail.Contract.BuyerName,
            SupplierId = detail.Contract.SupplierId,
            SupplierName = detail.Contract.SupplierName,
            ImportContractId = detail.Contract.ImportContractId,
            ImportContractNo = detail.Contract.ImportContractNo,
            SalesOrderId = detail.Contract.SalesOrderId,
            Incoterm = detail.Contract.Incoterm,
            PaymentTerms = detail.Contract.PaymentTerms,
            ShipmentDatePlan = detail.Contract.ShipmentDatePlan,
            Note = detail.Contract.Note,
            CreatedBy = detail.Contract.CreatedBy,
            CreatedDate = detail.Contract.CreatedDate,
            UpdatedBy = detail.Contract.UpdatedBy,
            UpdatedDate = detail.Contract.UpdatedDate,
            Attachments = detail.Attachments.Select(attachment => new ContractAttachmentResponse
            {
                Id = attachment.Id,
                ContractId = attachment.ContractId,
                FileName = attachment.FileName,
                FileUrl = attachment.FileUrl,
                FileExtension = attachment.FileExtension
            }).ToList(),
            Packages = detail.Packages.Select(package => new ContractPackageResponse
            {
                Id = package.Id,
                ContractId = package.ContractId,
                PackageId = package.PackageId,
                PackageCode = package.PackageCode,
                PackageName = package.PackageName,
                ProductId = package.ProductId,
                ProductCode = package.ProductCode,
                ProductName = package.ProductName,
                ProductQuantity = package.ProductQuantity,
                PackageQuantity = package.PackageQuantity,
                Quantity = package.Quantity,
                SellPrice = package.SellPrice,
                TotalSellAmount = package.TotalSellAmount,
                Length = package.Length,
                Width = package.Width,
                Height = package.Height,
                Note = package.Note
            }).ToList(),
            Items = detail.Items.Select(item => new ImportContractItemResponse
            {
                Id = item.Id,
                ContractId = item.ContractId,
                ProductId = item.ProductId,
                ProductCode = item.ProductCode,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                ImportPrice = item.ImportPrice,
                TotalImportAmount = item.TotalImportAmount
            }).ToList()
        };
    }

    private static void ValidateCreateExport(CreateContractRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ContractNo))
        {
            throw new ArgumentException("ContractNo is required.");
        }

        if (request.ContractDate == default)
        {
            throw new ArgumentException("ContractDate is required.");
        }

        ValidateId(request.BuyerId, "BuyerId must be greater than 0.");

        if (string.IsNullOrWhiteSpace(request.ImportContractNo))
        {
            throw new ArgumentException("ImportContractNo is required.");
        }

        if (request.Packages.Count == 0)
        {
            throw new ArgumentException("Packages must have at least one item.");
        }

        foreach (var attachment in request.Attachments)
        {
            ValidateContractAttachment(attachment);
        }

        foreach (var package in request.Packages)
        {
            ValidateContractPackage(package);
        }
    }

    private static void ValidateCreateImport(CreateImportContractRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ContractNo))
        {
            throw new ArgumentException("ContractNo is required.");
        }

        if (request.ContractDate == default)
        {
            throw new ArgumentException("ContractDate is required.");
        }

        ValidateId(request.SupplierId, "SupplierId must be greater than 0.");

        if (request.Items.Count == 0)
        {
            throw new ArgumentException("Items must have at least one item.");
        }

        foreach (var attachment in request.Attachments)
        {
            ValidateContractAttachment(attachment);
        }

        foreach (var item in request.Items)
        {
            ValidateImportContractItem(item);
        }
    }

    private static void ValidateContractAttachment(CreateContractAttachmentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            throw new ArgumentException("FileName is required.");
        }

        if (string.IsNullOrWhiteSpace(request.FileUrl))
        {
            throw new ArgumentException("FileUrl is required.");
        }
    }

    private static void ValidateContractPackage(CreateContractPackageRequest request)
    {
        ValidateId(request.PackageId, "PackageId must be greater than 0.");
        ValidateId(request.ProductId, "ProductId must be greater than 0.");

        if (request.PackageQuantity <= 0)
        {
            throw new ArgumentException("PackageQuantity must be greater than 0.");
        }

        if (request.SellPrice < 0)
        {
            throw new ArgumentException("SellPrice must be greater than or equal to 0.");
        }
    }

    private static void ValidateImportContractItem(CreateImportContractItemRequest request)
    {
        ValidateId(request.ProductId, "ProductId must be greater than 0.");

        if (request.Quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than 0.");
        }

        if (request.ImportPrice < 0)
        {
            throw new ArgumentException("ImportPrice must be greater than or equal to 0.");
        }
    }

    private static void ValidateContractType(string contractType)
    {
        if (string.IsNullOrWhiteSpace(contractType))
        {
            throw new ArgumentException("ContractType is required.");
        }

        if (!AllowedContractTypes.Contains(contractType.Trim()))
        {
            throw new ArgumentException("ContractType must be IMPORT or EXPORT.");
        }
    }

    private static void ValidatePaging(int page, int pageSize)
    {
        if (page <= 0)
        {
            throw new ArgumentException("Page must be greater than 0.");
        }

        if (pageSize <= 0 || pageSize > 200)
        {
            throw new ArgumentException("PageSize must be between 1 and 200.");
        }
    }

    private static void ValidateId(long? id, string message = "Id must be greater than 0.")
    {
        if (id is null or <= 0)
        {
            throw new ArgumentException(message);
        }
    }

    private static string? TrimToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
