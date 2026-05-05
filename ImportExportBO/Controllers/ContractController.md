# ContractController API

Base route: `api/Contract`

## Ghi chu chung

- Tat ca endpoint yeu cau JWT Bearer token.
- Backend doc `ClaimTypes.NameIdentifier` tu token de gan audit fields.
- Tat ca API tra ve theo `ResponseBase<T>`.
- He thong ho tro 2 loai hop dong:
  - `IMPORT`: hop dong nhap, nhap theo `product`, `quantity`, `importPrice`, khong lien quan den package.
  - `EXPORT`: hop dong xuat, nhap theo `package` + `product`, co `sellPrice`, va phai link sang 1 hop dong nhap bang `importContractNo`.
- `buyerId` chi dung cho `EXPORT`, phai la partner co `partnerType = CUSTOMER`.
- `supplierId` chi dung cho `IMPORT`, phai la partner co `partnerType = SUPPLIER`.
- `packageId` trong request export la `packinglist.id`.
- Luong export khong nhap `importPrice`; gia nhap duoc quan ly o hop dong nhap.

## Tong quan endpoint

| API | Method | URL | Auth |
|---|---|---|---|
| Search contracts | `GET` | `api/Contract/Search-Contract` | Bearer token |
| Get contract detail | `GET` | `api/Contract/Get-Contract/{contractId}` | Bearer token |
| Create export contract | `POST` | `api/Contract/Insert-Contract` | Bearer token |
| Create import contract | `POST` | `api/Contract/Insert-Import-Contract` | Bearer token |
| Add attachment | `POST` | `api/Contract/Insert-Contract-Attachment/{contractId}` | Bearer token |

## 1) Search contracts

- **Method**: `GET`
- **URL**: `api/Contract/Search-Contract`

### Example URL

`api/Contract/Search-Contract?keyword=HD&page=1&pageSize=20&contractType=EXPORT`

### Query params

- `keyword`: tim theo contract no, incoterm, payment terms, shipment plan, note, buyer name, supplier name, ma hop dong nhap lien ket
- `contractType`: tuy chon, `IMPORT` hoac `EXPORT`
- `page`: default `1`, phai `> 0`
- `pageSize`: default `20`, phai tu `1` den `200`

### Success (200)

```json
{
  "code": 1,
  "message": "Search contracts successfully.",
  "data": {
    "items": [
      {
        "id": 1,
        "contractNo": "HDX001",
        "contractDate": "2026-05-04",
        "contractType": "EXPORT",
        "buyerId": 10,
        "buyerName": "Customer A",
        "supplierId": null,
        "supplierName": null,
        "importContractId": 5,
        "importContractNo": "HDN001",
        "salesOrderId": null,
        "incoterm": "FOB",
        "paymentTerms": "30% deposit",
        "shipmentDatePlan": "May 2026",
        "note": "Hop dong xuat"
      }
    ],
    "total": 1,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1
  }
}
```

## 2) Get contract detail

- **Method**: `GET`
- **URL**: `api/Contract/Get-Contract/{contractId}`
- **Path param**: `contractId > 0`

### Success (200) - EXPORT

```json
{
  "code": 1,
  "message": "Get contract successfully.",
  "data": {
    "id": 1,
    "contractNo": "HDX001",
    "contractDate": "2026-05-04",
    "contractType": "EXPORT",
    "buyerId": 10,
    "buyerName": "Customer A",
    "supplierId": null,
    "supplierName": null,
    "importContractId": 5,
    "importContractNo": "HDN001",
    "salesOrderId": null,
    "incoterm": "FOB",
    "paymentTerms": "30% deposit",
    "shipmentDatePlan": "May 2026",
    "note": "Hop dong xuat",
    "attachments": [],
    "packages": [
      {
        "id": 1,
        "contractId": 1,
        "packageId": 10,
        "packageCode": "BOX_001",
        "packageName": "Thung carton 60x40x40",
        "productId": 5,
        "productCode": "P001",
        "productName": "Product A",
        "productQuantity": 20,
        "packageQuantity": 5,
        "quantity": 100,
        "sellPrice": 8.0,
        "totalSellAmount": 800.0,
        "length": 60,
        "width": 40,
        "height": 40,
        "note": "Dong kien hang 1"
      }
    ],
    "items": []
  }
}
```

### Success (200) - IMPORT

