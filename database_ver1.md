# Database Ver 1

Tai lieu nay tong hop schema tu toan bo file `.sql` trong project tai thoi diem 23/04/2026.

## Nguon SQL da tong hop

- `ie_bo_setup.sql`: tao role/database, sequence va cac bang nen ban dau.
- `table_dll_20260422.sql`: DDL nen hien tai trong schema `public`.
- `table_dll_alter_update_22060422.sql`: them audit columns, tao `contractitem`, `contractattachment`, doi `product.createddate` sang `timestamp`.
- `table_dll_alter_contract_buyer_22060422.sql`: them `contract.buyerid`, cho `contract.salesorderid` nullable.
- `table_dll_alter_package_price_22060422.sql`: them `contractitem.packageid` theo luong package cu.
- `table_dll_alter_product_attributes_22060422.sql`: them thuoc tinh vat lieu, nam san xuat, ghi chu va kich thuoc cho product.
- `table_dll_alter_package_master_contractpackage_22060422.sql`: dung `packinglist` lam package master, them `contractpackage`, them `contract.note`.
- `2.databasechange.sql`: migration ad hoc cu, cac thay doi chinh da duoc bao phu boi cac migration moi hon.

## Quy uoc chung

- Schema su dung: `public`.
- Cac cot `createdby`, `createddate`, `updatedby`, `updateddate` la audit columns.
- Cot `delt` duoc dung nhu soft-delete flag o cac bang co ho tro. Gia tri `Y` thuong duoc xem la da xoa.
- Trong nghiep vu moi, `package` trong API duoc luu bang bang `packinglist`; `packinglist.id` chinh la `packageId`.
- Cac bang invoice, sales order, packing list item la legacy/nen nghiep vu, hien API moi chua khai thac het.

## Tong quan quan he chinh

- `partner` la doi tac. `partnertype = CUSTOMER` duoc dung lam khach mua hang trong `contract.buyerid`.
- `product` la danh muc san pham.
- `packinglist` la danh muc loai kien/package master trong luong moi.
- `contract` la hop dong, co nhieu `contractattachment`, nhieu `contractpackage`, nhieu `contractitem`.
- `contractpackage` la dong kien hang trong hop dong, tham chieu `contract`, `packinglist`, `product`.
- `contractitem` luu gia nhap/gia ban/tong tien theo dong kien, tham chieu `contractpackage`.

---

## 1. `partner`

### Chuc nang bang

Luu danh muc doi tac: khach hang, nha cung cap, don vi van chuyen.

### Cot

| Cot | Kieu du lieu | Null | Rang buoc | Chuc nang |
|---|---|---:|---|---|
| `id` | `int8` | No | PK, default `nextval('seq_partner')` | Khoa chinh partner |
| `partnercode` | `varchar(50)` | No | Unique | Ma doi tac |
| `partnername` | `varchar(255)` | No |  | Ten doi tac |
| `partnertype` | `varchar(20)` | No | Check `CUSTOMER`, `SUPPLIER`, `SHIPPING` | Phan loai doi tac |
| `address` | `text` | Yes |  | Dia chi |
| `phone` | `varchar(50)` | Yes |  | So dien thoai |
| `email` | `varchar(255)` | Yes |  | Email lien he |
| `isactive` | `bool` | No | Default `true` | Trang thai hoat dong |
| `createdby` | `int8` | Yes |  | User tao ban ghi |
| `createddate` | `timestamp` | Yes |  | Thoi diem tao |
| `updatedby` | `int8` | Yes |  | User cap nhat |
| `updateddate` | `timestamp` | Yes |  | Thoi diem cap nhat |

### Quan he va index

- `contract.buyerid` tham chieu `partner.id`.
- `salesorder.buyerid`, `salesorder.sellerid` tham chieu `partner.id`.
- `contractitem.supplierid` tham chieu `partner.id`, nhung hien cot nay nullable.

---

## 2. `product`

### Chuc nang bang

Luu danh muc san pham xuat khau va cac thuoc tinh mo ta san pham.

### Cot

