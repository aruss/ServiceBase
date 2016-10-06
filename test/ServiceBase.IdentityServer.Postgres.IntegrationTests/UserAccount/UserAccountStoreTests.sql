

insert into public.useraccounts(id, email, isemailverified, emailverifiedat, isloginallowed, lastloginat, lastfailedloginat, failedlogincount, passwordhash, passwordchangedat, verificationkey, verificationpurpose, verificationkeysentat, verificationstorage, createdat, updatedat) VALUES

	('bfa385d9-b818-4a9f-a3e8-36d467972383', 'alice@localhost', TRUE,'2016-08-02 10:10:03.361794', TRUE, NULL, NULL, 0, 'alice', NULL, NULL, NULL, NULL, NULL, '2016-08-02 10:10:03.361794', '2016-08-02 10:10:03.361794'),
	('a1628ade-1f5f-4032-b13c-63683236e718', 'bob@localhost', TRUE, '2016-08-02 10:10:03.361794', TRUE, NULL, NULL, 0, 'bob', NULL, NULL, NULL, NULL, NULL, '2016-08-02 10:10:03.361794', '2016-08-02 10:10:03.361794'),
	('284b28fc-c4f2-40a5-b106-428d8565e466', 'jim@giggle.com', FALSE, '2016-08-02 10:10:03.361794', TRUE, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, '2016-08-02 10:10:03.361794', '2016-08-02 10:10:03.361794'),
	('284b28fc-c4f2-40a5-b106-428d8565e467', 'bill@giggle.com', FALSE, '2016-08-02 10:10:03.361794', TRUE, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, '2016-08-02 10:10:03.361794', '2016-08-02 10:10:03.361794')
;


INSERT INTO public.userclaims(userid, type, value, valuetype) VALUES 

	('bfa385d9-b818-4a9f-a3e8-36d467972383', 'name', 'Alice Smith', NULL),
	('bfa385d9-b818-4a9f-a3e8-36d467972383', 'given_name', 'Alice', NULL),
	('bfa385d9-b818-4a9f-a3e8-36d467972383', 'family_name', 'Smith', NULL),
	('bfa385d9-b818-4a9f-a3e8-36d467972383', 'email', 'alicesmith@email.com', NULL),
	('bfa385d9-b818-4a9f-a3e8-36d467972383', 'email_verified', 'true', 'http://www.w3.org/2001/XMLSchema#boolean'),
	('bfa385d9-b818-4a9f-a3e8-36d467972383', 'role', 'Admin', NULL),
	('bfa385d9-b818-4a9f-a3e8-36d467972383', 'role', 'Geek', NULL),
	('bfa385d9-b818-4a9f-a3e8-36d467972383', 'website', 'http://alice.com', NULL),

	('a1628ade-1f5f-4032-b13c-63683236e718', 'name', 'Bob Smith', NULL),
	('a1628ade-1f5f-4032-b13c-63683236e718', 'given_name', 'Bob', NULL),
	('a1628ade-1f5f-4032-b13c-63683236e718', 'family_name', 'Smith', NULL),
	('a1628ade-1f5f-4032-b13c-63683236e718', 'email', 'bobsmith@email.com', NULL),
	('a1628ade-1f5f-4032-b13c-63683236e718', 'email_verified', 'true', 'http://www.w3.org/2001/XMLSchema#boolean'),
	('a1628ade-1f5f-4032-b13c-63683236e718', 'role', 'Developer', NULL),

	('284b28fc-c4f2-40a5-b106-428d8565e466', 'name', 'Jim Giggle', NULL),
	('284b28fc-c4f2-40a5-b106-428d8565e466', 'website', 'http://jim.com', NULL)
;

INSERT INTO public.externalaccounts(userid, provider, subject, email, lastloginat, createdat) VALUES
	('284b28fc-c4f2-40a5-b106-428d8565e466', 'google', '1234567890', 'jim@giggle.com', NULL, '2016-08-02 10:10:03.361794')
;

