# Ke Hoach Phat Trien Module Quan Ly Hop Dong Va Kien Hang

Cap nhat ngay: 22/04/2026

Tai lieu nay mo ta workflow moi cua module Export Order Management sau khi tach ro `package` thanh danh muc loai kien hang CRUD rieng, tuong tu nhu `product`. Trong DB hien co, package master duoc luu bang `packinglist`. Migration chinh cho thay doi nay la `table_dll_alter_package_master_contractpackage_22060422.sql`.

## 1. Pham Vi Nghiep Vu

- Quan ly doi tac/khach hang trong `partner`.
- Quan ly san pham trong `product`.
- Quan ly loai kien hang trong `package` theo CRUD rieng, tuong tu nhu san pham.
- Quan ly hop dong xuat hang trong `contract`, trong do `buyerid` la khach mua hang va tham chieu den `partner` co `partnertype = 'CUSTOMER'`.
- Khi tao hop dong, nguoi dung chon khach hang, chon san pham, chon loai kien, nhap gia nhap/gia ban va ghi chu hop dong.
- Mot hop dong co nhieu dong kien hang. Moi dong kien hang chon 1 loai kien va 1 san pham.
- Khi chon loai kien hang tren man hinh hop dong, giao dien phai hien thi thong so D/R/C ben canh: chieu dai kien, chieu rong kien, chieu cao kien.
- Login su dung `username` va `password`, khong dung `email` va `password`.

## 2. Khai Niem Du Lieu Chinh

### 2.1 Partner

`partner` la danh muc doi tac:

- `CUSTOMER`: nguoi mua hang, duoc dung lam `buyerId` trong hop dong.
- `SUPPLIER`: nha cung cap/xuong san xuat, duoc dung lam `supplierId` neu can quan ly nguon hang.
- `SHIPPING`: doi tac van chuyen.

### 2.2 Product

`product` la danh muc san pham. Ngoai cac truong hien co, product can co them:

- `material`: vat lieu.
- `productionyear`: nam san xuat.
- `note`: ghi chu.
- `length`: chieu dai san pham.
- `width`: chieu rong san pham.
- `height`: chieu cao san pham.

### 2.3 Package

`package` la danh muc loai kien hang, duoc khai bao CRUD rieng giong nhu san pham. Ve vat ly DB, du lieu package luu trong bang `packinglist`.

Thong tin quan trong cua package:

- Ma loai kien.
- Ten loai kien.
- So luong san pham trong kien.
- Chieu dai kien.
- Chieu rong kien.
- Chieu cao kien.
- Ghi chu neu co.
- Audit fields: `createdby`, `createddate`, `updatedby`, `updateddate`.

Luu y: Package o day la "loai kien hang" dung de chon khi tao hop dong, khong phai mot kien phat sinh truc tiep trong hop dong.

### 2.4 Contract

`contract` la hop dong xuat hang.

Thong tin hop dong gom:

- So hop dong.
- Ngay hop dong.
- Khach hang mua hang (`buyerId`).
- Incoterm.
- Payment terms.
- Shipment date plan.
- Ghi chu hop dong.
- File dinh kem neu co.
- Danh sach dong kien hang cua hop dong.

Mot hop dong co nhieu dong kien hang. Moi dong kien hang gom:

- Loai kien hang (`packageId`).
- Ma san pham (`productId`).
- Gia nhap tren dau san pham (`importPrice`).
- Gia ban tren dau san pham (`sellPrice`).
- So luong kien neu can tinh tong.
- Ghi chu dong hang neu co.

Khi chon `packageId`, API/UI can lay thong tin D/R/C cua package de hien thi ben canh:

- Chieu dai kien.
- Chieu rong kien.
- Chieu cao kien.

## 3. Database Va Schema Dinh Huong

### 3.1 Audit columns

Tat ca bang nghiep vu can co audit columns:

- `createdby int8 NULL`
- `createddate timestamp NULL`
- `updatedby int8 NULL`
- `updateddate timestamp NULL`

