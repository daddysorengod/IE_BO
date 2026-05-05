# Export Order Workflow API

Base routes:

- `api/Partner`
- `api/Product`
- `api/Package`
- `api/Contract`

Tat ca endpoint partner/package/contract yeu cau JWT Bearer token. Backend doc `ClaimTypes.NameIdentifier` tu token va ghi vao audit fields `createdby`/`updatedby`.

## Workflow hien tai

1. Tao partner nguoi mua hang voi `partnerType = CUSTOMER`.
2. Tao product trong Product CRUD.
3. Tao package master trong Package CRUD. Du lieu nay luu vao `packinglist`; `packageCode` map voi `packinglist.packinglistno`.
4. Tao hop dong nhap truoc bang `api/Contract/Insert-Import-Contract`.
5. Tao hop dong xuat voi `buyerId`, `importContractNo`, thong tin hop dong, attachments va danh sach `packages[]`.
6. Moi dong `packages[]` trong create export contract chon `packageId`, `productId`, `packageQuantity`, `sellPrice`, `note`.
7. Backend tao `contract`, `contractattachment`, `contractpackage`, `contractitem` trong cung transaction.
8. Contract detail tra D/R/C cua package master trong `packinglist` de UI hien thi khi chon loai kien.

Khong co buoc goi API AddItem rieng cho package hay contract. API create export contract khong tao `packinglist` moi, chi tham chieu package master da khai bao; `packinglistitem` la legacy.

## Export contract create

- **Method**: `POST`
- **URL**: `api/Contract/Insert-Contract`

```json
{
  "contractNo": "HDX001",
  "contractDate": "2026-05-04",
  "buyerId": 1,
  "importContractNo": "HDN001",
  "incoterm": "FOB",
  "paymentTerms": "30% deposit",
  "shipmentDatePlan": "May 2026",
  "note": "Hop dong xuat",
  "attachments": [
    {
      "fileName": "contract.pdf",
      "fileUrl": "https://cdn/contract.pdf",
      "fileExtension": ".pdf"
    }
  ],
  "packages": [
    {
      "packageId": 10,
      "productId": 1,
      "packageQuantity": 5,
      "sellPrice": 8.0,
      "note": "Dong kien hang 1"
    }
  ]
}
```

Rules:

- `buyerId` phai tham chieu partner loai `CUSTOMER`.
- `importContractNo` phai tro toi 1 hop dong `IMPORT` da ton tai.
- `packageId` phai tham chieu `packinglist.id`.
- `productId` phai tham chieu product.
- `packageQuantity > 0`.
- `sellPrice >= 0`.
- `quantity = package.productquantity * packageQuantity`.
- `totalSellAmount = quantity * sellPrice`.
- Luong export khong nhap `importPrice`.

## Contract endpoint URLs

| API | Method | URL |
|---|---|---|
| Create export contract | `POST` | `api/Contract/Insert-Contract` |
| Create import contract | `POST` | `api/Contract/Insert-Import-Contract` |
| Add attachment | `POST` | `api/Contract/Insert-Contract-Attachment/{contractId}` |
| Get contract detail | `GET` | `api/Contract/Get-Contract/{contractId}` |
| Search contracts | `GET` | `api/Contract/Search-Contract?keyword=&contractType=&page=1&pageSize=20` |

## Package CRUD

- **Method**: `POST`
- **URL**: `api/Package/Insert-Package`

```json
{
  "packageCode": "BOX_001",
  "packageName": "Thung carton 60x40x40",
  "productQuantity": 20,
  "length": 60,
  "width": 40,
  "height": 40,
  "note": "Loai thung tieu chuan"
}
```

## Package endpoint URLs

| API | Method | URL |
|---|---|---|
| Create package | `POST` | `api/Package/Insert-Package` |
| Update package | `PUT` | `api/Package/Update-Package/{packageId}` |
| Delete package | `DELETE` | `api/Package/Delete-Package/{packageId}` |
| Get package | `GET` | `api/Package/Get-Package/{packageId}` |
| Search packages | `GET` | `api/Package/Search-Package?keyword=&page=1&pageSize=20` |
