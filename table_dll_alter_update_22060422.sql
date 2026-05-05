BEGIN;

-- ============================================================
-- 1. Add standardized audit columns to existing tables
-- ============================================================

ALTER TABLE public.partner
    ADD COLUMN IF NOT EXISTS createdby int8 NULL,
    ADD COLUMN IF NOT EXISTS createddate timestamp NULL,
    ADD COLUMN IF NOT EXISTS updatedby int8 NULL,
    ADD COLUMN IF NOT EXISTS updateddate timestamp NULL;

ALTER TABLE public.product
    ADD COLUMN IF NOT EXISTS createdby int8 NULL,
    ADD COLUMN IF NOT EXISTS createddate timestamp NULL,
    ADD COLUMN IF NOT EXISTS updatedby int8 NULL,
    ADD COLUMN IF NOT EXISTS updateddate timestamp NULL;

ALTER TABLE public.product
    ALTER COLUMN createddate TYPE timestamp USING createddate::timestamp;

ALTER TABLE public."user"
    ADD COLUMN IF NOT EXISTS createdby int8 NULL,
    ADD COLUMN IF NOT EXISTS createddate timestamp NULL,
    ADD COLUMN IF NOT EXISTS updatedby int8 NULL,
    ADD COLUMN IF NOT EXISTS updateddate timestamp NULL;

ALTER TABLE public.productimage
    ADD COLUMN IF NOT EXISTS createdby int8 NULL,
    ADD COLUMN IF NOT EXISTS createddate timestamp NULL,
    ADD COLUMN IF NOT EXISTS updatedby int8 NULL,
    ADD COLUMN IF NOT EXISTS updateddate timestamp NULL;

ALTER TABLE public.salesorder
    ADD COLUMN IF NOT EXISTS createdby int8 NULL,
    ADD COLUMN IF NOT EXISTS createddate timestamp NULL,
    ADD COLUMN IF NOT EXISTS updatedby int8 NULL,
    ADD COLUMN IF NOT EXISTS updateddate timestamp NULL;

ALTER TABLE public.salesorderitem
    ADD COLUMN IF NOT EXISTS createdby int8 NULL,
    ADD COLUMN IF NOT EXISTS createddate timestamp NULL,
    ADD COLUMN IF NOT EXISTS updatedby int8 NULL,
    ADD COLUMN IF NOT EXISTS updateddate timestamp NULL;

ALTER TABLE public.contract
    ADD COLUMN IF NOT EXISTS createdby int8 NULL,
    ADD COLUMN IF NOT EXISTS createddate timestamp NULL,
    ADD COLUMN IF NOT EXISTS updatedby int8 NULL,
    ADD COLUMN IF NOT EXISTS updateddate timestamp NULL;

ALTER TABLE public.packinglist
    ADD COLUMN IF NOT EXISTS createdby int8 NULL,
    ADD COLUMN IF NOT EXISTS createddate timestamp NULL,
    ADD COLUMN IF NOT EXISTS updatedby int8 NULL,
    ADD COLUMN IF NOT EXISTS updateddate timestamp NULL;

ALTER TABLE public.packinglistitem
    ADD COLUMN IF NOT EXISTS createdby int8 NULL,
    ADD COLUMN IF NOT EXISTS createddate timestamp NULL,
    ADD COLUMN IF NOT EXISTS updatedby int8 NULL,
    ADD COLUMN IF NOT EXISTS updateddate timestamp NULL;

ALTER TABLE public.proformainvoice
    ADD COLUMN IF NOT EXISTS createdby int8 NULL,
    ADD COLUMN IF NOT EXISTS createddate timestamp NULL,
    ADD COLUMN IF NOT EXISTS updatedby int8 NULL,
    ADD COLUMN IF NOT EXISTS updateddate timestamp NULL;

ALTER TABLE public.proformainvoiceitem
    ADD COLUMN IF NOT EXISTS createdby int8 NULL,
    ADD COLUMN IF NOT EXISTS createddate timestamp NULL,
    ADD COLUMN IF NOT EXISTS updatedby int8 NULL,
    ADD COLUMN IF NOT EXISTS updateddate timestamp NULL;

