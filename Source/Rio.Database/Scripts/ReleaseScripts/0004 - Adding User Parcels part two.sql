DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0004 - Adding User Parcels part two'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN

    create table #users
    (
        UserID int not null,
        FirstName varchar(100) not null,
        LastName varchar(100) not null,
        Email varchar(100) not null,
        Company varchar(100) null,
        ObfuscatedFirstName varchar(100) not null,
        ObfuscatedLastName varchar(100) not null,
        ObfuscatedEmail varchar(100) not null
    )

    insert into #users(UserID, FirstName, LastName, Email, Company, ObfuscatedFirstName, ObfuscatedLastName, ObfuscatedEmail)
    values
    (5, 'Brent', 'Hankins', 'brent@hankinsfarmsinc.com', 'Hankins Farms', 'Landowner', 'A', 'LandownerA@somecompany.com'),
    (6, 'George', 'Capello', 'gcappello@grimmway.com', 'Grimmway Farms/ Diamond Farming', 'Landowner', 'B', 'LandownerB@somecompany.com'),
    (7, 'Garrett', 'Busch', 'garrett.busch@wonderful.com', 'Wonderful', 'Landowner', 'C', 'LandownerC@somecompany.com'),
    (8, 'Tim', 'Gobler', 'timothy.gobler@wonderful.com', 'Wonderful', 'Landowner', 'D', 'LandownerD@somecompany.com'),
    (9, 'Jason', 'Selvidge', 'jselvidge@aol.com', 'BLACCO', 'Landowner', 'E', 'LandownerE@somecompany.com'),
    (10, 'Todd', 'Tracy', 'todd@buttonwillow.com', 'Tracy Ranch Inc', 'Landowner', 'F', 'LandownerF@somecompany.com'),
    (11, 'Brad', 'DeBranch', 'bdebranch@bolthouseproperties.com', 'Bolthouse', 'Landowner', 'G', 'LandownerG@somecompany.com'),
    (12, 'Marie', 'Millan', 'rmillan@bak.rr.com', 'Affentranger Farms', 'Landowner', 'H', 'LandownerH@somecompany.com'),
    (13, 'Mike', 'Hopkins', 'hopkinsmd67@yahoo.com', 'Hopkins Ag Inc', 'Landowner', 'I', 'LandownerI@somecompany.com'),
    (14, 'Bill', 'McCaslin', 'bill.mccaslin@example.com', 'BJ Farms', 'Landowner', 'J', 'LandownerJ@somecompany.com'),
    (15, 'Heith', 'Baughman', 'HBhbharv@aol.com ', NULL, 'Landowner', 'K', 'LandownerK@somecompany.com'),
    (16, 'Barry', 'Watts', 'bwwatts@msn.com ', NULL, 'Landowner', 'L', 'LandownerL@somecompany.com'),
    (17, 'Greg', 'Wegis', 'greg@wegisandyoung.com', 'Tricor Energy', 'Landowner', 'M', 'LandownerM@somecompany.com'),
    (18, 'Mark', 'Romanini', 'markromanini@gmail.com ', NULL, 'Landowner', 'N', 'LandownerN@somecompany.com'),
    (19, 'Royce', 'Fast', 'roycefast@sbcglobal.net ', NULL, 'Landowner', 'O', 'LandownerO@somecompany.com'),
    (20, 'Mark', 'Johnson', 'johnsonfarms4@sbcglobal.net', 'Johnson Farms', 'Landowner', 'P', 'LandownerP@somecompany.com'),
    (21, 'Keith', 'Gardiner', 'pres@prh20.com', 'Pacific Ag Resources', 'Landowner', 'Q', 'LandownerQ@somecompany.com')


    update u
    set u.FirstName = us.FirstName,
	    u.LastName = us.LastName,
	    u.Email = us.Email,
	    u.Company = us.Company,
	    u.IsActive = 1
    from dbo.[User] u
    join #users us on u.UserID = us.UserID

    select *
    from dbo.[User]


    -- john burns real:
    insert into dbo.UserParcel(UserID, ParcelID)
    select 3 as UserID, p.ParcelID
    from dbo.Parcel p
    where p.OwnerName = 'BELLUOMINI RANCHES LP'

    -- john burns personal
    insert into dbo.UserParcel(UserID, ParcelID)
    select 2 as UserID, p.ParcelID
    from dbo.Parcel p
    where p.OwnerName = 'J G BOSWELL TOMATO CO KERN LLC'

    -- andrew
    insert into dbo.UserParcel(UserID, ParcelID)
    select 26 as UserID, p.ParcelID
    from dbo.Parcel p
    where p.OwnerName = 'GOOSELAKE HOLDING CO'

    --ray
    insert into dbo.UserParcel(UserID, ParcelID)
    select 1 as UserID, p.ParcelID
    from dbo.Parcel p
    where p.OwnerName in ('CAUZZA RANCHES L L C', 'CAUZZA RANCHES LLC')

    -- jolly
    insert into dbo.UserParcel(UserID, ParcelID)
    select 4 as UserID, p.ParcelID
    from dbo.Parcel p
    where p.OwnerName = 'ECHEVERRIA JUAN & DOLORES FAMILY TRUST'

    -- kathleen
    insert into dbo.UserParcel(UserID, ParcelID)
    select 23 as UserID, p.ParcelID
    from dbo.Parcel p
    where p.OwnerName like 'KOSAREFF EDWARD%'

    -- harry
    insert into dbo.UserParcel(UserID, ParcelID)
    select 24 as UserID, p.ParcelID
    from dbo.Parcel p
    where p.OwnerName = 'GLOBAL AG PROP USA LLC'

    -- bill
    insert into dbo.UserParcel(UserID, ParcelID)
    select 25 as UserID, p.ParcelID
    from dbo.Parcel p
    where p.OwnerName = 'GOYENETCHE FAMILY TRUST'

    --christina
    insert into dbo.UserParcel(UserID, ParcelID)
    select 27 as UserID, p.ParcelID
    from dbo.Parcel p
    where p.OwnerName = 'HB AG INVS LLC'

    -- clay
    insert into dbo.UserParcel(UserID, ParcelID)
    select 28 as UserID, p.ParcelID
    from dbo.Parcel p
    where p.OwnerName = 'MARVIN WEIDENBACH FARMS'

    -- set the parcel annual allocation
    insert into dbo.ParcelAllocation(ParcelID, WaterYear, AcreFeetAllocated)
    select p.ParcelID, 2016 as WaterYear, p.ParcelAreaInAcres * 3 as AcreFeetAllocated
    from dbo.UserParcel up
    join dbo.Parcel p on up.ParcelID = p.ParcelID
    union
    select p.ParcelID, 2017 as WaterYear, p.ParcelAreaInAcres * 3 as AcreFeetAllocated
    from dbo.UserParcel up
    join dbo.Parcel p on up.ParcelID = p.ParcelID
    union
    select p.ParcelID, 2018 as WaterYear, p.ParcelAreaInAcres * 3 as AcreFeetAllocated
    from dbo.UserParcel up
    join dbo.Parcel p on up.ParcelID = p.ParcelID



    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Ray Lee', @MigrationName, 'Adding parcels to admin users and unobfuscating land owners.'

END