# Database Diagram Ver 1

Tai lieu nay mo hinh hoa database dua tren `database_ver1.md`.

## 1. So do tong quan module

```mermaid
flowchart LR
    USER_ACCOUNT["user<br/>Tai khoan dang nhap"]

    subgraph MASTER["Master data"]
        PARTNER["partner<br/>Doi tac"]
        PRODUCT["product<br/>San pham"]
        PRODUCTIMAGE["productimage<br/>Hinh anh san pham"]
        PACKINGLIST["packinglist<br/>Package master / loai kien"]
    end

    subgraph ORDER["Order legacy"]
        SALESORDER["salesorder<br/>Don hang"]
        SALESORDERITEM["salesorderitem<br/>Dong don hang"]
        PACKINGLISTITEM["packinglistitem<br/>Dong packing legacy"]
    end

    subgraph CONTRACTS["Contract workflow"]
        CONTRACT["contract<br/>Hop dong"]
        CONTRACTATTACHMENT["contractattachment<br/>File dinh kem"]
        CONTRACTPACKAGE["contractpackage<br/>Dong kien trong hop dong"]
        CONTRACTITEM["contractitem<br/>Gia va tong tien"]
    end

    subgraph INVOICE["Invoice & payment"]
        PROFORMAINVOICE["proformainvoice<br/>PI"]
        PROFORMAINVOICEITEM["proformainvoiceitem<br/>Dong PI"]
        COMMERCIALINVOICE["commercialinvoice<br/>CI"]
        COMMERCIALINVOICEITEM["commercialinvoiceitem<br/>Dong CI"]
        PAYMENT["payment<br/>Thanh toan"]
    end

    PRODUCT --> PRODUCTIMAGE
    PARTNER --> SALESORDER
    PRODUCT --> SALESORDERITEM
    SALESORDER --> SALESORDERITEM
    SALESORDER -. legacy nullable .-> CONTRACT

    PARTNER --> CONTRACT
    CONTRACT --> CONTRACTATTACHMENT
    CONTRACT --> CONTRACTPACKAGE
    PACKINGLIST --> CONTRACTPACKAGE
    PRODUCT --> CONTRACTPACKAGE
    CONTRACTPACKAGE --> CONTRACTITEM
    CONTRACT --> CONTRACTITEM
    PACKINGLIST --> CONTRACTITEM
    PRODUCT --> CONTRACTITEM

    CONTRACT --> PROFORMAINVOICE
    PROFORMAINVOICE --> PROFORMAINVOICEITEM
    PRODUCT --> PROFORMAINVOICEITEM
    CONTRACT --> COMMERCIALINVOICE
    COMMERCIALINVOICE --> COMMERCIALINVOICEITEM
    PRODUCT --> COMMERCIALINVOICEITEM
    COMMERCIALINVOICE --> PAYMENT

    PACKINGLIST --> PACKINGLISTITEM
    PRODUCT --> PACKINGLISTITEM
```

## 2. ERD chi tiet

