-- TABLE: NODE TABLE 
-- TRACKS THE PHYSICAL DEVICES (NODES) THAT ARE PERMITTED TO USE THE SYSTEM
CREATE TABLE Node
(
	NodeId	INTEGER NOT NULL IDENTITY(1,1), -- A UNIQUE IDENTIFIER FOR THE NODE
	CONSTRAINT PK_Node PRIMARY KEY (NodeId)
);

-- TABLE: STATUS CODE TABLE
-- A CODE TABLE CONTAINING THE ALLOWED STATUS CODES FOR USE IN THE AUDIT SUBSYSTEM
CREATE TABLE StatusCode 
(
	CodeId	INTEGER NOT NULL, -- THE CODE IDENTIFIER
	Name	VARCHAR(16) NOT NULL UNIQUE, -- THE DISPLAY NAME OF THE CODE
	CONSTRAINT PK_StatusCode PRIMARY KEY (CodeId)
);

INSERT INTO StatusCode VALUES (0, 'NEW'); -- DATA IS NEW AND UNVERIFIED
INSERT INTO StatusCode VALUES (1, 'ACTIVE'); -- DATA IS VERIFIED AND ACTIVE
INSERT INTO StatusCode VALUES (2, 'HELD'); -- DATA WAS ACTIVE BUT IS NOW ON HOLD AND REQUIRES A REVIEW
INSERT INTO StatusCode VALUES (3, 'NULLIFIED'); -- DATA IS NULLIFIED AND WAS NEVER INTENDED TO BE ENTERED
INSERT INTO StatusCode VALUES (4, 'OBSOLETE'); -- DATA IS OBSOLETE AND HAS BEEN REPLACED
INSERT INTO StatusCode VALUES (5, 'ARCHIVED'); -- DATA WAS ACTIVE BUT IS NO LONGER RELEVANT
INSERT INTO StatusCode VALUES (6, 'SYSTEM'); -- SYSTEM LEVEL AUDIT NOT FOR DISPLAY 

-- TABLE: AUDIT CODE TABLE
-- USED TO TRACK THE CODES THAT ARE USED IN THE AUDITING
CREATE TABLE AuditCode
(
	CodeId INTEGER NOT NULL IDENTITY(1,1), -- A UNIQUE ID FOR THE CODE
	Mnemonic VARCHAR(54) NOT NULL, -- THE MNEMNONIC FOR THE CODE
	Domain VARCHAR(54) NOT NULL, -- A DOMAIN TO WHICH THE MNEMONIC BELONGS
	DisplayName VARCHAR(256), -- THE HUMAN READABLE NAME FOR THE CODE
	CONSTRAINT PK_AuditCode PRIMARY KEY (CodeId)
);
CREATE UNIQUE INDEX AuditCode_MnemonicDomainIdx ON AuditCode(Domain, Mnemonic);

-- SEED DATA
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('1','AuditableObjectType','Person');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('2','AuditableObjectType','System Object');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('3','AuditableObjectType','Organization');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('4','AuditableObjectType','Other');

INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('1','AuditableObjectRole','Patient');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('2','AuditableObjectRole','Location');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('3','AuditableObjectRole','Report');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('4','AuditableObjectRole','Resource');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('5','AuditableObjectRole','Master File');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('6','AuditableObjectRole','User');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('7','AuditableObjectRole','List');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('8','AuditableObjectRole','Doctor');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('9','AuditableObjectRole','Subscriber');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('10','AuditableObjectRole','Guarantor');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('11','AuditableObjectRole','Security User');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('12','AuditableObjectRole','Security Group');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('13','AuditableObjectRole','Security Resource');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('14','AuditableObjectRole','Security Granularity Definition');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('15','AuditableObjectRole','Provider');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('16','AuditableObjectRole','Data Destination');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('17','AuditableObjectRole','Data Repository');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('18','AuditableObjectRole','Schedule');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('19','AuditableObjectRole','Customer');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('20','AuditableObjectRole','Job');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('21','AuditableObjectRole','Job Stream');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('22','AuditableObjectRole','Table');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('23','AuditableObjectRole','Routing Criteria');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('24','AuditableObjectRole','Query');

INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('1','AuditableObjectLifecycle','Creation');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('2','AuditableObjectLifecycle','Import');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('3','AuditableObjectLifecycle','Amendment');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('4','AuditableObjectLifecycle','Verification');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('5','AuditableObjectLifecycle','Translation');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('6','AuditableObjectLifecycle','Access');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('7','AuditableObjectLifecycle','Deidentification');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('8','AuditableObjectLifecycle','Aggregation');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('9','AuditableObjectLifecycle','Report');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('10','AuditableObjectLifecycle','Export');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('11','AuditableObjectLifecycle','Disclosure');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('12','AuditableObjectLifecycle','Receipt of Disclosure');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('13','AuditableObjectLifecycle','Archiving');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('14','AuditableObjectLifecycle','Logical Deletion');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('15','AuditableObjectLifecycle','Permanent Erasure');

INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('1','RFC-3881','Medical Record');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('2','RFC-3881','Patient Number');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('3','RFC-3881','Encounter Number');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('4','RFC-3881','Enrollee Number');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('5','RFC-3881','Social Security Number');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('6','RFC-3881','Account Number');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('7','RFC-3881','Guarantor Number');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('8','RFC-3881','Report Name');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('9','RFC-3881','Report Number');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('10','RFC-3881','Search Critereon');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('11','RFC-3881','User Identifier');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('12','RFC-3881','Uri');

INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:a54d6aa5-d40d-43f9-88c5-b4633d873bdd','IHE XDS Meta Data','Submission Set');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:a7058bb9-b4e4-4307-ba5b-e3f0ab85e12d','IHE XDS Meta Data','Submission Set Author');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:aa543740-bdda-424e-8c96-df4873be8500','IHE XDS Meta Data','Submission Set Content Type');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:96fdda7c-d067-4183-912e-bf5ee74998a8','IHE XDS Meta Data','Submission Set Unique Id');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:7edca82f-054d-47f2-a032-9b2a5b5186c1','IHE XDS Meta Data','Document Entry');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:93606bcf-9494-43ec-9b4e-a7748d1a838d','IHE XDS Meta Data','Document Entry Author');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:41a5887f-8865-4c09-adf7-e362475b143a','IHE XDS Meta Data','Document Entry Class Code');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:f4f85eac-e6cb-4883-b524-f2705394840f','IHE XDS Meta Data','Document Entry Confidentiality Code');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:2c6b8cb7-8b2a-4051-b291-b1ae6a575ef4','IHE XDS Meta Data','Document Entry Event Code List');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:a09d5840-386c-46f2-b5ad-9c3699a4309d','IHE XDS Meta Data','Document Entry Format Code');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:f33fb8ac-18af-42cc-ae0e-ed0b0bdb91e1','IHE XDS Meta Data','Document Entry Health Care Fcility Type');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:58a6f841-87b3-4a3e-92fd-a8ffeff98427','IHE XDS Meta Data','Document Entry Patient Id');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:cccf5598-8b07-4b77-a05e-ae952c785ead','IHE XDS Meta Data','Document Entry Practice Setting');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:f0306f51-975f-434e-a61c-c59651d33983','IHE XDS Meta Data','Document Entry Type Code');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:2e82c1f6-a085-4c72-9da3-8640a32e42ab','IHE XDS Meta Data','Document Entry Unique Id');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:d9d542f3-6cc4-48b6-8870-ea235fbc94c2','IHE XDS Meta Data','Folder');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:1ba97051-7806-41a8-a48b-8fce7af683c5','IHE XDS Meta Data','Folder Code List');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:f64ffdf0-4b97-4e06-b79f-a52b38ec2f8a','IHE XDS Meta Data','Folder Patient Id');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:75df8f67-9973-4fbe-a900-df66cefecc5a','IHE XDS Meta Data','Folder Unique Id');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:917dc511-f7da-4417-8664-de25b34d3def','IHE XDS Meta Data','Append');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:60fd13eb-b8f6-4f11-8f28-9ee000184339','IHE XDS Meta Data','Replacement');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:ede379e6-1147-4374-a943-8fcdcf1cd620','IHE XDS Meta Data','Transform');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:b76a27c7-af3c-4319-ba4c-b90c1dc45408','IHE XDS Meta Data','Transform / Replace');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:8ea93462-ad05-4cdc-8e54-a8084f6aff94','IHE XDS Meta Data','Sign');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('urn:uuid:10aa1a4b-715a-4120-bfd0-9760414112c8','IHE XDS Meta Data','Document Entry Stub');

INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-1','IHE Transactions','Maintain Time');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-2','IHE Transactions','Get User Authentication');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-3','IHE Transactions','Get Service Ticket');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-4','IHE Transactions','Kerberized Communication');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-5','IHE Transactions','Join Context');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-6','IHE Transactions','Change Context');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-7','IHE Transactions','Leave Context');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-8','IHE Transactions','Patient Identity Feed');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-9','IHE Transactions','PIX Query');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-10','IHE Transactions','PIX Update Notification');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-11','IHE Transactions','Retrieve Specific Information for Display');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-12','IHE Transactions','Retrieve Document for Display');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-13','IHE Transactions','Follow Context');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-14','IHE Transactions','Register Document Set');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-15','IHE Transactions','Provide and Register Document Set');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-16','IHE Transactions','Query Registry');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-17','IHE Transactions','Retrieve Documents');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-18','IHE Transactions','Registry Stored Query');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-19','IHE Transactions','Authenticate Node');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-20','IHE Transactions','Record Audit Event');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-21','IHE Transactions','Patient Demographics Query');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-22','IHE Transactions','Patient Demographics and Visit Query');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-23','IHE Transactions','Find Personnel White Pages');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-24','IHE Transactions','Query Personnel White Pages');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-30','IHE Transactions','Patient Identity Management');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-31','IHE Transactions','Patient Encounter Management');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-32','IHE Transactions','Distribute Document Set on Media');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-38','IHE Transactions','Cross Gateway Query');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-39','IHE Transactions','Cross Gateway Retrieve');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-40','IHE Transactions','Provide X-User Assertion');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-41','IHE Transactions','Provide and Register Document Set-b');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-42','IHE Transactions','Register Document Set-b');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-43','IHE Transactions','Retrieve Document Set');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-44','IHE Transactions','Patient Identity Feed HL7v3');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-45','IHE Transactions','PIXv3 Query');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-46','IHE Transactions','PIXv3 Update Notification');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-47','IHE Transactions','Patient Demographics Query HL7v3');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('ITI-51','IHE Transactions','Multi-Patient Stored Query');

INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('1','NetworkAccessPointType','Machine Name');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('2','NetworkAccessPointType','IP Address');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('3','NetworkAccessPointType','Telephone Number');

INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('C','ActionType','Create');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('R','ActionType','Read');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('U','ActionType','Update');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('D','ActionType','Delete');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('E','ActionType','Execute');

INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('0','OutcomeIndicator','Success');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('4','OutcomeIndicator','Minor Fail');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('8','OutcomeIndicator','Serious Fail');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('12','OutcomeIndicator','Major Fail');

INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('IHE0001','IHE','Provisioning Event');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('IHE0002','IHE','Medication Event');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('IHE0003','IHE','Resource Assignment');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('IHE0004','IHE','Care Episode');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('IHE0005','IHE','Care Protocol');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('IHE0006','IHE','Disclosure');

INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('CDT-100002','CDT', 'Patient Search Activity');

INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110100','DCM','Application Activity');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110101','DCM','Audit Log Used');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110102','DCM','Begin Transferring DICOM Instances ');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110103','DCM','DICOM Instances Accessed');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110104','DCM','DICOM Instances Transferred');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110105','DCM','DICOM Study Deleted');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110106','DCM','Export');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110107','DCM','Import');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110108','DCM','Network Activity');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110109','DCM','Order Record');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110110','DCM','Patient Record');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110111','DCM','Procedure Record');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110112','DCM','Query');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110113','DCM','Security Alert');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110114','DCM','User Authentication');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110120','DCM','Application Start');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110121','DCM','Application Stop');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110122','DCM','Login');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110123','DCM','Logout');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110124','DCM','Attach');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110125','DCM','Detach');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110126','DCM','Node Authentication');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110127','DCM','Emergency Override Started');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110132','DCM','Use of a restricted function');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110135','DCM','Object Security Attributes Changed');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110136','DCM','Security Roles Changed');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110137','DCM','User security Attributes Changed');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110153','DCM','Source');
INSERT INTO AuditCode (Mnemonic, Domain, DisplayName) VALUES ('110152','DCM','Destination');
GO

-- CREATE AUDIT CODE IF NOT ALREADY EXISTS
CREATE PROCEDURE sp_CreateAuditCodeIfNotExists(
	@DomainIn	AS VARCHAR(48), 
	@MnemonicIn	AS VARCHAR(48),
	@DisplayNameIn AS VARCHAR(48),
	@CodeIdOut AS INTEGER OUTPUT
) AS BEGIN
	DECLARE @ExistingCode AS TABLE(
		CodeId	INTEGER
	);
	INSERT INTO @ExistingCode SELECT CodeId FROM AuditCode WHERE Mnemonic = @MnemonicIn AND Domain = @DomainIn;
	
	IF NOT EXISTS (SELECT * FROM @ExistingCode) 
	BEGIN
		INSERT INTO AuditCode (Domain, Mnemonic, DisplayName) VALUES (@DomainIn, @MnemonicIn, @DisplayNameIn);
		SELECT @CodeIdOut = SCOPE_IDENTITY();
	END
	ELSE
		SELECT @CodeIdOut = (SELECT TOP 1 CodeId FROM @ExistingCode);
END
RETURN;

GO

-- GET AN AUDIT CODE IDENTIFIER FROM THE SPECIFIED DOMAIN
CREATE FUNCTION fn_GetAuditCodeId(
	@DomainIn AS VARCHAR,
	@MnemonicIn AS VARCHAR
) RETURNS INTEGER AS BEGIN
	RETURN (SELECT TOP 1 CodeId FROM AuditCode WHERE Mnemonic = @MnemonicIn AND Domain = @DomainIn);
END;

GO

