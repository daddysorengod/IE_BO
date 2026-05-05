BEGIN;

INSERT INTO public.allcode (cdname, cdvalue, text_vn, text_en, display)
SELECT 'CONTRACT_TYPE', 'IMPORT', 'Hop dong nhap', 'Import Contract', 'Y'
WHERE NOT EXISTS (
    SELECT 1
    FROM public.allcode
    WHERE cdname = 'CONTRACT_TYPE'
      AND cdvalue = 'IMPORT'
);

INSERT INTO public.allcode (cdname, cdvalue, text_vn, text_en, display)
SELECT 'CONTRACT_TYPE', 'EXPORT', 'Hop dong xuat', 'Export Contract', 'Y'
WHERE NOT EXISTS (
    SELECT 1
    FROM public.allcode
    WHERE cdname = 'CONTRACT_TYPE'
      AND cdvalue = 'EXPORT'
);

COMMIT;
