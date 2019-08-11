IF (NOT EXISTS(SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
				WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'MasturbationRecorderUser'))
BEGIN
	CREATE TABLE dbo.MasturbationRecorderUser(
		UserId INT PRIMARY KEY IDENTITY(0,1),
		UserName NCHAR(128) CHECK(LEN(UserName) > 0) UNIQUE NOT NULL,
		Password CHAR(256) CHECK(LEN(Password) >= 8) NOT NULL,
		PersonData image
	)
END
ELSE
BEGIN
	IF (NOT EXISTS(SELECT UserName FROM dbo.MasturbationRecorderUser))
	BEGIN
		INSERT INTO dbo.MasturbationRecorderUser (UserName,Password) 
		VALUES('Janyee Liang', 'ddbc4578'),
		      ('Phoebe Kilminster', 'phoebe001234'),
		      ('Reina Stamm', 'edc54gbyuio'),
		      ('Lsm', '1234567890AXC'),
		      ('Han Solo', 'Wow!!Shit007')

		INSERT INTO dbo.MasturbationRecorderUser (UserName,Password,PersonData)
		VALUES('Sherlock Holemes','sH..TheGameIsOn!',(SELECT BulkColumn FROM Openrowset( Bulk 'C:\Users\a124p\Pictures\Status.png', Single_Blob) as img))
	END
END