-- TABLE: NODE VERSION
-- TRACKS THE INFORMATION ABOUT THE NODE OVER TIME
CREATE TABLE NodeVersion
(
	NodeVersionId INTEGER NOT NULL IDENTITY(1,1), -- A UNIQUE IDENTIFIER FOR THE VERSION
	NodeId INTEGER NOT NULL, -- THE IDENTIFIER OF THE NODE TO WHICH THIS VERSION APPLIES
	Name VARCHAR(MAX), -- A FRIENDLY NAME FOR THE NODE
	HostName VARCHAR(MAX), -- THE HOST URL/SCHEME
	NodeMagic VARBINARY(MAX),  -- THE NODE IDENTIFIER USED IN AUTHENTICATION TOKENS
	StatusCodeId INTEGER NOT NULL DEFAULT 0, -- THE STATUS OF THE NODE
	CreationTimestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP, -- THE CREATION TIME OF THIS VERSION
	ObsoletionTime DATETIME, -- THE TIME THIS RECORD WAS OBSOLETED
	CONSTRAINT PK_NodeVersion PRIMARY KEY (NodeVersionId),
	CONSTRAINT FK_NodeVersion_Node FOREIGN KEY (NodeId) REFERENCES Node(NodeId),
	CONSTRAINT FK_StatusCode_StatusCode FOREIGN KEY (StatusCodeId) REFERENCES StatusCode(CodeId)
);

-- TABLE: AUDIT SESSION
-- TRACKS A SESSION WITH A REMOTE HOST
CREATE TABLE AuditSession
(
	SessionId UNIQUEIDENTIFIER NOT NULL, -- UNIQUE IDENTIFIER FOR THE SESSION
	ReceiverNodeVersionId INTEGER NOT NULL, -- THE ID OF THE RECEIVING NODE
	SenderNodeVersionId INTEGER NOT NULL, -- THE ID OF THE SENDING NODE
	CreationTimestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP, -- THE TIME THAT THE SESSION WAS CREATED
	CONSTRAINT PK_AuditSession PRIMARY KEY (SessionId),
	CONSTRAINT FK_ReceiverNodeVersionId FOREIGN KEY (ReceiverNodeVersionId) REFERENCES NodeVersion(NodeVersionId),
	CONSTRAINT FK_SenderNodeVersionId FOREIGN KEY (SenderNodeVersionId) REFERENCES NodeVersion(NodeVersionId)
)

-- TABLE: AUDIT TABLE
-- TRACKS THE AUDITS SENT TO THE VISUALIZER
CREATE TABLE Audit
(
	AuditId	INTEGER NOT NULL IDENTITY(1,1), -- UNIQUE IDENTIFIER FOR THE AUDIT 
	GlobalId UNIQUEIDENTIFIER NOT NULL, -- UUID FOR THE AUDIT
	ActionCodeId INTEGER NOT NULL, -- THE CODE CONTAINING THE ACTION
	OutcomeCodeId INTEGER NOT NULL, -- THE CODE CONTAINING THE OUTCOME
	EventCodeId INTEGER NOT NULL, -- THE EVENT CODE
	EventTimestamp DATETIME NOT NULL, -- THE TIME THE EVENT OCCURRED
	CreationTimestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP, -- THE TIMESTAMP OF THE EVENT
	SessionId UNIQUEIDENTIFIER NOT NULL, -- THE UUID OF THE AUDIT CORRELATION
	ProcessName VARCHAR(MAX) NOT NULL, -- THE NAME OF THE PROCESS
	CONSTRAINT PK_Audit PRIMARY KEY (AuditId),
	CONSTRAINT FK_ActionCodeId_Code FOREIGN KEY (ActionCodeId) REFERENCES AuditCode(CodeId),
	CONSTRAINT FK_OutcomeCodeId_Code FOREIGN KEY (OutcomeCodeId) REFERENCES AuditCode(CodeId),
	CONSTRAINT FK_EventCodeId_Code FOREIGN KEY (EventCodeId) REFERENCES AuditCode(CodeId),
	CONSTRAINT FK_AuditSession FOREIGN KEY (SessionId) REFERENCES AuditSession(SessionId)
);

-- INDEX: LOOKUP BY ACTION CODE OR OUTCOME
CREATE INDEX Audit_ActionCodeIdIdx ON Audit(ActionCodeId);
CREATE INDEX Audit_OutcomeCodeIdIdx ON Audit(OutcomeCodeId);


-- TABLE: TRACK AUDIT PARTICIPANTS
-- TRACKS THE PARTICIPANTS IN AN AUDIT
CREATE TABLE AuditParticipant
(
	ParticipantId INTEGER NOT NULL IDENTITY(1,1), -- UNIQUE ISENTIFIER FOR THE PARTICIPANT
	AuditId INTEGER NOT NULL, -- THE AUDIT TO WHICH THE PARTICIPANT RECORD APPLIES
	UserId VARCHAR(512), -- THE LINKED USER THAT IS REPRESENTED IN THE RECORD
	NodeVersionId INTEGER, -- THE LINKED NODE VERSION IN THE RECORD
	RawUserId VARCHAR(MAX), -- THE USER IDENTIFIER AS IT APPEARED ON THE AUDIT
	RawUserName VARCHAR(MAX), -- THE USER NAME AS IT APPEARED ON THE AUDIT
	IsRequestor BIT NOT NULL DEFAULT 0, -- TRUE IF THE PARTICIPANT INITIATED THE REQUEST
	NetworkAccessPoint VARCHAR(MAX), -- THE IP ADDRESS OF THE PARITICIPANT
	CONSTRAINT PK_AuditParticipant PRIMARY KEY (ParticipantId),
	CONSTRAINT FKAuditParticipant_NodeVersion FOREIGN KEY (NodeVersionId) REFERENCES NodeVersion(NodeVersionId),
	CONSTRAINT FK_AuditParticipant_Audit FOREIGN KEY (AuditId) REFERENCES Audit(AuditId)

);

