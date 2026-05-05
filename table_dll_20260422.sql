-- public.partner definition

-- Drop table

-- DROP TABLE public.partner;

CREATE TABLE public.partner (
	id int8 DEFAULT nextval('seq_partner'::regclass) NOT NULL,
	partnercode varchar(50) NOT NULL,
	partnername varchar(255) NOT NULL,
	partnertype varchar(20) NOT NULL,
	address text NULL,
	phone varchar(50) NULL,
	email varchar(255) NULL,
	isactive bool DEFAULT true NOT NULL,
	CONSTRAINT partner_partnercode_key UNIQUE (partnercode),
	CONSTRAINT partner_partnertype_check CHECK (((partnertype)::text = ANY ((ARRAY['CUSTOMER'::character varying, 'SUPPLIER'::character varying, 'SHIPPING'::character varying])::text[]))),
	CONSTRAINT partner_pkey PRIMARY KEY (id)
);


-- public.product definition

-- Drop table

-- DROP TABLE public.product;

CREATE TABLE public.product (
	id int8 DEFAULT nextval('seq_product'::regclass) NOT NULL,
	productcode varchar(50) NOT NULL,
	productname varchar(255) NOT NULL,
	hscode varchar(50) NULL,
	unitofmeasure varchar(50) NOT NULL,
	delt varchar(1) NULL,
	addby int8 NULL,
	updateby int8 NULL,
	createddate date NULL,
	CONSTRAINT product_pkey PRIMARY KEY (id),
	CONSTRAINT product_productcode_key UNIQUE (productcode)
);
CREATE INDEX idxproductcode ON public.product USING btree (productcode);


-- public."user" definition

-- Drop table

-- DROP TABLE public."user";

CREATE TABLE public."user" (
	id int8 DEFAULT nextval('users_id_seq'::regclass) NOT NULL,
	email varchar(255) NOT NULL,
	passwordhash varchar(255) NOT NULL,
	fullname varchar(255) NOT NULL,
	imageurl varchar(255) NULL,
	"role" varchar(50) NOT NULL,
	refreshtoken text NULL,
	delt varchar(50) NULL,
	isactive varchar(50) NULL,
	username varchar(255) NULL,
	CONSTRAINT user_unique UNIQUE (username),
	CONSTRAINT users_email_key UNIQUE (email),
	CONSTRAINT users_pkey PRIMARY KEY (id)
);


-- public.productimage definition

-- Drop table

-- DROP TABLE public.productimage;

CREATE TABLE public.productimage (
	id int8 DEFAULT nextval('seq_productimage'::regclass) NOT NULL,
	productid int8 NOT NULL,
	imageurl text NOT NULL,
	ismain bool DEFAULT false NOT NULL,
	sortorder int4 DEFAULT 0 NOT NULL,
	delt varchar(1) NULL,
	isdefault varchar NULL,
	CONSTRAINT productimage_pkey PRIMARY KEY (id),
	CONSTRAINT productimage_productid_fkey FOREIGN KEY (productid) REFERENCES public.product(id)
);


-- public.salesorder definition

-- Drop table

-- DROP TABLE public.salesorder;

CREATE TABLE public.salesorder (
	id int8 DEFAULT nextval('seq_salesorder'::regclass) NOT NULL,
	orderno varchar(50) NOT NULL,
	orderdate date NOT NULL,
	buyerid int8 NOT NULL,
	sellerid int8 NOT NULL,
	deliverylocation varchar(255) NULL,
	status varchar(50) NULL,
	deletedflag bool DEFAULT false NOT NULL,
	CONSTRAINT salesorder_orderno_key UNIQUE (orderno),
	CONSTRAINT salesorder_pkey PRIMARY KEY (id),
	CONSTRAINT salesorder_buyerid_fkey FOREIGN KEY (buyerid) REFERENCES public.partner(id),
	CONSTRAINT salesorder_sellerid_fkey FOREIGN KEY (sellerid) REFERENCES public.partner(id)
);
CREATE INDEX idxorderno ON public.salesorder USING btree (orderno);


-- public.salesorderitem definition

-- Drop table

-- DROP TABLE public.salesorderitem;

CREATE TABLE public.salesorderitem (
	id int8 DEFAULT nextval('seq_salesorderitem'::regclass) NOT NULL,
	salesorderid int8 NOT NULL,
	productid int8 NOT NULL,
	orderedqty numeric(18, 3) NOT NULL,
	unitpricevnd numeric(18, 2) NOT NULL,
	lineamountvnd numeric(18, 2) NOT NULL,
	CONSTRAINT salesorderitem_pkey PRIMARY KEY (id),
	CONSTRAINT salesorderitem_productid_fkey FOREIGN KEY (productid) REFERENCES public.product(id),
	CONSTRAINT salesorderitem_salesorderid_fkey FOREIGN KEY (salesorderid) REFERENCES public.salesorder(id)
);


-- public.contract definition

-- Drop table

-- DROP TABLE public.contract;

CREATE TABLE public.contract (
	id int8 DEFAULT nextval('seq_contract'::regclass) NOT NULL,
	contractno varchar(50) NOT NULL,
	contractdate date NOT NULL,
	salesorderid int8 NOT NULL,
	incoterm varchar(50) NULL,
	paymentterms varchar(255) NULL,
	shipmentdateplan varchar(255) NULL,
	CONSTRAINT contract_contractno_key UNIQUE (contractno),
	CONSTRAINT contract_pkey PRIMARY KEY (id),
	CONSTRAINT contract_salesorderid_fkey FOREIGN KEY (salesorderid) REFERENCES public.salesorder(id)
);


-- public.packinglist definition

-- Drop table