| Cot | Kieu du lieu | Null | Rang buoc | Chuc nang |
|---|---|---:|---|---|
| `id` | `int8` | No | PK, default `nextval('seq_product')` | Khoa chinh san pham |
| `productcode` | `varchar(50)` | No | Unique, index `idxproductcode` | Ma san pham |
| `productname` | `varchar(255)` | No |  | Ten san pham |
| `hscode` | `varchar(50)` | Yes |  | Ma HS code |
| `unitofmeasure` | `varchar(50)` | No |  | Don vi tinh |
| `delt` | `varchar(1)` | Yes |  | Soft-delete flag |
| `addby` | `int8` | Yes | Legacy | User tao theo cot cu |
| `updateby` | `int8` | Yes | Legacy | User cap nhat theo cot cu |
| `createdby` | `int8` | Yes |  | User tao theo audit moi |
| `createddate` | `timestamp` | Yes | Da doi tu `date` sang `timestamp` | Thoi diem tao |
| `updatedby` | `int8` | Yes |  | User cap nhat theo audit moi |
| `updateddate` | `timestamp` | Yes |  | Thoi diem cap nhat |
| `material` | `varchar(255)` | Yes |  | Vat lieu |
| `productionyear` | `int4` | Yes |  | Nam san xuat |
| `note` | `text` | Yes |  | Ghi chu san pham |
| `length` | `numeric(18,3)` | Yes |  | Chieu dai san pham |
| `width` | `numeric(18,3)` | Yes |  | Chieu rong san pham |
| `height` | `numeric(18,3)` | Yes |  | Chieu cao san pham |

### Quan he va index

- `productimage.productid` tham chieu `product.id`.
- `salesorderitem.productid`, `packinglistitem.productid`, `contractpackage.productid`, `contractitem.productid`, invoice item tables tham chieu `product.id`.

---

## 3. `"user"`

### Chuc nang bang

Luu tai khoan dang nhap, password hash, refresh token va trang thai user.

### Cot

| Cot | Kieu du lieu | Null | Rang buoc | Chuc nang |
|---|---|---:|---|---|
| `id` | `int8` | No | PK, default `nextval('users_id_seq')` | Khoa chinh user |
| `email` | `varchar(255)` | No | Unique | Email user |
| `passwordhash` | `varchar(255)` | No |  | Mat khau da hash |
| `fullname` | `varchar(255)` | No |  | Ho ten day du |
| `imageurl` | `varchar(255)` | Yes |  | Anh dai dien |
| `role` | `varchar(50)` | No |  | Vai tro/quyen |
| `refreshtoken` | `text` | Yes |  | Refresh token hien hanh |
| `delt` | `varchar(50)` | Yes |  | Soft-delete flag |
| `isactive` | `varchar(50)` | Yes |  | Trang thai kich hoat |
| `username` | `varchar(255)` | Yes | Unique | Ten dang nhap, API login dung username/password |
| `createdby` | `int8` | Yes |  | User tao |
| `createddate` | `timestamp` | Yes |  | Thoi diem tao |
| `updatedby` | `int8` | Yes |  | User cap nhat |
| `updateddate` | `timestamp` | Yes |  | Thoi diem cap nhat |

### Ghi chu

- `2.databasechange.sql` co ban khai bao `username NOT NULL`, nhung DDL nen moi hon dang de `username NULL`. Application hien yeu cau username khi register/login.
- Refresh token expiry khong thay trong cac file SQL hien co; neu application can cot nay thi can bo sung migration rieng.

---

## 4. `productimage`

### Chuc nang bang

Luu danh sach hinh anh cua san pham.

### Cot

| Cot | Kieu du lieu | Null | Rang buoc | Chuc nang |
|---|---|---:|---|---|
| `id` | `int8` | No | PK, default `nextval('seq_productimage')` | Khoa chinh hinh anh |
| `productid` | `int8` | No | FK `product(id)` | San pham so huu hinh |
| `imageurl` | `text` | No |  | URL hinh anh |
| `ismain` | `bool` | No | Default `false` | Co phai anh chinh theo schema cu |
| `sortorder` | `int4` | No | Default `0` | Thu tu hien thi |
| `delt` | `varchar(1)` | Yes |  | Soft-delete flag |
| `isdefault` | `varchar` | Yes |  | Anh mac dinh theo convention hien tai |
| `createdby` | `int8` | Yes |  | User tao |
| `createddate` | `timestamp` | Yes |  | Thoi diem tao |
| `updatedby` | `int8` | Yes |  | User cap nhat |
| `updateddate` | `timestamp` | Yes |  | Thoi diem cap nhat |

---

## 5. `salesorder`

### Chuc nang bang

Luu don hang ban/xuat hang, la phan nghiep vu nen/legacy. Trong luong contract moi, `contract.salesorderid` da nullable.

### Cot

