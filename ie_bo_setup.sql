DO
$$
BEGIN
   IF NOT EXISTS (
      SELECT FROM pg_roles WHERE rolname = 'ie_bo'
   ) THEN
      CREATE ROLE ie_bo LOGIN PASSWORD 'ie_bo';
   END IF;
END
$$;

-- =========================
-- 2. Tao database
-- Chay rieng block nay
-- =========================
CREATE DATABASE ie_bo
WITH OWNER = ie_bo
ENCODING = 'UTF8'
TEMPLATE template0;
 

-- =========================
-- 5. Grant quyen
-- =========================
GRANT ALL PRIVILEGES ON DATABASE ie_bo TO ie_bo;
GRANT ALL PRIVILEGES ON SCHEMA app TO ie_bo;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA app TO ie_bo;

\\connect ie_bo

-- Make sure IE_BO owns public schema and gets future privileges
ALTER SCHEMA public OWNER TO ie_bo;
GRANT ALL ON SCHEMA public TO ie_bo;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO ie_bo;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO ie_bo;

-- 1) Tables in public schema
CREATE SEQUENCE IF NOT EXISTS seq_partner START WITH 1 INCREMENT BY 1;
CREATE TABLE partner (
    id BIGINT PRIMARY KEY DEFAULT nextval('seq_partner'),
    partnercode VARCHAR(50) NOT NULL UNIQUE,
    partnername VARCHAR(255) NOT NULL,
    partnertype VARCHAR(20) NOT NULL CHECK (partnertype IN ('CUSTOMER','SUPPLIER','SHIPPING')),
    address TEXT,
    phone VARCHAR(50),
    email VARCHAR(255),
    isactive BOOLEAN NOT NULL DEFAULT TRUE
);
ALTER SEQUENCE seq_partner OWNED BY partner.id;

CREATE SEQUENCE IF NOT EXISTS seq_product START WITH 1 INCREMENT BY 1;
CREATE TABLE product (
    id BIGINT PRIMARY KEY DEFAULT nextval('seq_product'),
    productcode VARCHAR(50) NOT NULL UNIQUE,
    productname VARCHAR(255) NOT NULL,
    hscode VARCHAR(50),
    unitofmeasure VARCHAR(50) NOT NULL,
    delt varchar(1)
);
ALTER SEQUENCE seq_product OWNED BY product.id;

CREATE SEQUENCE IF NOT EXISTS seq_productimage START WITH 1 INCREMENT BY 1;
CREATE TABLE productimage (
    id BIGINT PRIMARY KEY DEFAULT nextval('seq_productimage'),
    productid BIGINT NOT NULL REFERENCES product(id),
    imageurl TEXT NOT NULL,
    ismain BOOLEAN NOT NULL DEFAULT FALSE,
    isdefault VARCHAR,
    sortorder INT NOT NULL DEFAULT 0,
    delt varchar(1)
);
ALTER SEQUENCE seq_productimage OWNED BY productimage.id;

CREATE SEQUENCE IF NOT EXISTS seq_salesorder START WITH 1 INCREMENT BY 1;
CREATE TABLE salesorder (
    id BIGINT PRIMARY KEY DEFAULT nextval('seq_salesorder'),
    orderno VARCHAR(50) NOT NULL UNIQUE,
    orderdate DATE NOT NULL,
    buyerid BIGINT NOT NULL REFERENCES partner(id),
    sellerid BIGINT NOT NULL REFERENCES partner(id),
    deliverylocation VARCHAR(255),
    status VARCHAR(50),
    deletedflag BOOLEAN NOT NULL DEFAULT FALSE
);
ALTER SEQUENCE seq_salesorder OWNED BY salesorder.id;

CREATE SEQUENCE IF NOT EXISTS seq_salesorderitem START WITH 1 INCREMENT BY 1;
CREATE TABLE salesorderitem (
    id BIGINT PRIMARY KEY DEFAULT nextval('seq_salesorderitem'),
    salesorderid BIGINT NOT NULL REFERENCES salesorder(id),
    productid BIGINT NOT NULL REFERENCES product(id),
    orderedqty NUMERIC(18,3) NOT NULL,
    unitpricevnd NUMERIC(18,2) NOT NULL,
    lineamountvnd NUMERIC(18,2) NOT NULL
);
ALTER SEQUENCE seq_salesorderitem OWNED BY salesorderitem.id;

CREATE SEQUENCE IF NOT EXISTS seq_contract START WITH 1 INCREMENT BY 1;
CREATE TABLE contract (
    id BIGINT PRIMARY KEY DEFAULT nextval('seq_contract'),
    contractno VARCHAR(50) NOT NULL UNIQUE,
    contractdate DATE NOT NULL,
    salesorderid BIGINT NOT NULL REFERENCES salesorder(id),
    incoterm VARCHAR(50),
    paymentterms VARCHAR(255),
    shipmentdateplan VARCHAR(255)
);
ALTER SEQUENCE seq_contract OWNED BY contract.id;