-- DROP TABLE public.packinglist;

CREATE TABLE public.packinglist (
	id int8 DEFAULT nextval('seq_packinglist'::regclass) NOT NULL,
	packinglistno varchar(50) NOT NULL,
	contractid int8 NOT NULL,
	containerno varchar(100) NULL,
	totalctns numeric(18, 3) NULL,
	totalcbm numeric(18, 3) NULL,
	length numeric NULL,
	width numeric NULL,
	height numeric NULL,
	CONSTRAINT packinglist_packinglistno_key UNIQUE (packinglistno),
	CONSTRAINT packinglist_pkey PRIMARY KEY (id),
	CONSTRAINT packinglist_contractid_fkey FOREIGN KEY (contractid) REFERENCES public.contract(id)
);


-- public.packinglistitem definition

-- Drop table

-- DROP TABLE public.packinglistitem;

CREATE TABLE public.packinglistitem (
	id int8 DEFAULT nextval('seq_packinglistitem'::regclass) NOT NULL,
	packinglistid int8 NOT NULL,
	productid int8 NOT NULL,
	totalunits numeric(18, 3) NOT NULL,
	totalctns numeric(18, 3) NULL,
	totalcbm numeric(18, 3) NULL,
	CONSTRAINT packinglistitem_pkey PRIMARY KEY (id),
	CONSTRAINT packinglistitem_packinglistid_fkey FOREIGN KEY (packinglistid) REFERENCES public.packinglist(id),
	CONSTRAINT packinglistitem_productid_fkey FOREIGN KEY (productid) REFERENCES public.product(id)
);


-- public.proformainvoice definition

-- Drop table

-- DROP TABLE public.proformainvoice;

CREATE TABLE public.proformainvoice (
	id int8 DEFAULT nextval('seq_proformainvoice'::regclass) NOT NULL,
	pino varchar(50) NOT NULL,
	contractid int8 NOT NULL,
	pidate date NOT NULL,
	currencycode varchar(10) NOT NULL,
	CONSTRAINT proformainvoice_pino_key UNIQUE (pino),
	CONSTRAINT proformainvoice_pkey PRIMARY KEY (id),
	CONSTRAINT proformainvoice_contractid_fkey FOREIGN KEY (contractid) REFERENCES public.contract(id)
);


-- public.proformainvoiceitem definition

-- Drop table

-- DROP TABLE public.proformainvoiceitem;

CREATE TABLE public.proformainvoiceitem (
	id int8 DEFAULT nextval('seq_proformainvoiceitem'::regclass) NOT NULL,
	proformainvoiceid int8 NOT NULL,
	productid int8 NOT NULL,
	quantity numeric(18, 3) NOT NULL,
	unitprice numeric(18, 2) NOT NULL,
	amount numeric(18, 2) NOT NULL,
	CONSTRAINT proformainvoiceitem_pkey PRIMARY KEY (id),
	CONSTRAINT proformainvoiceitem_productid_fkey FOREIGN KEY (productid) REFERENCES public.product(id),
	CONSTRAINT proformainvoiceitem_proformainvoiceid_fkey FOREIGN KEY (proformainvoiceid) REFERENCES public.proformainvoice(id)
);


-- public.commercialinvoice definition

-- Drop table

-- DROP TABLE public.commercialinvoice;

CREATE TABLE public.commercialinvoice (
	id int8 DEFAULT nextval('seq_commercialinvoice'::regclass) NOT NULL,
	invoiceno varchar(50) NOT NULL,
	contractid int8 NOT NULL,
	totalamount numeric(18, 2) NOT NULL,
	depositamount numeric(18, 2) NULL,
	balanceamount numeric(18, 2) NULL,
	delt varchar(1) NULL,
	CONSTRAINT commercialinvoice_invoiceno_key UNIQUE (invoiceno),
	CONSTRAINT commercialinvoice_pkey PRIMARY KEY (id),
	CONSTRAINT commercialinvoice_contractid_fkey FOREIGN KEY (contractid) REFERENCES public.contract(id)
);
CREATE INDEX idxinvoiceno ON public.commercialinvoice USING btree (invoiceno);


-- public.commercialinvoiceitem definition

-- Drop table

-- DROP TABLE public.commercialinvoiceitem;

CREATE TABLE public.commercialinvoiceitem (
	id int8 DEFAULT nextval('seq_commercialinvoiceitem'::regclass) NOT NULL,
	commercialinvoiceid int8 NOT NULL,
	productid int8 NOT NULL,
	quantity numeric(18, 3) NOT NULL,
	unitprice numeric(18, 2) NOT NULL,
	amount numeric(18, 2) NOT NULL,
	CONSTRAINT commercialinvoiceitem_pkey PRIMARY KEY (id),
	CONSTRAINT commercialinvoiceitem_commercialinvoiceid_fkey FOREIGN KEY (commercialinvoiceid) REFERENCES public.commercialinvoice(id),
	CONSTRAINT commercialinvoiceitem_productid_fkey FOREIGN KEY (productid) REFERENCES public.product(id)
);


-- public.payment definition

-- Drop table

-- DROP TABLE public.payment;

CREATE TABLE public.payment (
	id int8 DEFAULT nextval('seq_payment'::regclass) NOT NULL,
	commercialinvoiceid int8 NOT NULL,
	paymentdate date NOT NULL,
	amount numeric(18, 2) NOT NULL,
	paymenttype varchar(50) NULL,
	CONSTRAINT payment_pkey PRIMARY KEY (id),
	CONSTRAINT payment_commercialinvoiceid_fkey FOREIGN KEY (commercialinvoiceid) REFERENCES public.commercialinvoice(id)
);