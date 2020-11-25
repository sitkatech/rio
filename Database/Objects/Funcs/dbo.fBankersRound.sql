--This exists to be consistent with C# rounding
--See here https://stackoverflow.com/questions/41592666/tsql-rounding-vs-c-sharp-rounding
IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.fBankersRound'))
    drop procedure dbo.fBankersRound
go

--Function found here http://blogs.lessthandot.com/index.php/DataMgmt/DataDesign/sql-server-rounding-methods
Create Function dbo.fBankersRound(@Val Decimal(32,16), @Digits Int)
Returns Decimal(32,16)
AS
Begin
    Return Case When Abs(@Val - Round(@Val, @Digits, 1)) * Power(10, @Digits+1) = 5 
                Then Round(@Val, @Digits, Case When Convert(int, Round(abs(@Val) * power(10,@Digits), 0, 1)) % 2 = 1 Then 0 Else 1 End)
                Else Round(@Val, @Digits)
                End
End