ALTER TABLE public.commercialinvoice
    ADD COLUMN IF NOT EXISTS createdby int8 NULL,
    ADD COLUMN IF NOT EXISTS createddate timestamp NULL,
    ADD COLUMN IF NOT EXISTS updatedby int8 NULL,
    ADD COLUMN IF NOT EXISTS updateddate timestamp NULL;

ALTER TABLE public.commercialinvoiceitem
    ADD COLUMN IF NOT EXISTS createdby int8 NULL,
    ADD COLUMN IF NOT EXISTS createddate timestamp NULL,
    ADD COLUMN IF NOT EXISTS updatedby int8 NULL,
    ADD COLUMN IF NOT EXISTS updateddate timestamp NULL;

ALTER TABLE public.payment
    ADD COLUMN IF NOT EXISTS createdby int8 NULL,
    ADD COLUMN IF NOT EXISTS createddate timestamp NULL,
    ADD COLUMN IF NOT EXISTS updatedby int8 NULL,
    ADD COLUMN IF NOT EXISTS updateddate timestamp NULL;

-- ============================================================
-- 2. Contract item table
-- ============================================================

CREATE SEQUENCE IF NOT EXISTS public.seq_contractitem
    INCREMENT BY 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    START WITH 1
    CACHE 1
    NO CYCLE;

CREATE TABLE IF NOT EXISTS public.contractitem (
    id int8 DEFAULT nextval('public.seq_contractitem'::regclass) NOT NULL,
    contractid int8 NOT NULL,
    productid int8 NOT NULL,
    supplierid int8 NOT NULL,
    quantity numeric(18, 3) NOT NULL,
    importprice numeric(18, 2) NOT NULL,
    sellprice numeric(18, 2) NOT NULL,
    totalimportamount numeric(18, 2) NOT NULL,
    totalsellamount numeric(18, 2) NOT NULL,
    createdby int8 NULL,
    createddate timestamp NULL,
    updatedby int8 NULL,
    updateddate timestamp NULL,
    CONSTRAINT contractitem_pkey PRIMARY KEY (id),
    CONSTRAINT contractitem_contractid_fkey FOREIGN KEY (contractid) REFERENCES public.contract(id),
    CONSTRAINT contractitem_productid_fkey FOREIGN KEY (productid) REFERENCES public.product(id),
    CONSTRAINT contractitem_supplierid_fkey FOREIGN KEY (supplierid) REFERENCES public.partner(id)
);

CREATE INDEX IF NOT EXISTS idxcontractitemcontractid ON public.contractitem USING btree (contractid);
CREATE INDEX IF NOT EXISTS idxcontractitemproductid ON public.contractitem USING btree (productid);
CREATE INDEX IF NOT EXISTS idxcontractitemsupplierid ON public.contractitem USING btree (supplierid);

-- ============================================================
-- 3. Contract attachment table
-- ============================================================

CREATE SEQUENCE IF NOT EXISTS public.seq_contractattachment
    INCREMENT BY 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    START WITH 1
    CACHE 1
    NO CYCLE;

CREATE TABLE IF NOT EXISTS public.contractattachment (
    id int8 DEFAULT nextval('public.seq_contractattachment'::regclass) NOT NULL,
    contractid int8 NOT NULL,
    filename varchar(255) NOT NULL,
    fileurl text NOT NULL,
    fileextension varchar(50) NULL,
    createdby int8 NULL,
    createddate timestamp NULL,
    updatedby int8 NULL,
    updateddate timestamp NULL,
    CONSTRAINT contractattachment_pkey PRIMARY KEY (id),
    CONSTRAINT contractattachment_contractid_fkey FOREIGN KEY (contractid) REFERENCES public.contract(id)
);

CREATE INDEX IF NOT EXISTS idxcontractattachmentcontractid ON public.contractattachment USING btree (contractid);

COMMIT;