-- INDEX: LOOKUP PARTICIPANT BY AUDIT ID, USER ID or NODE VERSION
CREATE INDEX AuditParticipant_AuditIdIdx ON AuditParticipant(AuditId);
CREATE INDEX AuditParticipant_UserIdIdx ON AuditParticipant(UserId);
CREATE INDEX AuditParticipant_NodeVersionIdx ON AuditParticipant(NodeVersionId);

-- TABLE: AUDIT STATUS TABLE
-- TRACKS THE STATUS OF AN AUDIT OVER TIME
CREATE TABLE AuditStatus
(
	AuditVersionId INTEGER NOT NULL IDENTITY(1,1), -- UNIQUE IDENTIFIER FOR THE CHANGE TO THE AUDIT STATUS
	AuditId INTEGER NOT NULL, -- THE IDENTIFIER OF THE AUDIT FOR WHICH THIS CHANGE APPLIES
	StatusCodeId INTEGER NOT NULL, -- THE STATUS CODE TO WHICH THE NEW AUDIT IS TARGETED
	IsAlert BIT DEFAULT 0,
	CreationTimestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP, -- THE TIME THE STATUS VERSION WAS CREATED
	ObsoletionTimestamp DATETIME, -- THE TIME WHEN THE STATUS WAS NO LONGER VALID
	ModifiedBy VARCHAR(MAX) NOT NULL, -- MODIFIED BY
	CONSTRAINT PK_AuditStatus PRIMARY KEY (AuditVersionId),
	CONSTRAINT FK_AuditId_Audit FOREIGN KEY (AuditId) REFERENCES Audit(AuditId),
	CONSTRAINT FK_StatusCodeId_StatusCode FOREIGN KEY (StatusCodeId) REFERENCES StatusCode(CodeId)
);

-- INDEX: LOOKUP AUDIT STATUS BY AUDIT ID
CREATE INDEX AuditStatus_AuditIdIdx ON AuditStatus(AuditId);


-- TABLE: AUDIT SOURCES TABLE
-- TRACKS AN AUDIT SOURCE'S ENTERPRISE SITE AND SOURCE
CREATE TABLE AuditSource
(
	AuditSourceId INTEGER NOT NULL IDENTITY(1,1), -- A UNIQUE IDENTIFIER FOR THE AUDIT SOURCE RECORD
	EnterpriseSiteName VARCHAR(MAX), -- THE NAME OF THE ENTERPRISE SITE
	AuditSourceName VARCHAR(MAX), -- THE NAME OF THE AUDIT SOURCE
	CONSTRAINT PK_AuditSource PRIMARY KEY (AuditSourceId)
);

-- TRACKS A RELATIONSHIP BETWEEN THE AUDIT SOURCE AND TYPES
CREATE TABLE AuditSourceTypeAssoc
(
	AuditSourceId INTEGER NOT NULL, -- THE AUDIT SOURCE TO WHICH THE ASSOCIATION APPLIES
	CodeId INTEGER NOT NULL, -- THE CODE OF WHICH THE AUDIT SOURCE IS
	CONSTRAINT PK_AuditSourceTypeAssoc PRIMARY KEY(AuditSourceId, CodeId),
	CONSTRAINT FK_AuditSourceTypeAssoc_AuditSource FOREIGN KEY (AuditSourceId) REFERENCES AuditSource(AuditSourceId),
	CONSTRAINT FK_AuditSourceTypeAssoc_Code FOREIGN KEY (CodeId) REFERENCES AuditCode(CodeId)
);

-- TABLE: AUDIT SOURCE TO AUDIT ASSOCIATION TABLE
-- TRACKS THE RELATIONSHIP BETWEEN AN AUDIT SOURCE AND AN AUDIT MESSAGE
CREATE TABLE AuditAuditSourceAssoc
(
	AuditSourceId INTEGER NOT NULL, -- THE AUDIT SOURCE TO WHICH THE ASSOCIATION APPLIES,
	AuditId INTEGER NOT NULL, -- THE IDENTIFIER OF THE AUDIT TO WHICH THE ASSOCIATION APPLIES
	CONSTRAINT PK_AuditAuditSourceAssoc PRIMARY KEY (AuditSourceId, AuditId),
	CONSTRAINT FK_AuditAuditSourceAssoc_AuditSource FOREIGN KEY (AuditSourceId) REFERENCES AuditSource(AuditSourceId),
	CONSTRAINT FK_AuditAuditSourceAssoc_Audit FOREIGN KEY (AuditId) REFERENCES Audit(AuditId)
);

