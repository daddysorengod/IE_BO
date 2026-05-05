# UserController API

Base route: `api/User`

## Ghi chu chung

- JSON response duoc serialize theo `camelCase`
- Tat ca API tra ve theo `ResponseBase<T>`
- `code = id` khi thanh cong, `-1` khi loi
- Access token la JWT Bearer
- Refresh token duoc luu trong bang `public."user"`
- He thong co ho tro 1 built-in `supperadmin` doc tu `ImportExportBO/appsettings.json`
- Login su dung `username` va `password`; request login khong dung `email`
- Hien tai chi `Logout` co `[Authorize]`
- `Login`, `Refresh-Token`, `Revoke-Token`, `Register-User` duoc `[AllowAnonymous]`
- `Update-User`, `Delete-User`, `Activate-User` hien tai chua gan `[Authorize]` trong controller

## Built-in supperadmin

- Config nam trong `appsettings.json`:

```json
"SuperAdmin": {
  "supperuser": "supperadmin",
  "supperpassword": "SupperAdmin@123",
  "userid": 999999,
  "fullname": "Supper Admin",
  "role": "SuperAdmin",
  "imageurl": ""
}
```

- Khi `username` trong request login bang `supperuser` va `password` bang `supperpassword`, he thong:
- Bo qua truy van DB
- Tao 1 user built-in voi full quyen theo `role`
- Van cap `accessToken` va `refreshToken` nhu user thuong
- Refresh token cua built-in user duoc giu trong memory cua API process, khong luu DB
- Neu restart API, refresh token cua built-in user se mat hieu luc

## Tong quan endpoint

| API | Method | URL | Auth |
|---|---|---|---|
| Login | `POST` | `api/User/Login` | Khong can token |
| Refresh token | `POST` | `api/User/Refresh-Token` | Khong can token |
| Revoke token | `POST` | `api/User/Revoke-Token` | Khong can token |
| Logout | `POST` | `api/User/Logout/{userId}` | Bearer token |
| Register user | `POST` | `api/User/Register-User` | Khong can token |
| Update user | `PUT` | `api/User/Update-User` | Chua yeu cau token |
| Delete user | `DELETE` | `api/User/Delete-User/{userId}` | Chua yeu cau token |
| Activate user | `PUT` | `api/User/Activate-User/{userId}` | Chua yeu cau token |

## 1) Login

- **Method**: `POST`
- **URL**: `api/User/Login`
- **Auth**: khong can token

### Request body

```json
{
  "username": "nguyenvana",
  "password": "123456"
}
```

### Success (200)

```json
{
  "code": 1,
  "message": "Login successfully.",
  "data": {
    "id": 1,
    "username": "nguyenvana",
    "email": "user@example.com",
    "fullName": "Nguyen Van A",
    "imageUrl": "https://cdn/avatar.jpg",
    "role": "Admin",
    "tokenType": "Bearer",
    "accessToken": "<jwt-access-token>",
    "accessTokenExpiresAtUtc": "2026-04-21T10:15:00Z",
    "refreshToken": "<refresh-token>",
    "refreshTokenExpiresAtUtc": "2026-04-28T10:00:00Z",
    "isActive": true
  }
}
```

### Loi chinh

- `400 BadRequest`: request thieu `username` hoac `password`
- `401 Unauthorized`: sai username hoac password
- `403 Forbidden`: user chua active
- `500 InternalServerError`: loi he thong

### Luu y

- Login thanh cong se cap moi ca `accessToken` va `refreshToken`
- `accessToken` dung de goi API duoc bao ve boi JWT
- `refreshToken` dung de xin cap lai token khi access token het han
- `username` co the la username user trong DB hoac gia tri `supperuser` duoc cau hinh san

## 2) Refresh token

- **Method**: `POST`
- **URL**: `api/User/Refresh-Token`
- **Auth**: khong can token

### Request body

```json
{
  "refreshToken": "<refresh-token>"
}
```

### Success (200)

```json
{
  "code": 1,
  "message": "Refresh token successfully.",
  "data": {
    "id": 1,
    "username": "nguyenvana",
    "email": "user@example.com",
    "fullName": "Nguyen Van A",
    "imageUrl": "https://cdn/avatar.jpg",
    "role": "Admin",
    "tokenType": "Bearer",
    "accessToken": "<jwt-access-token-moi>",
    "accessTokenExpiresAtUtc": "2026-04-21T10:30:00Z",
    "refreshToken": "<refresh-token-moi>",
    "refreshTokenExpiresAtUtc": "2026-04-28T10:15:00Z",
    "isActive": true
  }
}
```

### Loi chinh

- `400 BadRequest`: thieu `refreshToken`
- `401 Unauthorized`: refresh token sai hoac het han
- `500 InternalServerError`: loi he thong

### Luu y

