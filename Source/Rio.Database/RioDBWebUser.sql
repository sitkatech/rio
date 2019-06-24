CREATE LOGIN [RioDBWebUser] WITH PASSWORD = 'password#1'
GO

CREATE user [RioDBWebUser] for login [RioDBWebUser]
GO

exec sp_addrolemember 'db_owner', 'RioDBWebUser'