```json
{
  "code": 5,
  "message": "Get contract successfully.",
  "data": {
    "id": 5,
    "contractNo": "HDN001",
    "contractDate": "2026-05-03",
    "contractType": "IMPORT",
    "buyerId": null,
    "buyerName": null,
    "supplierId": 20,
    "supplierName": "Supplier A",
    "importContractId": null,
    "importContractNo": null,
    "salesOrderId": null,
    "incoterm": "CIF",
    "paymentTerms": "100% after invoice",
    "shipmentDatePlan": "May 2026",
    "note": "Hop dong nhap",
    "attachments": [],
    "packages": [],
    "items": [
      {
        "id": 11,
        "contractId": 5,
        "productId": 5,
        "productCode": "P001",
        "productName": "Product A",
        "quantity": 100,
        "importPrice": 5.5,
        "totalImportAmount": 550.0
      }
    ]
  }
}
```

## 3) Create export contract

- **Method**: `POST`
- **URL**: `api/Contract/Insert-Contract`

### Request body

```json
{
  "contractNo": "HDX001",
  "contractDate": "2026-05-04",
  "buyerId": 10,
  "importContractNo": "HDN001",
  "incoterm": "FOB",
  "paymentTerms": "30% deposit",
  "shipmentDatePlan": "May 2026",
  "note": "Hop dong xuat",
  "attachments": [
    {
      "fileName": "export-contract.pdf",
      "fileUrl": "https://cdn/export-contract.pdf",
      "fileExtension": ".pdf"
    }
  ],
  "packages": [
    {
      "packageId": 10,
      "productId": 5,
      "packageQuantity": 5,
      "sellPrice": 8.0,
      "note": "Dong kien hang 1"
    }
  ]
}
```

### Rules

- `buyerId` la bat buoc va phai tham chieu partner loai `CUSTOMER`.
- `importContractNo` la bat buoc va phai tro toi 1 hop dong `IMPORT` da ton tai.
- `packages` phai co it nhat 1 dong.
- `packageId` phai tham chieu `packinglist.id` chua soft-delete.
- `productId` phai tham chieu product chua soft-delete.
- `packageQuantity` phai `> 0`.
- `sellPrice` phai `>= 0`.
- Backend tinh:
  - `quantity = package.productquantity * packageQuantity`
  - `totalSellAmount = quantity * sellPrice`
- Luong export khong nhan `importPrice`.

## 4) Create import contract

- **Method**: `POST`
- **URL**: `api/Contract/Insert-Import-Contract`

### Request body

```json
{
  "contractNo": "HDN001",
  "contractDate": "2026-05-03",
  "supplierId": 20,
  "incoterm": "CIF",
  "paymentTerms": "100% after invoice",
  "shipmentDatePlan": "May 2026",
  "note": "Hop dong nhap",
  "attachments": [
    {
      "fileName": "import-contract.pdf",
      "fileUrl": "https://cdn/import-contract.pdf",
      "fileExtension": ".pdf"
    }
  ],
  "items": [
    {
      "productId": 5,
      "quantity": 100,
      "importPrice": 5.5
    }
  ]
}
```

### Rules

- `supplierId` la bat buoc va phai tham chieu partner loai `SUPPLIER`.
- `items` phai co it nhat 1 dong.
- `productId` phai tham chieu product chua soft-delete.
- `quantity` phai `> 0`.
- `importPrice` phai `>= 0`.
- Backend tinh `totalImportAmount = quantity * importPrice`.

## 5) Add contract attachment

- **Method**: `POST`
- **URL**: `api/Contract/Insert-Contract-Attachment/{contractId}`
- **Path param**: `contractId > 0`

### Request body

```json
{
  "fileName": "quote.xlsx",
  "fileUrl": "https://cdn/quote.xlsx",
  "fileExtension": ".xlsx"
}
```

## Validation messages hien tai

- `Id must be greater than 0.`
- `Current user id must be greater than 0.`
- `ContractNo is required.`
- `ContractDate is required.`
- `BuyerId must be greater than 0.`
- `SupplierId must be greater than 0.`
- `ImportContractNo is required.`
- `Buyer partner not found.`
- `Supplier partner not found.`
- `Buyer partner must have PartnerType CUSTOMER.`
- `Supplier partner must have PartnerType SUPPLIER.`
- `Import contract not found.`
- `Linked contract must be IMPORT contract.`
- `Packages must have at least one item.`
- `Items must have at least one item.`
- `PackageId must be greater than 0.`
- `Package not found.`
- `ProductId must be greater than 0.`
- `Product not found.`
- `PackageQuantity must be greater than 0.`
- `Quantity must be greater than 0.`
- `ImportPrice must be greater than or equal to 0.`
- `SellPrice must be greater than or equal to 0.`
- `ContractType must be IMPORT or EXPORT.`
- `FileName is required.`
- `FileUrl is required.`
- `Page must be greater than 0.`
- `PageSize must be between 1 and 200.`
