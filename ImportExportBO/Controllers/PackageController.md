# PackageController API

Base route: `api/Package`

## Ghi chu chung

- Tat ca endpoint yeu cau JWT Bearer token.
- Backend doc `ClaimTypes.NameIdentifier` tu token de gan audit fields.
- Tat ca API tra ve theo `ResponseBase<T>`.
- JSON response duoc serialize theo `camelCase`.
- `package` trong API la danh muc loai kien hang, CRUD rieng giong `product`.
- Du lieu package duoc luu vao bang DB `packinglist`.
- `packageCode` map vao `packinglist.packinglistno`.
- `packageId` trong API contract la `packinglist.id`.
- API package CRUD khong dung `packinglistitem`; bang do la legacy.
- `productQuantity` la so luong san pham trong 1 kien.
- `length`, `width`, `height` la D/R/C cua loai kien, dung de UI hien thi khi chon loai kien trong man hinh contract.

## Tong quan endpoint

| API | Method | URL | Auth |
|---|---|---|---|
| Search packages | `GET` | `api/Package/Search-Package` | Bearer token |
| Get package | `GET` | `api/Package/Get-Package/{packageId}` | Bearer token |
| Create package | `POST` | `api/Package/Insert-Package` | Bearer token |
| Update package | `PUT` | `api/Package/Update-Package/{packageId}` | Bearer token |
| Delete package | `DELETE` | `api/Package/Delete-Package/{packageId}` | Bearer token |

## 1) Search packages

- **Method**: `GET`
- **URL**: `api/Package/Search-Package`

### Example URL

`api/Package/Search-Package?keyword=BOX&page=1&pageSize=20`

### Query params

- `keyword`: tim theo package code, package name, note
- `page`: default `1`, phai `> 0`
- `pageSize`: default `20`, phai tu `1` den `200`

### Success (200)

```json
{
  "code": 1,
  "message": "Search packages successfully.",
  "data": {
    "items": [
      {
        "id": 1,
        "packageCode": "BOX_001",
        "packageName": "Thung carton 60x40x40",
        "productQuantity": 20,
        "length": 60,
        "width": 40,
        "height": 40,
        "note": "Loai thung tieu chuan"
      }
    ],
    "total": 1,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1
  }
}
```

## 2) Get package

- **Method**: `GET`
- **URL**: `api/Package/Get-Package/{packageId}`
- **Path param**: `packageId > 0`

### Success (200)

Tra ve `PackageResponse`.

### Loi chinh

- `400 BadRequest`: `packageId <= 0`
- `404 NotFound`: package khong ton tai hoac da soft-delete

## 3) Create package

- **Method**: `POST`
- **URL**: `api/Package/Insert-Package`

### Request body

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

### Success (201)

```json
{
  "code": 1,
  "message": "Created package successfully.",
  "data": {
    "id": 1
  }
}
```

## 4) Update package

- **Method**: `PUT`
- **URL**: `api/Package/Update-Package/{packageId}`
- **Path param**: `packageId > 0`

### Request body

Body giong create package.

### Success (200)

```json
{
  "code": 1,
  "message": "Updated package successfully.",
  "data": {
    "id": 1,
    "isUpdated": true
  }
}
```

## 5) Delete package

- **Method**: `DELETE`
- **URL**: `api/Package/Delete-Package/{packageId}`
- **Path param**: `packageId > 0`

Delete la soft delete bang `delt = 'Y'`.

### Success (200)

```json
{
  "code": 1,
  "message": "Deleted package successfully.",
  "data": {
    "id": 1,
    "isDeleted": true
  }
}
```

## Validation messages hien tai

- `Id must be greater than 0.`
- `Current user id must be greater than 0.`
- `PackageCode is required.`
- `PackageName is required.`
- `ProductQuantity must be greater than 0.`
- `PackageCode already exists.`
- `Length must be greater than or equal to 0.`
- `Width must be greater than or equal to 0.`
- `Height must be greater than or equal to 0.`
- `Page must be greater than 0.`
- `PageSize must be between 1 and 200.`