-- INDEX: LOOKUP AUDIT SOURCE BY AUDIT ID
CREATE INDEX AuditAuditSourceAssoc_AuditIdIdx ON AuditAuditSourceAssoc(AuditId);


-- TABLE: AUDIT EVENT TYPE CODE ASSOCIATION TABLE
-- TRACKS THE RELATIONSHIP BETWEEN AN AUDIT AND TYPE CODES
CREATE TABLE AuditEventTypeAuditCodeAssoc
(
	AuditId INTEGER NOT NULL, -- THE IDENTIFIER OF THE AUDIT TO WHICH THE ASSOCIATION APPLIES,
	CodeId INTEGER NOT NULL, -- THE IDENTIFIER OF THE CODE WHICH CARRIES MEANING IN RELATION TO THE EVENT TYPE
	CONSTRAINT PK_AuditEventTypeAuditCodeAssoc PRIMARY KEY (AuditId, CodeId),
	CONSTRAINT FK_AuditEventTypeAuditCodeAssoc_Audit FOREIGN KEY (AuditId) REFERENCES Audit(AuditId),
	CONSTRAINT FK_AuditEventTypeAuditCodeAssoc_Code FOREIGN KEY (CodeId) REFERENCES AuditCode(CodeId)
);


-- INDEX: LOOKUP EVENT TYPE BY CODE OR AUDIT
CREATE INDEX AuditEventTypeAuditCodeAssoc_AuditIdIdx ON AuditEventTypeAuditCodeAssoc(AuditId);
CREATE INDEX AuditEventTypeAuditCodeAssoc_CodeIdIdx ON AuditEventTypeAuditCodeAssoc(CodeId);


-- TABLE: AUDIT PARTICIPANT ROLE CODE ASSOCIATION
-- TRACKS AN ASSOCIATION BETWEEN AN AUDIT ACTIVE PARTICIPANT AND THE ROLE CODES
CREATE TABLE AuditParticipantRoleCodeAssoc
(
	ParticipantId INTEGER NOT NULL, -- THE IDENTIFIER OF THE PARTICIPANT TO WHICH THE ASSOCIATION APPLIES
	CodeId INTEGER NOT NULL, -- THE IDENTIFIER FOR THE ROLE CODE
	CONSTRAINT PK_AuditParticipantRoleCodeAssoc PRIMARY KEY (ParticipantId, CodeId),
	CONSTRAINT FK_AuditParticipantRoleCodeAssoc_AuditParticipant FOREIGN KEY (ParticipantId) REFERENCES AuditParticipant(ParticipantId),
	CONSTRAINT FK_AuditParticipantRoleCodeAssoc_AuditCode FOREIGN KEY (CodeId) REFERENCES AuditCode(CodeId)
);

-- INDEX: LOOKUP PARTICIPANT ROLE CODE BY PARTICIPANT ID
CREATE INDEX AuditParticipantRoleCodeAssoc_ParticipantIdIdx ON AuditParticipantRoleCodeAssoc(ParticipantId);
CREATE INDEX AuditParticipantRoleCodeAssoc_CodeIdIdx ON AuditParticipantRoleCodeAssoc(CodeId);


-- TABLE: AUDIT PARTICIPANT OBJECTS DETAIL
-- TRACKS DETAILS RELATED TO THE OBJECTS THAT WERE UPDATED, DISCLOSED, OR CREATED FOR THIS AUDIT
CREATE TABLE AuditObject 
(
	ObjectId INTEGER NOT NULL IDENTITY(1,1), -- A UNIQUE IDENTIFIER FOR THE OBJECT
	AuditId INTEGER NOT NULL, -- THE AUDIT TO WHICH THE OBJECT APPLIES
	ExternalIdentifier VARCHAR(MAX) NOT NULL, -- THE EXTERNAL IDENTIFIER (ID) OF THE PARTICIPANT OBJECT
	TypeCodeId INTEGER NOT NULL, -- THE TYPE OF OBJECT REFERENCED
	RoleCodeId INTEGER NOT NULL, -- THE ROLE OF THE OBJECT IN THE EVENT
	LifecycleCodeId INTEGER, -- THE LIFECYCLE OF THE OBJECT IF APPLICABLE
	IdTypeCodeId INTEGER, -- SPECIFIES THE TYPE CODE FOR THE OBJECT
	ObjectSpec TEXT, -- SPECIFIES ADDITIONAL DATA ABOUT THE OBJECt
	ObjectSpecType CHAR(1), -- SPECIFIES THE TYPE OF DATA CONTAINED IN THE OBJECTSPEC FIELD
	CONSTRAINT PK_AuditObject PRIMARY KEY (ObjectId),
	CONSTRAINT FK_AuditObject_TypeCode_Code FOREIGN KEY (TypeCodeId) REFERENCES AuditCode(CodeId),
	CONSTRAINT FK_AuditObject_LifecycleCode_Code FOREIGN KEY (LifecycleCodeId) REFERENCES AuditCode(CodeId),
	CONSTRAINT FK_AuditObject_RoleCodeId_Code FOREIGN KEY (RoleCodeId) REFERENCES AuditCode(CodeId),
	CONSTRAINT FK_AuditObject_IdTypeCodeId_Code FOREIGN KEY (IdTypeCodeId) REFERENCES AuditCode(CodeId),
	CONSTRAINT FK_AuditObject_Audit FOREIGN KEY (AuditId) REFERENCES Audit(AuditId),
	CONSTRAINT CK_AuditObject_ObjectSpecType CHECK (ObjectSpecType IN ('N','Q'))
);

