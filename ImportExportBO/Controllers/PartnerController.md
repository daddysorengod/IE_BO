# PartnerController API

Base route: `api/Partner`

## Ghi chu chung

- Tat ca endpoint trong controller nay yeu cau JWT Bearer token.
- Backend doc `ClaimTypes.NameIdentifier` tu token de gan `createdby` hoac `updatedby`.
- Tat ca API tra ve theo `ResponseBase<T>`.
- JSON response duoc serialize theo `camelCase`.
- `partnerType` hop le: `CUSTOMER`, `SUPPLIER`, `SHIPPING`.
- Partner `CUSTOMER` duoc dung lam `buyerId` khi tao contract.
- Partner `SUPPLIER` giu lai cho nhu cau quan ly nha cung cap sau nay; luong contract/package hien tai chua yeu cau `supplierId`.

## Tong quan endpoint

| API | Method | URL | Auth |
|---|---|---|---|
| Get partner | `GET` | `api/Partner/Get-Partner/{partnerId}` | Bearer token |
| Search partners | `GET` | `api/Partner/Search-Partner` | Bearer token |
| Create partner | `POST` | `api/Partner/Insert-Partner` | Bearer token |
| Update partner | `PUT` | `api/Partner/Update-Partner/{partnerId}` | Bearer token |

## 1) Get partner by id

- **Method**: `GET`
- **URL**: `api/Partner/Get-Partner/{partnerId}`
- **Path param**: `partnerId > 0`

### Success (200)

```json
{
  "code": 1,
  "message": "Get partner successfully.",
  "data": {
    "id": 1,
    "partnerCode": "CUS001",
    "partnerName": "Customer A",
    "partnerType": "CUSTOMER",
    "address": "Ho Chi Minh",
    "phone": "0900000000",
    "email": "customer@example.com",
    "isActive": true,
    "createdBy": 999999,
    "createdDate": "2026-04-22T10:00:00",
    "updatedBy": null,
    "updatedDate": null
  }
}
```

### Loi chinh

- `400 BadRequest`: `partnerId <= 0`
- `404 NotFound`: partner khong ton tai
- `500 InternalServerError`: loi he thong

## 2) Search partners

- **Method**: `GET`
- **URL**: `api/Partner/Search-Partner`

### Query params

- `keyword`: tim theo `partnerCode`, `partnerName`, `email`, `phone`
- `partnerType`: `CUSTOMER`, `SUPPLIER`, `SHIPPING`
- `isActive`: `true` hoac `false`
- `page`: default `1`, phai `> 0`
- `pageSize`: default `20`, phai tu `1` den `200`

### Example URL

`api/Partner/Search-Partner?keyword=customer&partnerType=CUSTOMER&isActive=true&page=1&pageSize=20`

### Success (200)

```json
{
  "code": 1,
  "message": "Search partners successfully.",
  "data": {
    "items": [
      {
        "id": 1,
        "partnerCode": "CUS001",
        "partnerName": "Customer A",
        "partnerType": "CUSTOMER",
        "address": "Ho Chi Minh",
        "phone": "0900000000",
        "email": "customer@example.com",
        "isActive": true,
        "createdBy": 999999,
        "createdDate": "2026-04-22T10:00:00",
        "updatedBy": null,
        "updatedDate": null
      }
    ],
    "total": 1,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1
  }
}
```

### Loi chinh

- `400 BadRequest`: paging khong hop le hoac `partnerType` khong hop le
- `500 InternalServerError`: loi he thong

## 3) Create partner

- **Method**: `POST`
- **URL**: `api/Partner/Insert-Partner`

### Request body

```json
{
  "partnerCode": "CUS001",
  "partnerName": "Customer A",
  "partnerType": "CUSTOMER",
  "address": "Ho Chi Minh",
  "phone": "0900000000",
  "email": "customer@example.com",
  "isActive": true
}
```

### Success (201)

```json
{
  "code": 1,
  "message": "Created partner successfully.",
  "data": {
    "id": 1
  }
}
```

### Loi chinh

- `400 BadRequest`: thieu `partnerCode`, `partnerName`, `partnerType` hoac `partnerType` khong hop le
- `409 Conflict`: `PartnerCode already exists.`
- `500 InternalServerError`: loi he thong

## 4) Update partner

- **Method**: `PUT`
- **URL**: `api/Partner/Update-Partner/{partnerId}`
- **Path param**: `partnerId > 0`

### Request body

```json
{
  "partnerCode": "CUS001",
  "partnerName": "Customer A Updated",
  "partnerType": "CUSTOMER",
  "address": "Ho Chi Minh",
  "phone": "0911111111",
  "email": "customer-new@example.com",
  "isActive": true
}
```

`id` trong body khong can gui. Controller lay id tu route va gan vao request truoc khi goi service.

### Success (200)

```json
{
  "code": 1,
  "message": "Updated partner successfully.",
  "data": {
    "id": 1,
    "isUpdated": true
  }
}
```

### Loi chinh

- `400 BadRequest`: du lieu khong hop le
- `404 NotFound`: partner khong ton tai
- `409 Conflict`: `PartnerCode already exists.`
- `500 InternalServerError`: loi he thong

## Validation messages hien tai

- `Id must be greater than 0.`
- `Current user id must be greater than 0.`
- `PartnerCode is required.`
- `PartnerName is required.`
- `PartnerType is required.`
- `PartnerType must be CUSTOMER, SUPPLIER, or SHIPPING.`
- `Page must be greater than 0.`
- `PageSize must be between 1 and 200.`