| Cot | Kieu du lieu | Null | Rang buoc | Chuc nang |
|---|---|---:|---|---|
| `id` | `int8` | No | PK, default `nextval('seq_salesorder')` | Khoa chinh sales order |
| `orderno` | `varchar(50)` | No | Unique, index `idxorderno` | So don hang |
| `orderdate` | `date` | No |  | Ngay don hang |
| `buyerid` | `int8` | No | FK `partner(id)` | Khach mua hang |
| `sellerid` | `int8` | No | FK `partner(id)` | Ben ban |
| `deliverylocation` | `varchar(255)` | Yes |  | Dia diem giao hang |
| `status` | `varchar(50)` | Yes |  | Trang thai don hang |
| `deletedflag` | `bool` | No | Default `false` | Flag xoa logic |
| `createdby` | `int8` | Yes |  | User tao |
| `createddate` | `timestamp` | Yes |  | Thoi diem tao |
| `updatedby` | `int8` | Yes |  | User cap nhat |
| `updateddate` | `timestamp` | Yes |  | Thoi diem cap nhat |

---

## 6. `salesorderitem`

### Chuc nang bang

Luu chi tiet san pham trong don hang.

### Cot

| Cot | Kieu du lieu | Null | Rang buoc | Chuc nang |
|---|---|---:|---|---|
| `id` | `int8` | No | PK, default `nextval('seq_salesorderitem')` | Khoa chinh dong hang |
| `salesorderid` | `int8` | No | FK `salesorder(id)` | Don hang cha |
| `productid` | `int8` | No | FK `product(id)` | San pham |
| `orderedqty` | `numeric(18,3)` | No |  | So luong dat |
| `unitpricevnd` | `numeric(18,2)` | No |  | Don gia VND |
| `lineamountvnd` | `numeric(18,2)` | No |  | Thanh tien VND |
| `createdby` | `int8` | Yes |  | User tao |
| `createddate` | `timestamp` | Yes |  | Thoi diem tao |
| `updatedby` | `int8` | Yes |  | User cap nhat |
| `updateddate` | `timestamp` | Yes |  | Thoi diem cap nhat |

---

## 7. `contract`

### Chuc nang bang

Luu thong tin hop dong xuat hang.

### Cot

| Cot | Kieu du lieu | Null | Rang buoc | Chuc nang |
|---|---|---:|---|---|
| `id` | `int8` | No | PK, default `nextval('seq_contract')` | Khoa chinh hop dong |
| `contractno` | `varchar(50)` | No | Unique | So hop dong |
| `contractdate` | `date` | No |  | Ngay hop dong |
| `salesorderid` | `int8` | Yes | FK `salesorder(id)` | Don hang lien quan, legacy nullable |
| `buyerid` | `int8` | Yes | FK `partner(id)`, index `idxcontractbuyerid` | Khach mua hang trong luong moi |
| `incoterm` | `varchar(50)` | Yes |  | Dieu kien giao hang |
| `paymentterms` | `varchar(255)` | Yes |  | Dieu khoan thanh toan |
| `shipmentdateplan` | `varchar(255)` | Yes |  | Ke hoach ngay giao hang |
| `note` | `text` | Yes |  | Ghi chu hop dong |
| `createdby` | `int8` | Yes |  | User tao |
| `createddate` | `timestamp` | Yes |  | Thoi diem tao |
| `updatedby` | `int8` | Yes |  | User cap nhat |
| `updateddate` | `timestamp` | Yes |  | Thoi diem cap nhat |

### Quan he

- Co nhieu `contractattachment`.
- Co nhieu `contractpackage`.
- Co nhieu `contractitem`.
- Duoc tham chieu boi invoice va packing/legacy tables.

---

## 8. `contractattachment`

### Chuc nang bang

Luu file dinh kem cua hop dong.

### Cot

| Cot | Kieu du lieu | Null | Rang buoc | Chuc nang |
|---|---|---:|---|---|
| `id` | `int8` | No | PK, default `nextval('seq_contractattachment')` | Khoa chinh file dinh kem |
| `contractid` | `int8` | No | FK `contract(id)`, index `idxcontractattachmentcontractid` | Hop dong cha |
| `filename` | `varchar(255)` | No |  | Ten file |
| `fileurl` | `text` | No |  | URL/path file |
| `fileextension` | `varchar(50)` | Yes |  | Phan mo rong file |
| `createdby` | `int8` | Yes |  | User tao |
| `createddate` | `timestamp` | Yes |  | Thoi diem tao |
| `updatedby` | `int8` | Yes |  | User cap nhat |
| `updateddate` | `timestamp` | Yes |  | Thoi diem cap nhat |

