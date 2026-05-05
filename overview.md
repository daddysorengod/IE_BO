# IE_BO System Overview

Tai lieu nay mo ta nhanh cac module dang duoc code/API ho tro trong backend hien tai.

## Kien truc

Solution chinh: `ImportExportBO/ImportExportBO.sln`.

Layer chinh:

- `ImportExportBO`: ASP.NET Core API host, controllers, Swagger/JWT setup.
- `Application`: service, request/response models, validation va orchestration.
- `Domain`: domain models.
- `Infrastructure`: Dapper/Npgsql repositories, QueryBuilder, JWT/password/refresh-token implementations.
- `Logger`: NLog helper project.

## Module hien co

### 1. User/Auth

Chuc nang:

- Login bang `username/password`.
- Cap access token JWT va refresh token.
- Refresh token.
- Revoke token.
- Logout.
- Register, update, delete, activate user.

API:

- `POST api/User/Login`
- `POST api/User/Refresh-Token`
- `POST api/User/Revoke-Token`
- `POST api/User/Logout/{userId}`
- `POST api/User/Register-User`
- `PUT api/User/Update-User`
- `DELETE api/User/Delete-User/{userId}`
- `PUT api/User/Activate-User/{userId}`

Bang lien quan: `public."user"`.

### 2. Product

Chuc nang:

- CRUD san pham.
- Quan ly anh san pham trong request create/update.
- Search san pham co paging.
- San pham co cac thuoc tinh: vat lieu, nam san xuat, ghi chu, chieu dai, chieu rong, chieu cao.

API:

- `GET api/Product/Get-Product/{productId}`
- `GET api/Product/Search-Product`
- `POST api/Product/Insert-Product`
- `PUT api/Product/Update-Product`
- `DELETE api/Product/Delete-Product/{productId}`

Bang lien quan: `product`, `productimage`.

### 3. Partner

Chuc nang:

- Quan ly doi tac.
- Loai doi tac hop le: `CUSTOMER`, `SUPPLIER`, `SHIPPING`.
- `CUSTOMER` duoc dung lam nguoi mua hang trong contract.

API:

- `GET api/Partner/Get-Partner/{partnerId}`
- `GET api/Partner/Search-Partner`
- `POST api/Partner/Insert-Partner`
- `PUT api/Partner/Update-Partner/{partnerId}`

Bang lien quan: `partner`.

### 4. Package

Chuc nang:

- CRUD loai kien hang/package master.
- Package trong API duoc luu vao bang DB `packinglist`.
- `packageCode` map voi `packinglist.packinglistno`.
- `packageId` trong contract la `packinglist.id`.
- Package co so luong san pham trong kien va D/R/C.

API:

- `GET api/Package/Get-Package/{packageId}`
- `GET api/Package/Search-Package`
- `POST api/Package/Insert-Package`
- `PUT api/Package/Update-Package/{packageId}`
- `DELETE api/Package/Delete-Package/{packageId}`

Bang lien quan: `packinglist`.

### 5. Contract

Chuc nang:

- Ho tro 2 loai hop dong: `IMPORT` va `EXPORT`.
- `IMPORT`: tao hop dong voi nha cung cap (`supplierId`), theo san pham, so luong, gia nhap.
- `EXPORT`: tao hop dong voi khach mua hang (`buyerId`), theo package, product, so kien, gia ban.
- `EXPORT` phai link sang 1 hop dong nhap qua `importContractNo`.
- Tao file dinh kem hop dong.

API:

- `GET api/Contract/Get-Contract/{contractId}`
- `GET api/Contract/Search-Contract`
- `POST api/Contract/Insert-Contract`
- `POST api/Contract/Insert-Import-Contract`
- `POST api/Contract/Insert-Contract-Attachment/{contractId}`

Bang lien quan: `contract`, `contractattachment`, `contractpackage`, `contractitem`, `partner`, `product`, `packinglist`.

## Database

Tai lieu database chi tiet:

- [database_ver1.md](database_ver1.md)
- [database_diagram_ver1.md](database_diagram_ver1.md)

## Validation

Build solution:

```powershell
dotnet build ImportExportBO\ImportExportBO.sln
```
