BEGIN;

ALTER TABLE public.contract
    ADD COLUMN IF NOT EXISTS buyerid int8 NULL;

ALTER TABLE public.contract
    ALTER COLUMN salesorderid DROP NOT NULL;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'contract_buyerid_fkey'
          AND conrelid = 'public.contract'::regclass
    ) THEN
        ALTER TABLE public.contract
            ADD CONSTRAINT contract_buyerid_fkey FOREIGN KEY (buyerid) REFERENCES public.partner(id);
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS idxcontractbuyerid ON public.contract USING btree (buyerid);

COMMIT;
