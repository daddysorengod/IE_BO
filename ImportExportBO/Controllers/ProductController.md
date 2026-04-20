# ProductController API

Base route: `api/Product`

## Ghi chu chung

- JSON response duoc serialize theo `camelCase`.
- Tat ca API tra ve theo `ResponseBase<T>`:

```json
{
  "code": 0,
  "message": "string",
  "data": {}
}
```

- `code`
  - `id` cua ban ghi voi `get`, `insert`, `update`, `delete`
  - `total` voi `search`
  - `-1` khi co loi
- `message`: thong bao ket qua
- `data`: du lieu tra ve, co the la `null`

## 1) Get product by id

- **Method**: `GET`
- **URL**: `api/Product/Get-Product/{productId}`
- **Path param**: `productId > 0`

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
        "productId": 10,
        "imageUrl": "https://cdn/img1.jpg",
        "isDefault": "Y",
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

### Luu y

- Neu `productId <= 0` thi tra ve `400 BadRequest` voi message: `Id must be greater than 0.`
- Product da soft delete (`Delt = "Y"`) cung duoc xem la `not found`
- `listImage[].productId` la field legacy, hien tai dang chua `product image id`, khong phai product id goc

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
      "productId": 0,
      "imageUrl": "https://cdn/img1.jpg",
      "isDefault": "Y",
      "sortOrder": 1
    },
    {
      "productId": 0,
      "imageUrl": "https://cdn/img2.jpg",
      "isDefault": "N",
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

### Luu y

- Truong bat buoc: `productCode`, `productName`, `unitOfMeasure`
- `images` co the rong
- Tren create, `images[].productId` hien tai khong duoc dung; co the bo qua hoac de `0`
- Moi image can `imageUrl`, neu thieu se tra ve `400` voi message: `ImageUrl is required.`

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
      "productId": 10,
      "imageUrl": "https://cdn/img1-new.jpg",
      "isDefault": "Y",
      "sortOrder": 1
    },
    {
      "productId": 0,
      "imageUrl": "https://cdn/img3.jpg",
      "isDefault": "N",
      "sortOrder": 3
    }
  ]
}
```

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

### Luu y

- `id > 0` la bat buoc, neu khong se tra ve `400` voi message: `Id must be greater than 0.`
- Truong bat buoc: `productCode`, `productName`, `unitOfMeasure`
- `images[].productId > 0`: update image da ton tai
- `images[].productId = 0`: insert image moi
- `images[].productId` la field legacy, thuc te dang duoc dung nhu `product image id`
- Neu `images` rong thi API chi update thong tin product, khong dong vao image

## 4) Delete product

- **Method**: `DELETE`
- **URL**: `api/Product/Delete-Product/{productId}`
- **Path param**: `productId > 0`

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

### Luu y

- Neu `productId <= 0` thi tra ve `400 BadRequest` voi message: `Id must be greater than 0.`
- Delete hien tai la soft delete, repository set `Delt = "Y"`

## 5) Search product

- **Method**: `GET`
- **URL**: `api/Product/Search-Product`

### Query params

- `keyword`
- `productCode`
- `productName`
- `hsCode`
- `unitOfMeasure`
- `page` (default: `1`, phai `> 0`)
- `pageSize` (default: `20`, phai `> 0`, toi da `200`)

### Example URL

`api/Product/Search-Product?productName=ao&unitOfMeasure=PCS&page=1&pageSize=10`

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
        "unitOfMeasure": "PCS",
        "imageUrl": "https://cdn/img1.jpg"
      },
      {
        "id": 2,
        "productCode": "P002",
        "productName": "Ao khoac",
        "hsCode": "6201",
        "unitOfMeasure": "PCS",
        "imageUrl": "https://cdn/img2.jpg"
      }
    ],
    "total": 2,
    "page": 1,
    "pageSize": 10,
    "totalPages": 1
  }
}
```

### Luu y

- Search bo qua product co `Delt = "Y"`
- `keyword` co trong request model, nhung implementation repository hien tai chua dung field nay de loc

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

## Validation messages hien tai

- `Id must be greater than 0.`
- `ProductCode is required.`
- `ProductName is required.`
- `UnitOfMeasure is required.`
- `ImageUrl is required.`
- `Page must be greater than 0.`
- `PageSize must be greater than 0.`
- `PageSize must be less than or equal to 200.`
