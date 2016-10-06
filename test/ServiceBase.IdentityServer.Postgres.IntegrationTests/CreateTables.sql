
-- Table: public.users

DROP TABLE IF EXISTS public.useraccounts;

CREATE TABLE public.useraccounts
(
  id uuid NOT NULL,
  email character varying(254) NOT NULL,
  isemailverified boolean NOT NULL, 
  emailverifiedat timestamp, 
  isloginallowed boolean NOT NULL, 
  lastloginat timestamp, 
  lastfailedloginat timestamp, 
  failedlogincount integer NOT NULL, 
  passwordhash character varying(200) , 
  passwordchangedat timestamp,
  verificationkey character varying(100), 
  verificationpurpose character varying(256),  
  verificationkeysentat timestamp, 
  verificationstorage character varying(2000), 
  createdat timestamp NOT NULL, 
  updatedat timestamp NOT NULL,

  CONSTRAINT pk_useraccounts PRIMARY KEY (id),
  CONSTRAINT useraccounts_email_key UNIQUE (email)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.useraccounts
  OWNER TO postgres;


-- Table: public.userclaims

DROP TABLE IF EXISTS public.userclaims;

CREATE TABLE public.userclaims
(
  userid uuid NOT NULL,
  type text NOT NULL,
  value text NOT NULL,
  valuetype text,
  CONSTRAINT pk_userclaims PRIMARY KEY (userid, type, value)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.userclaims
  OWNER TO postgres;

-- Table: public.externalaccounts

DROP TABLE IF EXISTS public.externalaccounts;

CREATE TABLE public.externalaccounts
(
  userid uuid NOT NULL,
  provider text NOT NULL,
  subject text NOT NULL,
  email text NOT NULL,
  lastloginat timestamp, 
  createdat timestamp NOT NULL, 
  CONSTRAINT pk_externalaccounts PRIMARY KEY (provider, subject)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.externalaccounts
  OWNER TO postgres;
