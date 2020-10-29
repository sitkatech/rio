Alter Table dbo.Posting
Add CreateUserID int null
constraint FK_Posting_User_CreateUserID_UserID Foreign Key references dbo.[User](UserID)