- Refresh token hop le se duoc rotate moi lan refresh
- Sau khi refresh thanh cong, refresh token cu khong con dung duoc nua
- Doi voi built-in `supperadmin`, refresh token duoc kiem tra trong memory thay vi DB

## 3) Revoke token

- **Method**: `POST`
- **URL**: `api/User/Revoke-Token`
- **Auth**: khong can token

### Request body

```json
{
  "refreshToken": "<refresh-token>"
}
```

### Success (200)

```json
{
  "code": 1,
  "message": "Revoke token successfully.",
  "data": {
    "id": 1,
    "isRevoked": true
  }
}
```

### Loi chinh

- `400 BadRequest`: thieu `refreshToken`
- `404 NotFound`: refresh token khong ton tai trong DB
- `500 InternalServerError`: loi he thong

### Luu y

- API nay chi revoke `refreshToken`
- `accessToken` da phat ra truoc do van dung duoc den khi het han
- Revoke thanh cong se clear `refreshtoken` va `refreshtokenexpiresatutc`
- Doi voi built-in `supperadmin`, revoke se clear refresh token dang giu trong memory

## 4) Logout

- **Method**: `POST`
- **URL**: `api/User/Logout/{userId}`
- **Auth**: bat buoc Bearer token

### Header

```http
Authorization: Bearer <jwt-access-token>
```

### Success (200)

```json
{
  "code": 1,
  "message": "Logout successfully.",
  "data": {
    "id": 1,
    "isLoggedOut": true
  }
}
```

### Loi chinh

- `400 BadRequest`: `userId <= 0`
- `403 Forbidden`: `userId` tren route khac `ClaimTypes.NameIdentifier` trong JWT
- `404 NotFound`: user khong ton tai
- `500 InternalServerError`: loi he thong

### Luu y

- Logout hien tai cung chi clear `refreshToken`
- Access token hien tai khong bi blacklist, nen van dung duoc den khi het han
- Doi voi built-in `supperadmin`, logout se clear refresh token trong memory

## 5) Register user

- **Method**: `POST`
- **URL**: `api/User/Register-User`
- **Auth**: khong can token

### Request body

```json
{
  "username": "nguyenvana",
  "email": "user@example.com",
  "password": "123456",
  "fullName": "Nguyen Van A",
  "imageUrl": "https://cdn/avatar.jpg",
  "role": "Admin"
}
```

### Success (201)

```json
{
  "code": 1,
  "message": "Registered successfully.",
  "data": {
    "id": 1,
    "isActive": false
  }
}
```

### Loi chinh

- `400 BadRequest`: thieu cac truong bat buoc
- `409 Conflict`: username hoac email da ton tai
- `500 InternalServerError`: loi he thong

### Luu y

- User moi dang ky mac dinh `isActive = false`

## 6) Update user

- **Method**: `PUT`
- **URL**: `api/User/Update-User`
- **Auth**: hien tai chua yeu cau token trong controller

### Request body

```json
{
  "id": 1,
  "username": "nguyenvanb",
  "email": "user@example.com",
  "password": "12345678",
  "fullName": "Nguyen Van B",
  "imageUrl": "https://cdn/avatar-new.jpg",
  "role": "Manager"
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

### Loi chinh

- `400 BadRequest`: du lieu khong hop le
- `404 NotFound`: user khong ton tai
- `409 Conflict`: username hoac email trung voi user khac
- `500 InternalServerError`: loi he thong

### Luu y

- `password` la optional; bo trong thi giu password cu

## 7) Delete user

- **Method**: `DELETE`
- **URL**: `api/User/Delete-User/{userId}`
- **Auth**: hien tai chua yeu cau token trong controller

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

### Loi chinh

- `400 BadRequest`: `userId <= 0`
- `404 NotFound`: user khong ton tai
- `500 InternalServerError`: loi he thong

### Luu y

- Delete hien tai la soft delete
- He thong set `delt = "Y"`, `isactive = "N"`, clear `refreshtoken`, clear `refreshtokenexpiresatutc`

## 8) Activate user

- **Method**: `PUT`
- **URL**: `api/User/Activate-User/{userId}`
- **Auth**: hien tai chua yeu cau token trong controller

### Success (200)

```json
{
  "code": 1,
  "message": "Activated successfully.",
  "data": {
    "id": 1,
    "isActivated": true
  }
}
```

### Loi chinh

- `400 BadRequest`: `userId <= 0`
- `404 NotFound`: user khong ton tai
- `500 InternalServerError`: loi he thong

### Luu y

- API set `isactive = "Y"` cho user chua bi soft delete

## Validation messages hien tai

- `Id must be greater than 0.`
- `Username is required.`
- `Email is required.`
- `Password is required.`
- `FullName is required.`
- `Role is required.`
- `RefreshToken is required.`