### 3.2 Product attributes

`public.product` can co them cac cot:

- `material varchar(255) NULL`
- `productionyear int4 NULL`
- `note text NULL`
- `length numeric(18, 3) NULL`
- `width numeric(18, 3) NULL`
- `height numeric(18, 3) NULL`

### 3.3 Package master

Dung bang hien co `public.packinglist` lam danh muc loai kien hang/package master.

Cot su dung/bo sung:

- `id int8` primary key.
- `packinglistno varchar(50) NOT NULL UNIQUE`: ma loai kien, map voi `packageCode` trong API.
- `contractid int8 NULL`: nullable de package master khong bat buoc thuoc contract.
- `packagename varchar(255) NOT NULL`.
- `productquantity numeric(18, 3) NOT NULL`: so luong san pham trong 1 kien.
- `length numeric(18, 3) NULL`.
- `width numeric(18, 3) NULL`.
- `height numeric(18, 3) NULL`.
- `note text NULL`.
- `delt varchar(1) NULL`: soft delete neu can dong bo voi product.
- Audit columns.

### 3.4 Contract package lines

Can co bang luu cac dong kien hang trong hop dong. Luong moi dung `contractpackage`; `packinglist` va `packinglistitem` giu lai nhu bang legacy.

Thiet ke de de doc va tach nghiep vu:

- `contractpackage`: dong kien hang cua hop dong.
- `packinglist`: danh muc loai kien.
- `contractitem`: thong tin gia theo san pham trong dong kien.

Cot de xuat cho `contractpackage`:

- `id int8` primary key.
- `contractid int8 NOT NULL`.
- `packageid int8 NOT NULL`: tham chieu danh muc `packinglist`.
- `productid int8 NOT NULL`: san pham trong dong kien.
- `packagequantity numeric(18, 3) NULL`: so kien trong hop dong neu can.
- `note text NULL`.
- Audit columns.

Cot de xuat cho `contractitem`:

- `id int8` primary key.
- `contractid int8 NOT NULL`.
- `contractpackageid int8 NOT NULL`: tham chieu dong kien hang trong hop dong.
- `packageid int8 NOT NULL`: tham chieu `packinglist`.
- `productid int8 NOT NULL`.
- `supplierid int8 NULL`: neu can quan ly nha cung cap.
- `quantity numeric(18, 3) NOT NULL`: so luong san pham tinh theo dong kien.
- `importprice numeric(18, 2) NOT NULL`.
- `sellprice numeric(18, 2) NOT NULL`.
- `totalimportamount numeric(18, 2) NOT NULL`.
- `totalsellamount numeric(18, 2) NOT NULL`.
- Audit columns.

Cong thuc de xuat:

- `quantity = package.productquantity * contractpackage.packagequantity` neu co so kien.
- `totalimportamount = quantity * importprice`.
- `totalsellamount = quantity * sellprice`.

## 4. Workflow Chinh

### 4.1 Khai bao master data

Truoc khi tao hop dong, nguoi dung khai bao cac danh muc:

1. Tao partner khach hang (`CUSTOMER`).
2. Tao san pham trong Product CRUD.
3. Tao loai kien hang trong Package CRUD.

Package CRUD phai co cac thao tac tuong tu Product:

- Get by id.
- Search/list.
- Create.
- Update.
- Delete/soft delete.

### 4.2 Tao hop dong

Khi tao hop dong:

1. Client chon khach hang.
2. Client nhap thong tin hop dong va ghi chu hop dong.
3. Client bam "Them kien hang".
4. Tren dong kien hang, client chon loai kien (`packageId`).
5. Sau khi chon loai kien, UI hien thi D/R/C cua loai kien ben canh.
6. Client chon ma san pham (`productId`).
7. Client nhap gia nhap (`importPrice`) va gia ban (`sellPrice`).
8. Client nhap so kien (`packageQuantity`) neu can tinh tong so san pham.
9. Client submit hop dong.
10. Backend tao hop dong va cac dong kien hang trong cung transaction.

