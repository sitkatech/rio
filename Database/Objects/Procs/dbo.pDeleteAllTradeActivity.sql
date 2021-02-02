IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.pDeleteAllTradeActivity'))
    drop procedure dbo.pDeleteAllTradeActivity
go

create procedure dbo.pDeleteAllTradeActivity

as

begin

alter table dbo.WaterTransferRegistrationParcel
drop constraint [FK_WaterTransferRegistrationParcel_WaterTransferRegistration_WaterTransferRegistrationID]

alter table dbo.WaterTransferRegistration
drop constraint [FK_WaterTransferRegistration_WaterTransfer_WaterTransferID]

alter table dbo.WaterTransfer
drop constraint [FK_WaterTransfer_Offer_OfferID]

alter table dbo.Offer
drop constraint [FK_Offer_Trade_TradeID]

alter table dbo.Trade
drop constraint [FK_Trade_Posting_PostingID]

truncate table dbo.WaterTransferRegistrationParcel
truncate table dbo.WaterTransferRegistration
truncate table dbo.WaterTransfer
truncate table dbo.Offer
truncate table dbo.Trade
truncate table dbo.Posting

alter table dbo.Trade
add constraint [FK_Trade_Posting_PostingID] FOREIGN KEY ([PostingID]) REFERENCES [dbo].[Posting] ([PostingID])

alter table dbo.Offer
add constraint [FK_Offer_Trade_TradeID] FOREIGN KEY ([TradeID]) REFERENCES [dbo].[Trade] ([TradeID])

alter table dbo.WaterTransfer
add constraint [FK_WaterTransfer_Offer_OfferID] FOREIGN KEY ([OfferID]) REFERENCES [dbo].[Offer] ([OfferID])

alter table dbo.WaterTransferRegistration
add constraint [FK_WaterTransferRegistration_WaterTransfer_WaterTransferID] FOREIGN KEY ([WaterTransferID]) REFERENCES [dbo].[WaterTransfer] ([WaterTransferID])

alter table dbo.WaterTransferRegistrationParcel
add constraint [FK_WaterTransferRegistrationParcel_WaterTransferRegistration_WaterTransferRegistrationID] FOREIGN KEY([WaterTransferRegistrationID])
REFERENCES [dbo].[WaterTransferRegistration] ([WaterTransferRegistrationID])

end