-- INDEX: LOOKUP OBJECT BY AUDIT ID
CREATE INDEX AuditObject_AuditIdIdx ON AuditObject(AuditId);


-- TABLE: AUDIT OBJECT DETAILS TABLE
-- TRACKS ADDITIONAL DETAIL ABOUT AN AUDITABLE OBJECT
CREATE TABLE AuditObjectDetail
(
	ObjectDetailId INTEGER NOT NULL IDENTITY(1,1), -- A UNIQUE IDENTIFIER FOR THE OBJECT DETAIL LINE
	ObjectId INTEGER NOT NULL, -- IDENTIFIES THE AUDITABLE OBJECT TO WHICH THE DETAIL BELONGS
	DetailType VARCHAR(MAX) NOT NULL, -- IDENTIFIES THE TYPE OF DETAIL
	DetailValue VARBINARY(MAX) NOT NULL, -- IDENTIFIES THE ADDITIONAL DETAIL DATA
	CONSTRAINT PK_AuditObjectDetail PRIMARY KEY (ObjectDetailId),
	CONSTRAINT FK_AuditObjectDetail_AuditableObject FOREIGN KEY (ObjectId) REFERENCES AuditObject(ObjectId)
);

-- INDEX: LOOKUP OBJECT DETAIL BY OBJECT ID
CREATE INDEX AuditObjectDetail_ObjectIdIdx ON AuditObjectDetail(ObjectId);

-- TABLE: AUDIT ERROR
-- STORES AUDIT ERRORS 
CREATE TABLE AuditError
(
	ErrorId INTEGER NOT NULL IDENTITY(1,1), -- A UNIQUE IDENTIFIER FOR THE ERROR
	SessionId UNIQUEIDENTIFIER NOT NULL, -- THE SESSION IN WHICH THE AUDIT WAS COLLECTED
	ErrorMessage VARCHAR(MAX) NOT NULL, -- THE ERROR MESSAGE
	AuditMessageId UNIQUEIDENTIFIER, -- THE MESSAGE ID
	StackTrace VARCHAR(MAX), -- THE STACK TRACE OF ANY EXCEPTION WHICH CAUSED THE ERROR
	CausedById INTEGER, -- THE CAUSE OF THIS ERROR (IF APPLICABLE)
	CONSTRAINT PK_AuditError PRIMARY KEY (ErrorId), 
	CONSTRAINT FK_SessionId FOREIGN KEY (SessionId) REFERENCES AuditSession(SessionId),
	CONSTRAINT FK_CausedBy FOREIGN KEY (CausedById) REFERENCES AuditError(ErrorId)
)

GO

-- AUDIT VIEW: DEIDENTIFIED DATA
CREATE VIEW AuditSummaryVw AS
	SELECT Audit.AuditId, Audit.CreationTimestamp, Audit.EventTimestamp, EventCode.DisplayName AS EventCode, ActionCode.DisplayName AS ActionCode, OutcomeCode.DisplayName AS OutcomeCode, StatusCode.Name AS StatusCode, AuditStatus.IsAlert, EventType.DisplayName AS EventType FROM 
		Audit LEFT JOIN AuditCode AS EventCode ON (EventCode.CodeId = Audit.EventCodeId)
		LEFT JOIN AuditCode AS ActionCode ON (ActionCode.CodeId = Audit.ActionCodeId)
		LEFT JOIN AuditCode AS OutcomeCode ON (OutcomeCode.CodeId = Audit.OutcomeCodeId) 
		LEFT JOIN AuditEventTypeAuditCodeAssoc ON (AuditEventTypeAuditCodeAssoc.AuditId = Audit.AuditId) 
		INNER JOIN AuditCode AS EventType ON (EventType.CodeId = AuditEventTypeAuditCodeAssoc.CodeId)
		LEFT JOIN AuditStatus ON (AuditStatus.AuditId = Audit.AuditId)
		INNER JOIN StatusCode ON (AuditStatus.StatusCodeId = StatusCode.CodeId)
		WHERE AuditStatus.ObsoletionTimestamp IS NULL;
	
GO
		
CREATE VIEW AuditStatusCodeSummaryVw AS
	SELECT StatusCode.CodeID, StatusCode.Name, (SELECT COUNT(DISTINCT AuditId) FROM AuditStatus WHERE StatusCode.CodeId = AuditStatus.StatusCodeId AND AuditStatus.ObsoletionTimestamp IS NULL) AS nAudits FROM StatusCode;

