BEGIN;

ALTER TABLE public.product
    ADD COLUMN IF NOT EXISTS material varchar(255) NULL,
    ADD COLUMN IF NOT EXISTS productionyear int4 NULL,
    ADD COLUMN IF NOT EXISTS note text NULL,
    ADD COLUMN IF NOT EXISTS length numeric(18, 3) NULL,
    ADD COLUMN IF NOT EXISTS width numeric(18, 3) NULL,
    ADD COLUMN IF NOT EXISTS height numeric(18, 3) NULL;

COMMIT;
