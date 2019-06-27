DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0002 - Associating Users to Parcels'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN

    insert into dbo.Parcel(ParcelNumber, ParcelGeometry, OwnerName, OwnerAddress, OwnerCity, OwnerZipCode, ParcelAreaInSquareFeet, ParcelAreaInAcres)
    select apn_label as ParcelNumber, ogr_geometry as ParcelGeometry, owner_name as OwnerName, own_add_1 as OwnerAddress, own_add_2 as OwnerCity, own_zip_co as OwnerZipCode, shape_sqft as ParcelAreaInSquareFeet, shape_acre as ParcelAreaInAcres
    from dbo.rrbma_parcels
    order by apn

    drop table [dbo].[rrbma_parcels]

    declare @dateToUse datetime
    set @dateToUse = '6/26/2019'

    /*
    'Brent Hankins', 'Hankins Farms', 'brent@hankinsfarmsinc.com', 104‐240‐29, 104‐240‐46
    'George Capello', 'Grimmway Farms/ Diamond Farming', 'gcappello@grimmway.com', 524‐020‐15, 524‐020‐16, 524‐020‐20104‐100‐05, 104‐100‐12, 104‐080‐23
    'Garrett Busch', 'Wonderful', 'garrett.busch@wonderful.com', 160‐130‐36, 104‐220‐04, 407‐111‐01, 401‐111‐40, 407‐112‐23, 408‐121‐06, 408‐121‐07
    'Tim Gobler', 'Wonderful', 'timothy.gobler@wonderful.com', 160‐130‐36, 104‐220‐04, 407‐111‐01, 401‐111‐40, 407‐112‐23, 408‐121‐06, 408‐121‐07
    'Jason Selvidge', 'BLACCO', 'jselvidge@aol.com', 104‐300‐04
    'Todd Tracy', 'Tracy Ranch Inc', 'todd@buttonwillow.com', 103‐160‐04, 103‐160‐14, 103‐160‐15, 103‐170‐33, 104‐200‐02, 104‐260‐01, 104‐260‐14
    'Brad DeBranch', 'Bolthouse', 'bdebranch@bolthouseproperties.com', 104‐012‐03, 104‐012‐06, 104‐012‐19,104‐012‐20, 104‐240‐22, 104‐240‐30,104‐240‐31
    'Marie Millan', 'Affentranger Farms', 'rmillan@bak.rr.com', 463‐030‐12, 463‐030‐13, 463‐030‐15,103‐010‐18, 103‐010‐19, 103‐010‐20,103‐010‐21, 103‐010‐25, 103‐010‐28,103‐010‐30, 103‐010‐31, 103‐010‐32,103‐010‐39, 103‐010‐40, 103‐060‐05,103‐060‐08, 103‐060‐09, 103‐060‐10,103‐060‐11, 103‐060‐12, 103‐070‐08,104‐050‐01, 104‐050‐08, 463‐052‐05,463‐052‐06
    'Mike Hopkins', 'Hopkins Ag Inc', 'hopkinsmd67@yahoo.com', 104‐030‐01
    'Bill McCaslin', 'BJ Farms', 104‐210‐01, 104‐210‐02, 104‐210‐08,104‐210‐13, 104‐210‐17, 104‐210‐24,104‐210‐30, 104‐210‐32, 104‐210‐36,104‐210‐37, 104‐210‐38, 104‐250‐02,104‐250‐03, 463‐140‐21, 463‐140‐23
    'Heith Baughman', 'HBhbharv@aol.com', 103‐010‐36, 104‐030‐05, 104‐060‐05,104‐060‐26, 104‐060‐27, 104‐240‐32,104‐240‐33
    'Barry Watts', 'bwwatts@msn.com', 104‐011‐43, 104‐011‐45, 104‐011‐46,104‐071‐02, 104‐071‐22
    'Greg Wegis', 'Tricor Energy', 'greg@wegisandyoung.com', 160‐120‐06, 160‐130‐20, 524‐080‐03,524‐080‐04, 524‐080‐09, 524‐130‐21,524‐130‐22, 524‐130‐23, 524‐130‐24
    'Mark Romanini', 'markromanini@gmail.com', 104‐030‐003, 104‐060‐59
    'Royce Fast and Son', 'roycefast@sbcglobal.net', 104‐300‐01, 104‐300‐03, 104‐011‐08,407‐011‐10, 407‐112‐05, 407‐120‐06,408‐122‐09, 463‐160‐01, 463‐160‐18
    'Mark Johnson', 'Johnson Farms', 'johnsonfarms4@sbcglobal.net', 104‐292‐10, 104‐292‐11
    'Keith Gardiner (Preston Brittian), Pacific Ag Resources, pres@prh20.com, 463‐060‐23, 463‐070‐06, 463‐070‐07, 463-070-11
    */

    set identity_insert dbo.[User] on

    insert into dbo.[User](UserID, FirstName, LastName, Company, Email, RoleID, CreateDate, LastActivityDate, IsActive, ReceiveSupportEmails)
    values
    (5, 'Brent', 'Hankins', 'Hankins Farms', 'brent@hankinsfarmsinc.com', 2, @dateToUse, @dateToUse, 1, 0),
    (6, 'George', 'Capello', 'Grimmway Farms/ Diamond Farming', 'gcappello@grimmway.com', 2, @dateToUse, @dateToUse, 1, 0),
    (7, 'Garrett', 'Busch', 'Wonderful', 'garrett.busch@wonderful.com', 2, @dateToUse, @dateToUse, 1, 0),
    (8, 'Tim', 'Gobler', 'Wonderful', 'timothy.gobler@wonderful.com', 2, @dateToUse, @dateToUse, 1, 0),
    (9, 'Jason', 'Selvidge', 'BLACCO', 'jselvidge@aol.com', 2, @dateToUse, @dateToUse, 1, 0),
    (10, 'Todd', 'Tracy', 'Tracy Ranch Inc', 'todd@buttonwillow.com', 2, @dateToUse, @dateToUse, 1, 0),
    (11, 'Brad', 'DeBranch', 'Bolthouse', 'bdebranch@bolthouseproperties.com', 2, @dateToUse, @dateToUse, 1, 0),
    (12, 'Marie', 'Millan', 'Affentranger Farms', 'rmillan@bak.rr.com', 2, @dateToUse, @dateToUse, 1, 0),
    (13, 'Mike', 'Hopkins', 'Hopkins Ag Inc', 'hopkinsmd67@yahoo.com', 2, @dateToUse, @dateToUse, 1, 0),
    (14, 'Bill', 'McCaslin', 'BJ Farms', 'bill.mccaslin@example.com', 2, @dateToUse, @dateToUse, 1, 0),
    (15, 'Heith', 'Baughman', null, 'HBhbharv@aol.com', 2, @dateToUse, @dateToUse, 1, 0),
    (16, 'Barry', 'Watts', null, 'bwwatts@msn.com', 2, @dateToUse, @dateToUse, 1, 0),
    (17, 'Greg', 'Wegis', 'Tricor Energy', 'greg@wegisandyoung.com', 2, @dateToUse, @dateToUse, 1, 0),
    (18, 'Mark', 'Romanini', null, 'markromanini@gmail.com', 2, @dateToUse, @dateToUse, 1, 0),
    (19, 'Royce', 'Fast', null, 'roycefast@sbcglobal.net', 2, @dateToUse, @dateToUse, 1, 0),
    (20, 'Mark', 'Johnson', 'Johnson Farms', 'johnsonfarms4@sbcglobal.net', 2, @dateToUse, @dateToUse, 1, 0),
    (21, 'Keith', 'Gardiner', 'Pacific Ag Resources', 'pres@prh20.com', 2, @dateToUse, @dateToUse, 1, 0)

    set identity_insert dbo.[User] off

    create table #UserParcel
    (
        UserID int not null,
        ParcelNumber varchar(100) not null
    )

    insert into #UserParcel(UserID, ParcelNumber)
    values
    (5, '104‐240‐29'),
    (5, '104‐240‐46'),
    (6, '524‐020‐15'),
    (6, '524‐020‐16'),
    (6, '524‐020‐20'),
    (6, '104‐100‐05'),
    (6, '104‐100‐12'),
    (6, '104‐080‐23'),
    (7, '160‐130‐36'),
    (7, '104‐220‐04'),
    (7, '407‐111‐01'),
    (7, '401‐111‐40'),
    (7, '407‐112‐23'),
    (7, '408‐121‐06'),
    (7, '408‐121‐07'),
    (8, '160‐130‐36'),
    (8, '104‐220‐04'),
    (8, '407‐111‐01'),
    (8, '401‐111‐40'),
    (8, '407‐112‐23'),
    (8, '408‐121‐06'),
    (8, '408‐121‐07'),
    (9, '104‐300‐04'),
    (10, '103‐160‐04'),
    (10, '103‐160‐14'),
    (10, '103‐160‐15'),
    (10, '103‐170‐33'),
    (10, '104‐200‐02'),
    (10, '104‐260‐01'),
    (10, '104‐260‐14'),
    (11, '104‐012‐03'),
    (11, '104‐012‐06'),
    (11, '104‐012‐19'),
    (11, '104‐012‐20'),
    (11, '104‐240‐22'),
    (11, '104‐240‐30'),
    (11, '104‐240‐31'),
    (12, '463‐030‐12'),
    (12, '463‐030‐13'),
    (12, '463‐030‐15'),
    (12, '103‐010‐18'),
    (12, '103‐010‐19'),
    (12, '103‐010‐20'),
    (12, '103‐010‐21'),
    (12, '103‐010‐25'),
    (12, '103‐010‐28'),
    (12, '103‐010‐30'),
    (12, '103‐010‐31'),
    (12, '103‐010‐32'),
    (12, '103‐010‐39'),
    (12, '103‐010‐40'),
    (12, '103‐060‐05'),
    (12, '103‐060‐08'),
    (12, '103‐060‐09'),
    (12, '103‐060‐10'),
    (12, '103‐060‐11'),
    (12, '103‐060‐12'),
    (12, '103‐070‐08'),
    (12, '104‐050‐01'),
    (12, '104‐050‐08'),
    (12, '463‐052‐05'),
    (12, '463‐052‐06'),
    (13, '104‐030‐01'),
    (14, '104‐210‐01'),
    (14, '104‐210‐02'),
    (14, '104‐210‐08'),
    (14, '104‐210‐13'),
    (14, '104‐210‐17'),
    (14, '104‐210‐24'),
    (14, '104‐210‐30'),
    (14, '104‐210‐32'),
    (14, '104‐210‐36'),
    (14, '104‐210‐37'),
    (14, '104‐210‐38'),
    (14, '104‐250‐02'),
    (14, '104‐250‐03'),
    (14, '463‐140‐21'),
    (14, '463‐140‐23'),
    (15, '103‐010‐36'),
    (15, '104‐030‐05'),
    (15, '104‐060‐05'),
    (15, '104‐060‐26'),
    (15, '104‐060‐27'),
    (15, '104‐240‐32'),
    (15, '104‐240‐33'),
    (16, '104‐011‐43'),
    (16, '104‐011‐45'),
    (16, '104‐011‐46'),
    (16, '104‐071‐02'),
    (16, '104‐071‐22'),
    (17, '160‐120‐06'),
    (17, '160‐130‐20'),
    (17, '524‐080‐03'),
    (17, '524‐080‐04'),
    (17, '524‐080‐09'),
    (17, '524‐130‐21'),
    (17, '524‐130‐22'),
    (17, '524‐130‐23'),
    (17, '524‐130‐24'),
    (18, '104‐030‐03'),
    (18, '104‐060‐59'),
    (19, '104‐300‐01'),
    (19, '104‐300‐03'),
    (19, '104‐011‐08'),
    (19, '407‐011‐10'),
    (19, '407‐112‐05'),
    (19, '407‐120‐06'),
    (19, '408‐122‐09'),
    (19, '463‐160‐01'),
    (19, '463‐160‐18'),
    (20, '104‐292‐10'),
    (20, '104‐292‐11'),
    (21, '463‐060‐23'),
    (21, '463‐070‐06'),
    (21, '463‐070‐07'),
    (21, '463-070-11')

    insert into dbo.UserParcel(UserID, ParcelID)
    select up.UserID, p.ParcelID
    from #UserParcel up
    join dbo.Parcel p on up.ParcelNumber = p.ParcelNumber

    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Ray Lee', @MigrationName, 'Associating Users to Parcels.'
END