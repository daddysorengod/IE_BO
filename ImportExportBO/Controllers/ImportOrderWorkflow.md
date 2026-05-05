# Import Order Workflow API

Base routes:

- `api/Partner`
- `api/Product`
- `api/Contract`

Tat ca endpoint partner/contract yeu cau JWT Bearer token. Backend doc `ClaimTypes.NameIdentifier` tu token va ghi vao audit fields `createdby`/`updatedby`.

## Workflow hien tai

1. Tao partner nha cung cap voi `partnerType = SUPPLIER`.
2. Tao product trong Product CRUD.
3. Tao hop dong nhap voi `supplierId`, thong tin hop dong, attachments va danh sach `items[]`.
4. Moi dong `items[]` trong create import contract chon `productId`, `quantity`, `importPrice`.
5. Backend tao `contract`, `contractattachment`, `contractitem` trong cung transaction.
6. Sau do hop dong xuat co the link lai bang `importContractNo`.

## Import contract create

- **Method**: `POST`
- **URL**: `api/Contract/Insert-Import-Contract`

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

Rules:

- `supplierId` phai tham chieu partner loai `SUPPLIER`.
- `items` phai co it nhat 1 dong.
- `productId` phai tham chieu product.
- `quantity > 0`.
- `importPrice >= 0`.
- `totalImportAmount = quantity * importPrice`.

## Contract endpoint URLs

| API | Method | URL |
|---|---|---|
| Create import contract | `POST` | `api/Contract/Insert-Import-Contract` |
| Add attachment | `POST` | `api/Contract/Insert-Contract-Attachment/{contractId}` |
| Get contract detail | `GET` | `api/Contract/Get-Contract/{contractId}` |
| Search contracts | `GET` | `api/Contract/Search-Contract?keyword=&contractType=IMPORT&page=1&pageSize=20` |