---

## 9. `packinglist`

### Chuc nang bang

Trong schema goc, bang nay luu packing list theo hop dong. Trong luong moi, bang nay duoc dung lam package master/danh muc loai kien hang.

### Cot

| Cot | Kieu du lieu | Null | Rang buoc | Chuc nang |
|---|---|---:|---|---|
| `id` | `int8` | No | PK, default `nextval('seq_packinglist')` | Khoa chinh package/packing list |
| `packinglistno` | `varchar(50)` | No | Unique | Ma package, map voi `packageCode` API |
| `contractid` | `int8` | Yes | FK `contract(id)` | Hop dong cha theo luong legacy; nullable de lam package master |
| `packagename` | `varchar(255)` | No | Index `idxpackinglistpackagename` | Ten loai kien/package |
| `productquantity` | `numeric(18,3)` | No | Default `1` | So luong san pham trong 1 kien |
| `containerno` | `varchar(100)` | Yes | Legacy | So container |
| `totalctns` | `numeric(18,3)` | Yes | Legacy | Tong so carton |
| `totalcbm` | `numeric(18,3)` | Yes | Legacy | Tong CBM |
| `length` | `numeric` | Yes |  | Chieu dai kien |
| `width` | `numeric` | Yes |  | Chieu rong kien |
| `height` | `numeric` | Yes |  | Chieu cao kien |
| `note` | `text` | Yes |  | Ghi chu package |
| `delt` | `varchar(1)` | Yes |  | Soft-delete flag |
| `createdby` | `int8` | Yes |  | User tao |
| `createddate` | `timestamp` | Yes |  | Thoi diem tao |
| `updatedby` | `int8` | Yes |  | User cap nhat |
| `updateddate` | `timestamp` | Yes |  | Thoi diem cap nhat |

### Quan he

- `contractpackage.packageid` tham chieu `packinglist.id`.
- `contractitem.packageid` tham chieu `packinglist.id`.
- `packinglistitem.packinglistid` tham chieu `packinglist.id` trong luong legacy.

---

## 10. `packinglistitem`

### Chuc nang bang

Bang legacy luu san pham trong packing list. Luong package/contract moi khong dung bang nay de tao contract.

### Cot

| Cot | Kieu du lieu | Null | Rang buoc | Chuc nang |
|---|---|---:|---|---|
| `id` | `int8` | No | PK, default `nextval('seq_packinglistitem')` | Khoa chinh dong packing |
| `packinglistid` | `int8` | No | FK `packinglist(id)` | Packing list/package cha |
| `productid` | `int8` | No | FK `product(id)` | San pham |
| `totalunits` | `numeric(18,3)` | No |  | Tong so don vi san pham |
| `totalctns` | `numeric(18,3)` | Yes |  | Tong carton |
| `totalcbm` | `numeric(18,3)` | Yes |  | Tong CBM |
| `createdby` | `int8` | Yes |  | User tao |
| `createddate` | `timestamp` | Yes |  | Thoi diem tao |
| `updatedby` | `int8` | Yes |  | User cap nhat |
| `updateddate` | `timestamp` | Yes |  | Thoi diem cap nhat |

---

## 11. `contractpackage`

### Chuc nang bang

Luu cac dong kien hang trong hop dong theo luong moi. Moi dong chon mot package master (`packinglist`) va mot product.

### Cot

| Cot | Kieu du lieu | Null | Rang buoc | Chuc nang |
|---|---|---:|---|---|
| `id` | `int8` | No | PK, default `nextval('seq_contractpackage')` | Khoa chinh dong kien |
| `contractid` | `int8` | No | FK `contract(id)`, index `idxcontractpackagecontractid` | Hop dong cha |
| `packageid` | `int8` | No | FK `packinglist(id)`, index `idxcontractpackagepackageid` | Loai kien/package master |
| `productid` | `int8` | No | FK `product(id)`, index `idxcontractpackageproductid` | San pham trong dong kien |
| `packagequantity` | `numeric(18,3)` | No | Default `1` | So kien trong hop dong |
| `note` | `text` | Yes |  | Ghi chu dong kien |
| `createdby` | `int8` | Yes |  | User tao |
| `createddate` | `timestamp` | Yes |  | Thoi diem tao |
| `updatedby` | `int8` | Yes |  | User cap nhat |
| `updateddate` | `timestamp` | Yes |  | Thoi diem cap nhat |

