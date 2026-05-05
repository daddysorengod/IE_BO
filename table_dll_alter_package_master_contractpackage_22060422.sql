BEGIN;

ALTER TABLE public.contract
    ADD COLUMN IF NOT EXISTS note text NULL;

ALTER TABLE public.packinglist
    ALTER COLUMN contractid DROP NOT NULL;

ALTER TABLE public.packinglist
    ADD COLUMN IF NOT EXISTS packagename varchar(255) NULL,
    ADD COLUMN IF NOT EXISTS productquantity numeric(18, 3) NULL,
    ADD COLUMN IF NOT EXISTS note text NULL,
    ADD COLUMN IF NOT EXISTS delt varchar(1) NULL;

UPDATE public.packinglist
SET packagename = packinglistno
WHERE packagename IS NULL;

UPDATE public.packinglist
SET productquantity = 1
WHERE productquantity IS NULL;

ALTER TABLE public.packinglist
    ALTER COLUMN packagename SET NOT NULL,
    ALTER COLUMN productquantity SET NOT NULL,
    ALTER COLUMN productquantity SET DEFAULT 1;

CREATE INDEX IF NOT EXISTS idxpackinglistpackagename
    ON public.packinglist USING btree (packagename);

CREATE SEQUENCE IF NOT EXISTS public.seq_contractpackage
    INCREMENT BY 1
    MINVALUE 1
    NO MAXVALUE
    START WITH 1
    CACHE 1;

CREATE TABLE IF NOT EXISTS public.contractpackage (
    id int8 DEFAULT nextval('seq_contractpackage'::regclass) NOT NULL,
    contractid int8 NOT NULL,
    packageid int8 NOT NULL,
    productid int8 NOT NULL,
    packagequantity numeric(18, 3) DEFAULT 1 NOT NULL,
    note text NULL,
    createdby int8 NULL,
    createddate timestamp NULL,
    updatedby int8 NULL,
    updateddate timestamp NULL,
    CONSTRAINT contractpackage_pkey PRIMARY KEY (id)
);

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'contractpackage_contractid_fkey'
          AND conrelid = 'public.contractpackage'::regclass
    ) THEN
        ALTER TABLE public.contractpackage
            ADD CONSTRAINT contractpackage_contractid_fkey
            FOREIGN KEY (contractid) REFERENCES public.contract(id);
    END IF;
END $$;

ALTER TABLE public.contractpackage
    DROP CONSTRAINT IF EXISTS contractpackage_packageid_fkey;

ALTER TABLE public.contractpackage
    ADD CONSTRAINT contractpackage_packageid_fkey
    FOREIGN KEY (packageid) REFERENCES public.packinglist(id);

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'contractpackage_productid_fkey'
          AND conrelid = 'public.contractpackage'::regclass
    ) THEN
        ALTER TABLE public.contractpackage
            ADD CONSTRAINT contractpackage_productid_fkey
            FOREIGN KEY (productid) REFERENCES public.product(id);
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS idxcontractpackagecontractid
    ON public.contractpackage USING btree (contractid);

CREATE INDEX IF NOT EXISTS idxcontractpackagepackageid
    ON public.contractpackage USING btree (packageid);

CREATE INDEX IF NOT EXISTS idxcontractpackageproductid
    ON public.contractpackage USING btree (productid);

ALTER TABLE public.contractitem
    ADD COLUMN IF NOT EXISTS contractpackageid int8 NULL,
    ADD COLUMN IF NOT EXISTS packageid int8 NULL;

ALTER TABLE public.contractitem
    ALTER COLUMN supplierid DROP NOT NULL;

ALTER TABLE public.contractitem
    DROP CONSTRAINT IF EXISTS contractitem_packageid_fkey;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'contractitem_contractpackageid_fkey'
          AND conrelid = 'public.contractitem'::regclass
    ) THEN
        ALTER TABLE public.contractitem
            ADD CONSTRAINT contractitem_contractpackageid_fkey
            FOREIGN KEY (contractpackageid) REFERENCES public.contractpackage(id) NOT VALID;
    END IF;
END $$;

ALTER TABLE public.contractitem
    ADD CONSTRAINT contractitem_packageid_fkey
    FOREIGN KEY (packageid) REFERENCES public.packinglist(id) NOT VALID;

CREATE INDEX IF NOT EXISTS idxcontractitemcontractpackageid
    ON public.contractitem USING btree (contractpackageid);

CREATE INDEX IF NOT EXISTS idxcontractitempackageid
    ON public.contractitem USING btree (packageid);

COMMIT;