### 4.3 Tra cuu hop dong

Man hinh chi tiet hop dong hien thi:

- Thong tin hop dong.
- Thong tin khach hang.
- Danh sach file dinh kem.
- Danh sach dong kien hang.
- Voi moi dong kien hang: loai kien, D/R/C, san pham, so luong san pham trong kien, gia nhap, gia ban, tong tien nhap, tong tien ban.

## 5. API Contract Dinh Huong

Tat ca API nghiep vu partner/contract/package yeu cau JWT Bearer token. Backend doc `ClaimTypes.NameIdentifier` de gan audit fields.

### 5.1 Package CRUD

Base route theo ProductController convention: `api/Package`

#### Create package

`POST api/Package/Insert-Package`

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

#### Update package

`PUT api/Package/Update-Package/{packageId}`

Body tuong tu create package.

#### Get/search package

- `GET api/Package/Get-Package/{packageId}`
- `GET api/Package/Search-Package?keyword=&page=1&pageSize=20`

#### Delete package

`DELETE api/Package/Delete-Package/{packageId}`

### 5.2 Contract create

`POST api/Contract/Insert-Contract`

```json
{
  "contractNo": "HD001",
  "contractDate": "2026-04-22",
  "buyerId": 1,
  "incoterm": "FOB",
  "paymentTerms": "30% deposit",
  "shipmentDatePlan": "May 2026",
  "note": "Ghi chu hop dong",
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
      "importPrice": 5.5,
      "sellPrice": 8.0,
      "note": "Dong kien hang 1"
    }
  ]
}
```

Rules:

- `buyerId` phai tham chieu partner loai `CUSTOMER`.
- `packageId` phai tham chieu loai kien hang da khai bao.
- `productId` phai tham chieu san pham da khai bao.
- `importPrice` va `sellPrice` phai `>= 0`.
- `packageQuantity` neu co thi phai `> 0`.
- Khi tra detail contract, moi dong package can tra kem thong so D/R/C lay tu danh muc package.

## 6. Code Architecture

### 6.1 Layering

- `ImportExportBO`: controller va response envelope.
- `Application`: service, request/response models, validation va orchestration.
- `Domain`: domain objects.
- `Infrastructure`: Dapper repositories, PostgreSQL mapping, `IQueryBuilder`.

### 6.2 Repository SQL

Repository uu tien dung `Infrastructure.Persistence.Querying.IQueryBuilder<T, TResult>` cho:

- `SELECT` don gian.
- `INSERT`.
- `UPDATE`.
- Query co `JOIN`.
- Search data query va count query khi co the dien dat an toan.

Chi giu raw SQL cho truong hop `IQueryBuilder` hien tai khong dien dat sach, vi du:

- `SELECT EXISTS (...)`.
- Normalize bang `public."user"` do `user` la keyword cua PostgreSQL.

### 6.3 Transaction boundary

Create aggregate phai chay trong transaction:

- Create contract: `contract` + attachments + contract package lines + contract items.
- Package CRUD la master data CRUD rieng, khong nam trong transaction tao contract tru khi tao hop dong.

Neu mot dong kien hang loi validation hoac insert loi, rollback toan bo contract aggregate. API moi khong con endpoint AddItem rieng cho package/contract.

## 7. Validation

Build solution:

```powershell
dotnet build ImportExportBO\ImportExportBO.sln
```

Smoke test toi thieu:

- Login bang username/password.
- Tao partner `CUSTOMER`.
- Tao san pham.
- Tao loai kien hang qua Package CRUD.
- Tao hop dong voi `buyerId`, attachments va danh sach dong kien hang.
- Verify khi chon `packageId` co the lay va hien thi D/R/C.
- Verify contract detail tra ve loai kien, D/R/C, product, gia nhap, gia ban va tong tien.
- Chay search contract/package/partner va verify paging/count.