### Ghi chu nghiep vu

- So luong san pham thuc te duoc tinh: `packinglist.productquantity * contractpackage.packagequantity`.

---

## 12. `contractitem`

### Chuc nang bang

Luu gia nhap/gia ban va tong tien theo san pham/dong kien trong hop dong.

### Cot

| Cot | Kieu du lieu | Null | Rang buoc | Chuc nang |
|---|---|---:|---|---|
| `id` | `int8` | No | PK, default `nextval('seq_contractitem')` | Khoa chinh contract item |
| `contractid` | `int8` | No | FK `contract(id)`, index `idxcontractitemcontractid` | Hop dong cha |
| `contractpackageid` | `int8` | Yes | FK `contractpackage(id)`, index `idxcontractitemcontractpackageid` | Dong kien tuong ung |
| `packageid` | `int8` | Yes | FK `packinglist(id)`, index `idxcontractitempackageid` | Package master |
| `productid` | `int8` | No | FK `product(id)`, index `idxcontractitemproductid` | San pham |
| `supplierid` | `int8` | Yes | FK `partner(id)`, index `idxcontractitemsupplierid` | Nha cung cap, hien optional |
| `quantity` | `numeric(18,3)` | No |  | So luong san pham |
| `importprice` | `numeric(18,2)` | No |  | Gia nhap tren dau san pham |
| `sellprice` | `numeric(18,2)` | No |  | Gia ban tren dau san pham |
| `totalimportamount` | `numeric(18,2)` | No |  | Tong tien nhap |
| `totalsellamount` | `numeric(18,2)` | No |  | Tong tien ban |
| `createdby` | `int8` | Yes |  | User tao |
| `createddate` | `timestamp` | Yes |  | Thoi diem tao |
| `updatedby` | `int8` | Yes |  | User cap nhat |
| `updateddate` | `timestamp` | Yes |  | Thoi diem cap nhat |

### Ghi chu nghiep vu

- `totalimportamount = quantity * importprice`.
- `totalsellamount = quantity * sellprice`.
- `supplierid` ban dau `NOT NULL`, migration moi da `DROP NOT NULL`.

---

## 13. `proformainvoice`

### Chuc nang bang

Luu proforma invoice cua hop dong.

### Cot

| Cot | Kieu du lieu | Null | Rang buoc | Chuc nang |
|---|---|---:|---|---|
| `id` | `int8` | No | PK, default `nextval('seq_proformainvoice')` | Khoa chinh PI |
| `pino` | `varchar(50)` | No | Unique | So proforma invoice |
| `contractid` | `int8` | No | FK `contract(id)` | Hop dong lien quan |
| `pidate` | `date` | No |  | Ngay PI |
| `currencycode` | `varchar(10)` | No |  | Ma tien te |
| `createdby` | `int8` | Yes |  | User tao |
| `createddate` | `timestamp` | Yes |  | Thoi diem tao |
| `updatedby` | `int8` | Yes |  | User cap nhat |
| `updateddate` | `timestamp` | Yes |  | Thoi diem cap nhat |

---

## 14. `proformainvoiceitem`

### Chuc nang bang

Luu chi tiet san pham trong proforma invoice.

### Cot

| Cot | Kieu du lieu | Null | Rang buoc | Chuc nang |
|---|---|---:|---|---|
| `id` | `int8` | No | PK, default `nextval('seq_proformainvoiceitem')` | Khoa chinh dong PI |
| `proformainvoiceid` | `int8` | No | FK `proformainvoice(id)` | PI cha |
| `productid` | `int8` | No | FK `product(id)` | San pham |
| `quantity` | `numeric(18,3)` | No |  | So luong |
| `unitprice` | `numeric(18,2)` | No |  | Don gia |
| `amount` | `numeric(18,2)` | No |  | Thanh tien |
| `createdby` | `int8` | Yes |  | User tao |
| `createddate` | `timestamp` | Yes |  | Thoi diem tao |
| `updatedby` | `int8` | Yes |  | User cap nhat |
| `updateddate` | `timestamp` | Yes |  | Thoi diem cap nhat |

---

## 15. `commercialinvoice`

### Chuc nang bang

Luu commercial invoice cua hop dong.

### Cot