GO

CREATE VIEW AuditParticipantSummaryVw AS
	SELECT AuditParticipant.*, AuditCode.Mnemonic AS RoleCodeMnemonic, AuditCode.DisplayName  AS RoleCodeDisplay FROM AuditParticipant LEFT JOIN AuditParticipantRoleCodeAssoc ON (AuditParticipant.ParticipantId = AuditParticipantRoleCodeAssoc.ParticipantId)
	INNER JOIN AuditCode ON (AuditParticipantRoleCodeAssoc.CodeId = AuditCode.CodeId);

GO

CREATE VIEW AuditObjectSummaryVw AS
	SELECT AuditObject.*, IdTypeCode.Mnemonic AS IdTypeCodeMnemonic, IdTypeCode.Domain AS IdTypeCodeDomain, IdTypeCode.DisplayName AS IdTypeCodeDisplayName, RoleCode.DisplayName AS RoleCode, TypeCode.DisplayName AS TypeCode, LifecycleCode.DisplayName AS LifecycleCode FROM AuditObject INNER JOIN AuditCode AS IdTypeCode ON (AuditObject.IdTypeCodeId = IdTypeCode.CodeId)
		LEFT JOIN AuditCode AS TypeCode ON (AuditObject.TypeCodeId = Typecode.CodeId)
		LEFT JOIN AuditCode AS LifecycleCode ON (AuditObject.LifecycleCodeId = LifecycleCode.CodeId)
		LEFT JOIN AuditCode AS RoleCode ON (AuditObject.RoleCodeId = RoleCode.CodeId);

GO 

/*
CREATE VIEW AuditDetailVw AS
	SELECT 
		AuditSummaryVw.*, 
		AuditParticipantSummaryVw.ParticipantId, 
		AuditParticipantSummaryVw.NetworkAccessPoint, 
		AuditParticipantSummaryVw.RawUserId AS PtcptUserId, 
		AuditParticipantSummaryVw.RawUserName AS PtcptUserName,
		AspNetUsers.Id AS PtcptAspNetId, 
		AspNetUsers.FirstName AS PtcptFirstName, 
		AspNetUsers.LastName AS PtcptLastName, 
		AspNetUsers.Email AS PtcptEmail, 
		NodeVersion.Name AS PtcptDeviceName, 
		AuditParticipantSummaryVw.RoleCodeDisplay AS PtcptObjectRoleCodeDisplay, 
		AuditParticipantSummaryVw.RoleCodeMnemonic AS PtcptObjectRoleCodeMnemonic, 
		AuditObjectSummaryVw.ExternalIdentifier AS ObjExternalIdentifier,
		AuditObjectSummaryVw.TypeCode AS ObjTypeCode,
		AuditObjectSummaryVw.LifecycleCode AS ObjLifecycle,
		AuditObjectSummaryVw.RoleCode AS ObjRoleCode,
		AuditObjectSummaryVw.IdTypeCodeDisplayName AS ObjIdTypeCode,
		AuditObjectSummaryVw.ObjectSpec
	FROM AuditSummaryVw LEFT JOIN AuditParticipantSummaryVw ON (AuditParticipantSummaryVw.AuditId = AuditSummaryVw.AuditId)
		LEFT JOIN AuditObjectSummaryVw ON (AuditObjectSummaryVw .AuditId = AuditSummaryVw.AuditId)
		LEFT JOIN AspNetUsers ON (AuditParticipantSummaryVw.UserId = AspNetUsers.Id)
		LEFT JOIN NodeVersion ON (AuditParticipantSummaryVw.NodeVersionId = NodeVersion.NodeVersionId)
*/
GO

CREATE PROCEDURE sp_SetAuditStatus(@AuditIdIn INTEGER, @StatusIn VARCHAR(28), @IsAlert BIT, @VersionId INTEGER OUTPUT) AS
DECLARE
	@StatusId INTEGER,
	@AsAlert BIT
BEGIN

	SET @AsAlert = (SELECT IsAlert FROM AuditStatus WHERE AuditStatus.AuditVersionId = (SELECT TOP 1 AuditVersionId FROM AuditStatus WHERE AuditId = @AuditIdIn ORDER BY CreationTimestamp DESC));
	SET @StatusId = (SELECT CodeId FROM StatusCode WHERE Name = @StatusIn);
	UPDATE AuditStatus SET ObsoletionTimestamp = CURRENT_TIMESTAMP WHERE AuditId = @AuditIdIn AND ObsoletionTimestamp IS NULL;
	INSERT INTO AuditStatus(AuditId, IsAlert, StatusCodeId) VALUES (@AuditIdIn, COALESCE(@IsAlert, @AsAlert), @StatusId);
	SET @VersionId = SCOPE_IDENTITY();
END;

SELECT * FROM AuditParticipantSummaryVw;
SELECT * FROM AuditObjectSummaryVw;
