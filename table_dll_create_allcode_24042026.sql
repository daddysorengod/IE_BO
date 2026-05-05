BEGIN;

-- ============================================================
-- Create allcode table for fixed combobox values
-- ============================================================

CREATE SEQUENCE IF NOT EXISTS public.seq_allcode
    INCREMENT BY 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    START WITH 1
    CACHE 1
    NO CYCLE;

CREATE TABLE IF NOT EXISTS public.allcode (
    id int8 DEFAULT nextval('public.seq_allcode'::regclass) NOT NULL,
    cdname varchar(100) NOT NULL,
    cdvalue varchar(100) NOT NULL,
    text_vn varchar(255) NOT NULL,
    text_en varchar(255) NULL,
    display varchar(1) NOT NULL DEFAULT 'Y',
    CONSTRAINT allcode_pkey PRIMARY KEY (id),
    CONSTRAINT allcode_display_chk CHECK (display IN ('Y', 'N'))
);

ALTER SEQUENCE IF EXISTS public.seq_allcode OWNED BY public.allcode.id;

CREATE INDEX IF NOT EXISTS idxallcodecdname
    ON public.allcode USING btree (cdname);

CREATE INDEX IF NOT EXISTS idxallcodecdnamedisplay
    ON public.allcode USING btree (cdname, display);

INSERT INTO public.allcode (cdname, cdvalue, text_vn, text_en, display)
VALUES
  ('USER_ROLE', 'USER', 'USER', 'User', 'Y'),
  ('USER_ROLE', 'MANAGER', 'MANAGER', 'Manager', 'Y'),
  ('USER_ROLE', 'ADMIN', 'ADMIN', 'Administrator', 'Y'),
  ('PARTNER_TYPE', 'CUSTOMER', 'Khach hang', 'Customer', 'Y'),
  ('PARTNER_TYPE', 'SUPPLIER', 'Nha cung cap', 'Supplier', 'Y'),
  ('ACTIVE_STATUS', 'false', 'Ngung hoat dong', 'Inactive', 'Y'),
  ('CONTRACT_TYPE', 'IMPORT', 'Hop dong nhap', 'Import Contract', 'Y'),
  ('CONTRACT_TYPE', 'EXPORT', 'Hop dong xuat', 'Export Contract', 'Y');
COMMIT;