| Cot | Kieu du lieu | Null | Rang buoc | Chuc nang |
|---|---|---:|---|---|
| `id` | `int8` | No | PK, default `nextval('seq_commercialinvoice')` | Khoa chinh invoice |
| `invoiceno` | `varchar(50)` | No | Unique, index `idxinvoiceno` | So invoice |
| `contractid` | `int8` | No | FK `contract(id)` | Hop dong lien quan |
| `totalamount` | `numeric(18,2)` | No |  | Tong tien invoice |
| `depositamount` | `numeric(18,2)` | Yes |  | So tien dat coc |
| `balanceamount` | `numeric(18,2)` | Yes |  | So tien con lai |
| `delt` | `varchar(1)` | Yes |  | Soft-delete flag |
| `createdby` | `int8` | Yes |  | User tao |
| `createddate` | `timestamp` | Yes |  | Thoi diem tao |
| `updatedby` | `int8` | Yes |  | User cap nhat |
| `updateddate` | `timestamp` | Yes |  | Thoi diem cap nhat |

---

## 16. `commercialinvoiceitem`

### Chuc nang bang

Luu chi tiet san pham trong commercial invoice.

### Cot

| Cot | Kieu du lieu | Null | Rang buoc | Chuc nang |
|---|---|---:|---|---|
| `id` | `int8` | No | PK, default `nextval('seq_commercialinvoiceitem')` | Khoa chinh dong invoice |
| `commercialinvoiceid` | `int8` | No | FK `commercialinvoice(id)` | Invoice cha |
| `productid` | `int8` | No | FK `product(id)` | San pham |
| `quantity` | `numeric(18,3)` | No |  | So luong |
| `unitprice` | `numeric(18,2)` | No |  | Don gia |
| `amount` | `numeric(18,2)` | No |  | Thanh tien |
| `createdby` | `int8` | Yes |  | User tao |
| `createddate` | `timestamp` | Yes |  | Thoi diem tao |
| `updatedby` | `int8` | Yes |  | User cap nhat |
| `updateddate` | `timestamp` | Yes |  | Thoi diem cap nhat |

---

## 17. `payment`

### Chuc nang bang

Luu cac lan thanh toan theo commercial invoice.

### Cot

| Cot | Kieu du lieu | Null | Rang buoc | Chuc nang |
|---|---|---:|---|---|
| `id` | `int8` | No | PK, default `nextval('seq_payment')` | Khoa chinh payment |
| `commercialinvoiceid` | `int8` | No | FK `commercialinvoice(id)` | Commercial invoice lien quan |
| `paymentdate` | `date` | No |  | Ngay thanh toan |
| `amount` | `numeric(18,2)` | No |  | So tien thanh toan |
| `paymenttype` | `varchar(50)` | Yes |  | Hinh thuc/loai thanh toan |
| `createdby` | `int8` | Yes |  | User tao |
| `createddate` | `timestamp` | Yes |  | Thoi diem tao |
| `updatedby` | `int8` | Yes |  | User cap nhat |
| `updateddate` | `timestamp` | Yes |  | Thoi diem cap nhat |

---

## Sequence chinh

| Sequence | Bang su dung |
|---|---|
| `seq_partner` | `partner.id` |
| `seq_product` | `product.id` |
| `seq_productimage` | `productimage.id` |
| `users_id_seq` | `"user".id` |
| `seq_salesorder` | `salesorder.id` |
| `seq_salesorderitem` | `salesorderitem.id` |
| `seq_contract` | `contract.id` |
| `seq_contractattachment` | `contractattachment.id` |
| `seq_contractpackage` | `contractpackage.id` |
| `seq_contractitem` | `contractitem.id` |
| `seq_packinglist` | `packinglist.id` |
| `seq_packinglistitem` | `packinglistitem.id` |
| `seq_proformainvoice` | `proformainvoice.id` | 
| `seq_proformainvoiceitem` | `proformainvoiceitem.id` |
| `seq_commercialinvoice` | `commercialinvoice.id` |
| `seq_commercialinvoiceitem` | `commercialinvoiceitem.id` |
| `seq_payment` | `payment.id` |

## Luu y phien ban

- Tai lieu nay mo ta schema hieu luc theo cac file SQL trong source, khong phai ket qua introspect truc tiep tu database runtime.
- Neu database da tung apply migration cu tao bang/constraint ngoai cac file tren, can so sanh lai bang `information_schema` va `pg_constraint`.
- Trong luong API hien tai, package CRUD doc/ghi `packinglist`, contract create ghi `contractpackage` va `contractitem`.
