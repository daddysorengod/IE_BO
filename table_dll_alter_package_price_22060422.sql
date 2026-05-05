BEGIN;

ALTER TABLE public.contractitem
    ADD COLUMN IF NOT EXISTS packageid int8 NULL;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'contractitem_packageid_fkey'
          AND conrelid = 'public.contractitem'::regclass
    ) THEN
        ALTER TABLE public.contractitem
            ADD CONSTRAINT contractitem_packageid_fkey
            FOREIGN KEY (packageid) REFERENCES public.packinglist(id);
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS idxcontractitempackageid
    ON public.contractitem USING btree (packageid);

COMMIT;
