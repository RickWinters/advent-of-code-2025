declare @start int = 50;

if not exists (select * from sysobjects where name='TempTable' and xtype='U')
begin 
    CREATE TABLE TempTable (Value NVARCHAR(4000));
end
else 
begin
    TRUNCATE table TempTable 
end 

/* 
  Lokaal een docker container gebruikt en niet de juiste bestands rechten ingesteld.
  Daarom de input direct in het script gezet 
  */
--  BULK INSERT dbo.TempTable
--     FROM '/user/tmp/input.csv'
--     WITH
--     (
--         ROWTERMINATOR ='\n'
--     ) 

declare @input NVARCHAR(max) = N'L68
L30
R48
L5
R60
L55
L1
L99
R14
L82';

insert into TempTable
select Value
from STRING_SPLIT(@input + char(13), char(10))


select LEFT(value, 1) as Direction,
    substring(value, 2, LEN(value)-2) as Amount,
    0 as CalculationData,
    0 as CalculationResult,
    ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS rownum
into #TempData
from TempTable


update #TempData
set CalculationData = CASE when direction = 'R' then  Amount else 0- Amount end

UPDATE #TempData
SET CalculationResult = calc.[data]
FROM #TEMPDATA
    JOIN (
    SELECT CurrentRow.rownum, (1000000000 + 50 + (SUM(PreviousRows.CalculationData % 100))) % 100 as data
    FROM #TempData as CurrentRow
        join #TempData as PreviousRows on PreviousRows.rownum <= CurrentRow.rownum
    GROUP BY CurrentRow.rownum
) AS Calc
    on #tempdata.rownum = calc.rownum

SELECT case when CalculationResult = 0 then 1 else 0 end as Answer1,
    case 
         when (ISNULL(LAG(CalculationResult) over(order by rownum) , 50) <> 0)
        AND ( ( ISNULL( LAG(CalculationResult) over(order by rownum), 50)) + (CalculationData%100)  > =100
        OR ( ISNULL( LAG(CalculationResult) over(order by rownum), 50) ) + (CalculationData%100)  <=0) 
             then 1
         else 0 
         end as Answer2_Clicks,
    ABS((CalculationData - (CalculationData % 100)) / 100) as Answer2_ExtraRotation
INTO #CalcData
FROM #TempData


select sum(Answer1) as Answer1, sum(Answer2_Clicks) + sum(Answer2_ExtraRotation) as Answer2
from #CalcData

drop table #TempData
drop table #CalcData