```mermaid
erDiagram
    PARTNER {
        int8 id PK
        varchar partnercode UK
        varchar partnername
        varchar partnertype
        text address
        varchar phone
        varchar email
        bool isactive
        int8 createdby
        timestamp createddate
        int8 updatedby
        timestamp updateddate
    }

    PRODUCT {
        int8 id PK
        varchar productcode UK
        varchar productname
        varchar hscode
        varchar unitofmeasure
        varchar delt
        int8 addby
        int8 updateby
        int8 createdby
        timestamp createddate
        int8 updatedby
        timestamp updateddate
        varchar material
        int4 productionyear
        text note
        numeric length
        numeric width
        numeric height
    }

    USER_ACCOUNT {
        int8 id PK
        varchar username UK
        varchar email UK
        varchar passwordhash
        varchar fullname
        varchar imageurl
        varchar role
        text refreshtoken
        varchar delt
        varchar isactive
        int8 createdby
        timestamp createddate
        int8 updatedby
        timestamp updateddate
    }

    PRODUCTIMAGE {
        int8 id PK
        int8 productid FK
        text imageurl
        bool ismain
        int4 sortorder
        varchar delt
        varchar isdefault
        int8 createdby
        timestamp createddate
        int8 updatedby
        timestamp updateddate
    }

    SALESORDER {
        int8 id PK
        varchar orderno UK
        date orderdate
        int8 buyerid FK
        int8 sellerid FK
        varchar deliverylocation
        varchar status
        bool deletedflag
        int8 createdby
        timestamp createddate
        int8 updatedby
        timestamp updateddate
    }

    SALESORDERITEM {
        int8 id PK
        int8 salesorderid FK
        int8 productid FK
        numeric orderedqty
        numeric unitpricevnd
        numeric lineamountvnd
        int8 createdby
        timestamp createddate
        int8 updatedby
        timestamp updateddate
    }

    CONTRACT {
        int8 id PK
        varchar contractno UK
        date contractdate
        int8 salesorderid FK
        int8 buyerid FK
        varchar incoterm
        varchar paymentterms
        varchar shipmentdateplan
        text note
        int8 createdby
        timestamp createddate
        int8 updatedby
        timestamp updateddate
    }

    CONTRACTATTACHMENT {
        int8 id PK
        int8 contractid FK
        varchar filename
        text fileurl
        varchar fileextension
        int8 createdby
        timestamp createddate
        int8 updatedby
        timestamp updateddate
    }

    PACKINGLIST {
        int8 id PK
        varchar packinglistno UK
        int8 contractid FK
        varchar packagename
        numeric productquantity
        varchar containerno
        numeric totalctns
        numeric totalcbm
        numeric length
        numeric width
        numeric height
        text note
        varchar delt
        int8 createdby
        timestamp createddate
        int8 updatedby
        timestamp updateddate
    }

    PACKINGLISTITEM {
        int8 id PK
        int8 packinglistid FK
        int8 productid FK
        numeric totalunits
        numeric totalctns
        numeric totalcbm
        int8 createdby
        timestamp createddate
        int8 updatedby
        timestamp updateddate
    }

    CONTRACTPACKAGE {
        int8 id PK
        int8 contractid FK
        int8 packageid FK
        int8 productid FK
        numeric packagequantity
        text note
        int8 createdby
        timestamp createddate
        int8 updatedby
        timestamp updateddate
    }

    CONTRACTITEM {
        int8 id PK
        int8 contractid FK
        int8 contractpackageid FK
        int8 packageid FK
        int8 productid FK
        int8 supplierid FK
        numeric quantity
        numeric importprice
        numeric sellprice
        numeric totalimportamount
        numeric totalsellamount
        int8 createdby
        timestamp createddate
        int8 updatedby
        timestamp updateddate
    }

    PROFORMAINVOICE {
        int8 id PK
        varchar pino UK
        int8 contractid FK
        date pidate
        varchar currencycode
        int8 createdby
        timestamp createddate
        int8 updatedby
        timestamp updateddate
    }

    PROFORMAINVOICEITEM {
        int8 id PK
        int8 proformainvoiceid FK
        int8 productid FK
        numeric quantity
        numeric unitprice
        numeric amount
        int8 createdby
        timestamp createddate
        int8 updatedby
        timestamp updateddate
    }

    COMMERCIALINVOICE {
        int8 id PK
        varchar invoiceno UK
        int8 contractid FK
        numeric totalamount
        numeric depositamount
        numeric balanceamount
        varchar delt
        int8 createdby
        timestamp createddate
        int8 updatedby
        timestamp updateddate
    }

    COMMERCIALINVOICEITEM {
        int8 id PK
        int8 commercialinvoiceid FK
        int8 productid FK
        numeric quantity
        numeric unitprice
        numeric amount
        int8 createdby
        timestamp createddate
        int8 updatedby
        timestamp updateddate
    }

    PAYMENT {
        int8 id PK
        int8 commercialinvoiceid FK
        date paymentdate
        numeric amount
        varchar paymenttype
        int8 createdby
        timestamp createddate
        int8 updatedby
        timestamp updateddate
    }

    PARTNER ||--o{ SALESORDER : buyerid
    PARTNER ||--o{ SALESORDER : sellerid
    PARTNER ||--o{ CONTRACT : buyerid
    PARTNER ||--o{ CONTRACTITEM : supplierid

    PRODUCT ||--o{ PRODUCTIMAGE : productid
    PRODUCT ||--o{ SALESORDERITEM : productid
    PRODUCT ||--o{ PACKINGLISTITEM : productid
    PRODUCT ||--o{ CONTRACTPACKAGE : productid
    PRODUCT ||--o{ CONTRACTITEM : productid
    PRODUCT ||--o{ PROFORMAINVOICEITEM : productid
    PRODUCT ||--o{ COMMERCIALINVOICEITEM : productid

    SALESORDER ||--o{ SALESORDERITEM : salesorderid
    SALESORDER ||--o{ CONTRACT : salesorderid

    CONTRACT ||--o{ CONTRACTATTACHMENT : contractid
    CONTRACT ||--o{ PACKINGLIST : contractid_legacy
    CONTRACT ||--o{ CONTRACTPACKAGE : contractid
    CONTRACT ||--o{ CONTRACTITEM : contractid
    CONTRACT ||--o{ PROFORMAINVOICE : contractid
    CONTRACT ||--o{ COMMERCIALINVOICE : contractid

    PACKINGLIST ||--o{ PACKINGLISTITEM : packinglistid
    PACKINGLIST ||--o{ CONTRACTPACKAGE : packageid
    PACKINGLIST ||--o{ CONTRACTITEM : packageid

    CONTRACTPACKAGE ||--o| CONTRACTITEM : contractpackageid

    PROFORMAINVOICE ||--o{ PROFORMAINVOICEITEM : proformainvoiceid
    COMMERCIALINVOICE ||--o{ COMMERCIALINVOICEITEM : commercialinvoiceid
    COMMERCIALINVOICE ||--o{ PAYMENT : commercialinvoiceid
```

## 3. Ghi chu doc so do

- `USER_ACCOUNT` dai dien cho bang SQL `public."user"` vi `user` la keyword/ten dac biet.
- `PACKINGLIST` duoc dung nhu package master trong luong API hien tai.
- `PACKINGLISTITEM`, `SALESORDER`, `SALESORDERITEM` la cac phan legacy/nen nghiep vu.
- `CONTRACTPACKAGE` la dong kien hang trong hop dong.
- `CONTRACTITEM` la noi luu gia nhap, gia ban va tong tien theo dong kien/san pham.
