ALTER TABLE [dbo].[AccountParcel] 
DROP CONSTRAINT [CK_ParcelOwner_OwnerNameXorOwnerID]
GO

ALTER TABLE [dbo].[AccountParcel]  
WITH CHECK ADD CONSTRAINT [CK_ParcelOwner_OwnerNameXorOwnerIDXorParcelStatusID] CHECK  (([ParcelStatusID] = 2 or ([AccountID] IS NULL AND [OwnerName] IS NOT NULL) OR ([AccountID] IS NOT NULL AND [OwnerName] IS NULL)))

ALTER TABLE [dbo].[Account]
WITH CHECK ADD CONSTRAINT [CK_InactiveDate_ParcelStatusIDXorInactivateDate] CHECK ((([AccountStatusID] = 2 and [InactivateDate] IS NOT NULL) or ([AccountStatusID] <> 2 and [InactivateDate] IS NULL))) 