CREATE SEQUENCE IF NOT EXISTS seq_proformainvoice START WITH 1 INCREMENT BY 1;
CREATE TABLE proformainvoice (
    id BIGINT PRIMARY KEY DEFAULT nextval('seq_proformainvoice'),
    pino VARCHAR(50) NOT NULL UNIQUE,
    contractid BIGINT NOT NULL REFERENCES contract(id),
    pidate DATE NOT NULL,
    currencycode VARCHAR(10) NOT NULL
);
ALTER SEQUENCE seq_proformainvoice OWNED BY proformainvoice.id;

CREATE SEQUENCE IF NOT EXISTS seq_proformainvoiceitem START WITH 1 INCREMENT BY 1;
CREATE TABLE proformainvoiceitem (
    id BIGINT PRIMARY KEY DEFAULT nextval('seq_proformainvoiceitem'),
    proformainvoiceid BIGINT NOT NULL REFERENCES proformainvoice(id),
    productid BIGINT NOT NULL REFERENCES product(id),
    quantity NUMERIC(18,3) NOT NULL,
    unitprice NUMERIC(18,2) NOT NULL,
    amount NUMERIC(18,2) NOT NULL
);
ALTER SEQUENCE seq_proformainvoiceitem OWNED BY proformainvoiceitem.id;

CREATE SEQUENCE IF NOT EXISTS seq_commercialinvoice START WITH 1 INCREMENT BY 1;
CREATE TABLE commercialinvoice (
    id BIGINT PRIMARY KEY DEFAULT nextval('seq_commercialinvoice'),
    invoiceno VARCHAR(50) NOT NULL UNIQUE,
    contractid BIGINT NOT NULL REFERENCES contract(id),
    totalamount NUMERIC(18,2) NOT NULL,
    depositamount NUMERIC(18,2),
    balanceamount NUMERIC(18,2)
);
ALTER SEQUENCE seq_commercialinvoice OWNED BY commercialinvoice.id;

CREATE SEQUENCE IF NOT EXISTS seq_commercialinvoiceitem START WITH 1 INCREMENT BY 1;
CREATE TABLE commercialinvoiceitem (
    id BIGINT PRIMARY KEY DEFAULT nextval('seq_commercialinvoiceitem'),
    commercialinvoiceid BIGINT NOT NULL REFERENCES commercialinvoice(id),
    productid BIGINT NOT NULL REFERENCES product(id),
    quantity NUMERIC(18,3) NOT NULL,
    unitprice NUMERIC(18,2) NOT NULL,
    amount NUMERIC(18,2) NOT NULL
);
ALTER SEQUENCE seq_commercialinvoiceitem OWNED BY commercialinvoiceitem.id;

CREATE SEQUENCE IF NOT EXISTS seq_packinglist START WITH 1 INCREMENT BY 1;
CREATE TABLE packinglist (
    id BIGINT PRIMARY KEY DEFAULT nextval('seq_packinglist'),
    packinglistno VARCHAR(50) NOT NULL UNIQUE,
    contractid BIGINT NOT NULL REFERENCES contract(id),
    containerno VARCHAR(100),
    totalctns NUMERIC(18,3),
    totalcbm NUMERIC(18,3)
);
ALTER SEQUENCE seq_packinglist OWNED BY packinglist.id;

CREATE SEQUENCE IF NOT EXISTS seq_packinglistitem START WITH 1 INCREMENT BY 1;
CREATE TABLE packinglistitem (
    id BIGINT PRIMARY KEY DEFAULT nextval('seq_packinglistitem'),
    packinglistid BIGINT NOT NULL REFERENCES packinglist(id),
    productid BIGINT NOT NULL REFERENCES product(id),
    totalunits NUMERIC(18,3) NOT NULL,
    totalctns NUMERIC(18,3),
    totalcbm NUMERIC(18,3)
);
ALTER SEQUENCE seq_packinglistitem OWNED BY packinglistitem.id;

CREATE SEQUENCE IF NOT EXISTS seq_payment START WITH 1 INCREMENT BY 1;
CREATE TABLE payment (
    id BIGINT PRIMARY KEY DEFAULT nextval('seq_payment'),
    commercialinvoiceid BIGINT NOT NULL REFERENCES commercialinvoice(id),
    paymentdate DATE NOT NULL,
    amount NUMERIC(18,2) NOT NULL,
    paymenttype VARCHAR(50)
);
ALTER SEQUENCE seq_payment OWNED BY payment.id;

-- 2) Indexes
CREATE INDEX idxorderno ON salesorder(orderno);
CREATE INDEX idxproductcode ON product(productcode);
CREATE INDEX idxinvoiceno ON commercialinvoice(invoiceno);