# IE_BO

Backend .NET 8 cho nghiep vu Import/Export BO.

## Solution

Solution chinh:

```powershell
dotnet build ImportExportBO\ImportExportBO.sln
```

API host:

```powershell
dotnet run --project ImportExportBO\ImportExportBO.csproj
```

## Module chinh

- User/Auth: login bang `username/password`, JWT access token, refresh token.
- Product: CRUD san pham va anh san pham.
- Partner: CRUD doi tac `CUSTOMER`, `SUPPLIER`, `SHIPPING`.
- Package: CRUD loai kien, du lieu luu trong bang `packinglist`.
- Contract: ho tro hop dong nhap va hop dong xuat, attachments, dong kien hang export va dong san pham import.

## API docs

- [UserController.md](ImportExportBO/Controllers/UserController.md)
- [ProductController.md](ImportExportBO/Controllers/ProductController.md)
- [PartnerController.md](ImportExportBO/Controllers/PartnerController.md)
- [PackageController.md](ImportExportBO/Controllers/PackageController.md)
- [ContractController.md](ImportExportBO/Controllers/ContractController.md)
- [ExportOrderWorkflow.md](ImportExportBO/Controllers/ExportOrderWorkflow.md)
- [ImportOrderWorkflow.md](ImportExportBO/Controllers/ImportOrderWorkflow.md)

## Database docs

- [Database.md](Database.md)
- [database_ver1.md](database_ver1.md)
- [database_diagram_ver1.md](database_diagram_ver1.md)
- [database_schema_summary_22060422.md](database_schema_summary_22060422.md)

## Route convention

Controller routes follow the same style as `ProductController`:

- `api/Product/...`
- `api/Partner/...`
- `api/Package/...`
- `api/Contract/...`
- `api/User/...`
