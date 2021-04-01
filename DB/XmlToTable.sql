insert into CurrencyRates
select 
@Data.value('(/*/@Date)[1]','nvarchar(30)') as Date,
A.Valute.query('NumCode').value('.','nvarchar(30)') as NumCode,
A.Valute.query('CharCode').value('.','nvarchar(30)') as CharCode,
A.Valute.query('Nominal').value('.','int') as Nominal,
A.Valute.query('Name').value('.','nvarchar(50)') as Name,
A.Valute.query('Value').value('.','nvarchar(30)') as Value
from
(
select cast(c as xml) from
openrowset(bulk 'C:\Users\User\source\repos\Financies\Financies\bin\Debug\blah.xml', single_clob) as T(c)
) as S(c)
cross apply c.nodes('ValCurs/Valute') as A(Valute)