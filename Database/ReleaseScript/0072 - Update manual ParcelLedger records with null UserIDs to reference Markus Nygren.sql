update dbo.ParcelLedger
set UserID = 37
where ParcelLedgerEntrySourceTypeID = 1 and UserID is null
