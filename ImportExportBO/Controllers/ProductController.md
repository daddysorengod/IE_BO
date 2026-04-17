# ProductController API

Base route: `api/Product`

## Response envelope
Tat ca API tra ve theo format:

```json
{
  "code": 0,
  "message": "string",
  "data": {}
}
```

- `code`: id hoac tong so ban ghi (voi search), `-1` khi loi.
- `message`: thong bao ket qua.
- `data`: du lieu tra ve (co the `null`).

---

## 1) Get product by id
- **Method**: `GET`
- **URL**: `api/Product/Get-Product/{productId}`

### Success (200)
```json
{
  "code": 1,
  "message": "Get product successfully.",
  "data": {
    "id": 1,
    "productCode": "P001",
    "productName": "Ao thun",
    "hsCode": "6109",
    "unitOfMeasure": "PCS",
    "listImage": [
      {
        "productImageId": 10,
        "imageUrl": "https://cdn/img1.jpg",
        "isMain": "Y",
        "sortOrder": 1
      }
    ]
  }
}
```

### Not found (404)
```json
{
  "code": -1,
  "message": "Product not found.",
  "data": null
}
```

---

## 2) Insert product
- **Method**: `POST`
- **URL**: `api/Product/Insert-Product`

### Request body
```json
{
  "productCode": "P001",
  "productName": "Ao thun",
  "hsCode": "6109",
  "unitOfMeasure": "PCS",
  "images": [
    {
      "productImageId": 0,
      "imageUrl": "https://cdn/img1.jpg",
      "isMain": "Y",
      "sortOrder": 1
    },
    {
      "productImageId": 0,
      "imageUrl": "https://cdn/img2.jpg",
      "isMain": "N",
      "sortOrder": 2
    }
  ]
}
```

### Success (201)
```json
{
  "code": 1,
  "message": "Created successfully.",
  "data": {
    "id": 1
  }
}
```

---

## 3) Update product
- **Method**: `PUT`
- **URL**: `api/Product/Update-Product`

### Request body
```json
{
  "id": 1,
  "productCode": "P001",
  "productName": "Ao thun premium",
  "hsCode": "6109",
  "unitOfMeasure": "PCS",
  "images": [
    {
      "productImageId": 10,
      "imageUrl": "https://cdn/img1-new.jpg",
      "isMain": "Y",
      "sortOrder": 1
    },
    {
      "productImageId": 0,
      "imageUrl": "https://cdn/img3.jpg",
      "isMain": "N",
      "sortOrder": 3
    }
  ]
}
```

- `productImageId > 0`: update anh cu.
- `productImageId = 0`: insert anh moi.

### Success (200)
```json
{
  "code": 1,
  "message": "Updated successfully.",
  "data": {
    "id": 1,
    "isUpdated": true
  }
}
```

### Not found (404)
```json
{
  "code": -1,
  "message": "Product not found.",
  "data": {
    "id": 1,
    "isUpdated": false
  }
}
```

---

## 4) Delete product
- **Method**: `DELETE`
- **URL**: `api/Product/Delete-Product/{productId}`

### Success (200)
```json
{
  "code": 1,
  "message": "Deleted successfully.",
  "data": {
    "id": 1,
    "isDeleted": true
  }
}
```

### Not found (404)
```json
{
  "code": -1,
  "message": "Product not found.",
  "data": {
    "id": 1,
    "isDeleted": false
  }
}
```

---

## 5) Search product
- **Method**: `GET`
- **URL**: `api/Product/Search-Product`

### Query params (optional)
- `keyword`
- `productCode`
- `productName`
- `hsCode`
- `unitOfMeasure`
- `page` (default: `1`)
- `pageSize` (default: `20`, max: `200`)

### Example URL
`api/Product/Search-Product?keyword=ao&unitOfMeasure=PCS&page=1&pageSize=10`

### Success (200)
```json
{
  "code": 2,
  "message": "Search successfully.",
  "data": {
    "items": [
      {
        "id": 1,
        "productCode": "P001",
        "productName": "Ao thun",
        "hsCode": "6109",
        "unitOfMeasure": "PCS"
      },
      {
        "id": 2,
        "productCode": "P002",
        "productName": "Ao khoac",
        "hsCode": "6201",
        "unitOfMeasure": "PCS"
      }
    ],
    "total": 2,
    "page": 1,
    "pageSize": 10,
    "totalPages": 1
  }
}
```

---

## Error responses chung
### Bad request (400)
```json
{
  "code": -1,
  "message": "<validation message>",
  "data": null
}
```

### Internal server error (500)
```json
{
  "code": -1,
  "message": "Internal server error.",
  "data": null
}
```
