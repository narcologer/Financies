create function CurrencyByName(@Date nvarchar(30), @Name nvarchar(50))
Returns nvarchar(30)
Begin
Declare @Value nvarchar(30)
Select @Value = value from CurrencyRates where Date = @Date and Name = @Name
Return @Value
end;