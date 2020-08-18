
CREATE TABLE Tbl_StateMaster
(
	StateID INT Primary Key Identity (1,1),
	StateName VARCHAR(50)
) 

GO

CREATE TABLE Tbl_CityMaster
(
	CityID INT Primary Key Identity (1,1),
	CityName VARCHAR(50),
	StateID INT References Tbl_StateMaster (StateID)
) 

CREATE TABLE Tbl_Registration
(
	ID INT Primary Key Identity (1,1),
	Name VARCHAR(50),
	Gender VARCHAR(1),
	Qualification VARCHAR(100),
	Age INT,
	CityID  INT References Tbl_CityMaster (CityID),
	StateID  INT References Tbl_StateMaster (StateID),
	EmailID VARCHAR(100),
	Pass VARCHAR(50),
	ImageName VARCHAR(50),
	ImagePath VARCHAR(50),
	EntryDate Datetime,
	UpdateDate Datetime,
)

GO



create proc Usp_DeleteRegistrationDetails
(
	@ID INT
)
AS
BEGIN
	DELETE FROM Tbl_Registration
	WHERE ID=@ID
END

GO

INSERT INTO Tbl_StateMaster
SELECT 'Gujarat'
Union
SELECT 'Maharastra'
Union
SELECT 'Rajsthan'


GO

INSERT INTO Tbl_CityMaster
SELECT 'Surat',1
Union
SELECT 'Baroda',1
Union
SELECT 'Mumbai',2
Union
SELECT 'Nagpur',2
Union
SELECT 'Jaipur',3
Union
SELECT 'Udaipur',3

GO

create proc Usp_GetRegistrationDetails
as
begin
	Select ID,Name,Gender=(Case WHEN R.Gender='M' THEN 'Male' WHEN R.Gender='F' THEN 'Female' END) ,
		Qualification,Age,EmailID,Pass,ImageName,ImagePath,EntryDate,
		ST.StateName,CI.CityName
	FROM Tbl_Registration R
	LEFT JOIN Tbl_StateMaster ST ON ST.StateID=R.StateID
	LEFT JOIN Tbl_CityMaster CI ON CI.CityID=R.CityID
end

GO

create proc Usp_GetRegistrationDetailsByID
(
	@ID INT
)
as
begin
	Select ID,Name,Gender,Qualification,Age,StateID,CityID,EmailID,Pass,ImageName,ImagePath,EntryDate
	FROM Tbl_Registration R
	WHERE ID=@ID
end

GO

/*
exec Usp_AddEditRegisterDetails
'<RegisterModel>
<ID>0</ID>
<Name>AKhi</Name>
<Gender>M</Gender>
<Qualification>10</Qualification>
<Age>22</Age>
<CityID>4</CityID>
<StateID>2</StateID>
<EmailID>ask@gmail.com</EmailID>
<Pass>123</Pass>
<ConfirmPass>123</ConfirmPass>
<ImageName>2020-08-16-22-52-55_000_Wondershare.jpg</ImageName>
<ImagePath>~/UploadedImages/2020-08-16-22-52-55_000_Wondershare.jpg</ImagePath>
<ActionType>Add</ActionType>
</RegisterModel>'

exec Usp_AddEditRegisterDetails
'<RegisterModel>
	<ID>0</ID><Name>akhilesh</Name><Gender>M</Gender><Qualification>test</Qualification><Age>22</Age><CityID>3</CityID><StateID>2</StateID><EmailID>abcdf@gmail.com</EmailID><Pass>123</Pass><ActionType>Add</ActionType>
</RegisterModel>'

*/
alter proc Usp_AddEditRegisterDetails
(
	@XML XML
)
AS
BEGIN
	DECLARE @ActionType VARCHAR(10),@Message VARCHAR(100)

	DECLARE @Temp TABLE
	(
		ID INT,
		Name VARCHAR(50),
		Gender VARCHAR(1),
		Qualification VARCHAR(100),
		Age INT,
		CityID  INT ,
		StateID  INT ,
		EmailID VARCHAR(100),
		Pass VARCHAR(50),
		ImageName VARCHAR(50),
		ImagePath VARCHAR(50),
		ActionType VARCHAR(10)
	)

	INSERT INTO @Temp (ID,Name,Gender,Qualification,Age,CityID,StateID,EmailID,Pass,ImageName,ImagePath,ActionType)
	SELECT X.ID,X.Name,X.Gender,X.Qualification,X.Age,X.CityID,X.StateID,X.EmailID,X.Pass,X.ImageName,X.ImagePath,X.ActionType
	FROM
	(
		SELECT
			t.value('ID[1]','INT') AS ID,
			t.value('Name[1]','VARCHAR(50)') AS Name,
			t.value('Gender[1]','VARCHAR(1)') AS Gender,
			t.value('Qualification[1]','VARCHAR(50)') AS Qualification,
			t.value('Age[1]','INT') AS Age,
			t.value('CityID[1]','INT') AS CityID,
			t.value('StateID[1]','INT') AS StateID,
			t.value('EmailID[1]','VARCHAR(100)') AS EmailID,
			t.value('Pass[1]','VARCHAR(50)') AS Pass,
			t.value('ImageName[1]','VARCHAR(100)') AS ImageName,
			t.value('ImagePath[1]','VARCHAR(100)') AS ImagePath,
			t.value('ActionType[1]','VARCHAR(25)') AS ActionType
		FROM @XML.nodes('RegisterModel') AS X(t)
	)X

	SELECT @ActionType=ActionType FROM @Temp
		
	IF(@ActionType='Add')
	BEGIN
		INSERT INTO Tbl_Registration (Name,Gender,Qualification,Age,CityID,StateID,EmailID,Pass,ImageName,ImagePath,EntryDate)
		SELECT Name,Gender,Qualification,Age,CityID,StateID,EmailID,Pass,ImageName,ImagePath,GETDATE()
		FROM @Temp

		SELECT  @Message='Added Successfully'
	END
	ELSE IF(@ActionType='Edit')
	BEGIN
		Update R
		SET Name=T.Name,
			Gender=T.Gender,
			Qualification=T.Qualification,
			Age=T.Age,
			CityID=T.CityID,
			StateID=T.StateID,
			EmailID=T.EmailID,
			Pass=T.Pass,
			ImageName=T.ImageName,
			ImagePath=T.ImagePath,
			UpdateDate=GETDATE()
		FROM Tbl_Registration R
		INNER JOIN @Temp T ON R.ID=T.ID

		SELECT  @Message='Updated Successfully'
	END
	
END