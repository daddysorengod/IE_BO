# AllcodeController API

Base route: `api/Allcode`

## Ghi chu chung

- Endpoint yeu cau JWT Bearer token.
- API tra ve theo `ResponseBase<T>`.
- API nay dung de lay du lieu fix cung cho combobox tu bang `public.allcode`.
- Chi tra ve du lieu co `display = 'Y'`.
- Doi voi yeu cau truoc do bi lap tham so, API nay dung cap tham so hop ly theo schema hien tai: `cdName` va `cdValue`.

## Tong quan endpoint

| API | Method | URL | Auth |
|---|---|---|---|
| Search allcodes | `GET` | `api/Allcode/Search-Allcode` | Bearer token |

## 1) Search allcodes

- **Method**: `GET`
- **URL**: `api/Allcode/Search-Allcode`

### Query params

- `cdName`: bat buoc, ten nhom ma can lay, vi du `USER_ROLE`, `PARTNER_TYPE`
- `cdValue`: tuy chon, neu truyen thi loc them theo gia tri cu the trong nhom

### Example URL

Lay toan bo danh muc theo nhom:

`api/Allcode/Search-Allcode?cdName=PARTNER_TYPE`

Lay 1 gia tri cu the:

`api/Allcode/Search-Allcode?cdName=PARTNER_TYPE&cdValue=CUSTOMER`

### Success (200)

```json
{
  "code": 3,
  "message": "Search allcodes successfully.",
  "data": {
    "items": [
      {
        "id": 4,
        "cdName": "PARTNER_TYPE",
        "cdValue": "CUSTOMER",
        "textVn": "Khach hang",
        "textEn": "Customer",
        "display": "Y"
      },
      {
        "id": 5,
        "cdName": "PARTNER_TYPE",
        "cdValue": "SUPPLIER",
        "textVn": "Nha cung cap",
        "textEn": "Supplier",
        "display": "Y"
      },
      {
        "id": 6,
        "cdName": "PARTNER_TYPE",
        "cdValue": "SHIPPING",
        "textVn": "Van chuyen",
        "textEn": "Shipping",
        "display": "Y"
      }
    ],
    "total": 3
  }
}
```

### Rules

- `cdName` la bat buoc.
- `cdValue` la tuy chon.
- API chi tra ve cac ban ghi `display = 'Y'`.
- `cdName` duoc normalize sang uppercase trong service truoc khi query.

## Validation messages hien tai 

- `CdName is required.`
