# Database Schema Summary 22/04/2026

Tai lieu nay tong hop cac file SQL hien co trong project de lam can cu cho luong package/contract moi.

## File SQL da tong hop

- `ie_bo_setup.sql`: setup ban dau cho partner, product, sales order, contract, invoice, packing list va payment.
- `table_dll_20260422.sql`: DDL nen hien tai cho cac bang chinh.
- `table_dll_alter_update_22060422.sql`: audit columns, doi `product.createddate` sang timestamp, tao `contractitem`, `contractattachment`.
- `table_dll_alter_contract_buyer_22060422.sql`: them `contract.buyerid`, cho phep `salesorderid` nullable.
- `table_dll_alter_package_price_22060422.sql`: them `contractitem.packageid` theo luong packinglist cu.
- `table_dll_alter_product_attributes_22060422.sql`: them vat lieu, nam san xuat, ghi chu va kich thuoc cho product.
- `table_dll_alter_package_master_contractpackage_22060422.sql`: bo sung `packinglist` de dong vai tro package master, tao contract package line va them note cho contract.
- `2.databasechange.sql`: migration ad hoc cu, da duoc bao phu boi cac migration moi hon.

## Schema hieu luc sau cac migration

### Master data

- `partner`: doi tac, dung `partnertype = CUSTOMER` cho nguoi mua hang.
- `product`: danh muc san pham, co them `material`, `productionyear`, `note`, `length`, `width`, `height`.
- `packinglist`: danh muc loai kien hang/package master. `packinglistno` duoc dung nhu `packageCode`, bo sung `packagename`, `productquantity`, `note`, `delt`, D/R/C va audit columns.
- `"user"`: user dang nhap, login theo `username/password`.

### Contract

- `contract`: hop dong xuat hang, co `buyerid`, `note`, audit columns; `salesorderid` la legacy nullable.
- `contractattachment`: file dinh kem hop dong.
- `contractpackage`: dong kien hang cua hop dong, tham chieu `contract`, `packinglist`, `product`, co `packagequantity` va `note`.
- `contractitem`: gia va tong tien theo dong kien, tham chieu `contract`, `contractpackage`, `packinglist`, `product`; `supplierid` nullable cho nhu cau sau.

### Legacy/ke thua

- `packinglistitem`: bang cu cua luong package item theo hop dong, khong dung trong API contract/package moi.
- `salesorder`, `salesorderitem`, invoice va payment: giu nguyen theo schema nen hien co.

## Quy uoc workflow moi

- Khai bao package bang API CRUD rieng truoc khi tao hop dong; API nay doc/ghi vao `packinglist`.
- Khi tao hop dong, client gui danh sach `packages[]` la cac dong kien hang, moi dong co `packageId`, `productId`, `packageQuantity`, `importPrice`, `sellPrice`, `note`.
- Backend tinh:
  - `quantity = package.productquantity * packageQuantity`
  - `totalimportamount = quantity * importPrice`
  - `totalsellamount = quantity * sellPrice`
- Contract detail tra kem D/R/C cua package de UI hien thi khi chon loai kien.
