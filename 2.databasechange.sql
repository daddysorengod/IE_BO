-- public."user" definition

-- Drop table

-- DROP TABLE public."user";

CREATE TABLE public."user" (
	id int8 DEFAULT nextval('users_id_seq'::regclass) NOT NULL,
	username varchar(255) NOT NULL,
	email varchar(255) NOT NULL,
	passwordhash varchar(255) NOT NULL,
	fullname varchar(255) NOT NULL,
	imageurl varchar(255) NULL,
	"role" varchar(50) NOT NULL,
	refreshtoken text NULL,
	delt varchar(50) NULL,
	isactive varchar(50) NULL,
	CONSTRAINT user_unique UNIQUE (username),
	CONSTRAINT users_email_key UNIQUE (email),
	CONSTRAINT users_pkey PRIMARY KEY (id)
);
ALTER TABLE public.product ADD addby int8 NULL;

ALTER TABLE public.product ADD updateby int8 NULL;

ALTER TABLE public.product ADD createddate date NULL;
ALTER TABLE public.packinglist ADD length decimal NULL;
ALTER TABLE public.packinglist ADD width decimal NULL;
ALTER TABLE public.packinglist ADD height decimal NULL;
