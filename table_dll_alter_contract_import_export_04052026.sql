BEGIN;

-- ============================================================
-- Contract header: support IMPORT/EXPORT flow
-- ============================================================

ALTER TABLE public.contract
    ADD COLUMN IF NOT EXISTS contracttype varchar(20) NULL,
    ADD COLUMN IF NOT EXISTS supplierid int8 NULL,
    ADD COLUMN IF NOT EXISTS importcontractid int8 NULL;

UPDATE public.contract
SET contracttype = 'EXPORT'
WHERE contracttype IS NULL;

ALTER TABLE public.contract
    ALTER COLUMN contracttype SET DEFAULT 'EXPORT',
    ALTER COLUMN contracttype SET NOT NULL;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'contract_contracttype_chk'
          AND conrelid = 'public.contract'::regclass
    ) THEN
        ALTER TABLE public.contract
            ADD CONSTRAINT contract_contracttype_chk
            CHECK (contracttype IN ('IMPORT', 'EXPORT'));
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'contract_supplierid_fkey'
          AND conrelid = 'public.contract'::regclass
    ) THEN
        ALTER TABLE public.contract
            ADD CONSTRAINT contract_supplierid_fkey
            FOREIGN KEY (supplierid) REFERENCES public.partner(id);
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'contract_importcontractid_fkey'
          AND conrelid = 'public.contract'::regclass
    ) THEN
        ALTER TABLE public.contract
            ADD CONSTRAINT contract_importcontractid_fkey
            FOREIGN KEY (importcontractid) REFERENCES public.contract(id);
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS idxcontractcontracttype
    ON public.contract USING btree (contracttype);

CREATE INDEX IF NOT EXISTS idxcontractsupplierid
    ON public.contract USING btree (supplierid);

CREATE INDEX IF NOT EXISTS idxcontractimportcontractid
    ON public.contract USING btree (importcontractid);

-- ============================================================
-- Contract item: allow import contracts without package/package line
-- ============================================================

ALTER TABLE public.contractitem
    ADD COLUMN IF NOT EXISTS contractpackageid int8 NULL,
    ADD COLUMN IF NOT EXISTS packageid int8 NULL;

ALTER TABLE public.contractitem
    ALTER COLUMN contractpackageid DROP NOT NULL,
    ALTER COLUMN packageid DROP NOT NULL,
    ALTER COLUMN supplierid DROP NOT NULL;

COMMIT;
