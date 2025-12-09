
DECLARE @input NVARCHAR(max) = N'11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124';

SELECT Value as Sequence,
       Convert(bigint,  SUBSTRING(value, 0, CHARINDEX( '-', [value]))) as FirstValue,
       Convert(bigint,  SUBSTRING(value, CHARINDEX( '-', [value]) + 1, len([value]))) as LastValue
into #temp
FROM String_split(@input ,',');

declare @MagicRecursionNumber int = 32766
-- Waardeloze hack omdat de recursion in SQL niet meer dan 32767 kan zijn in de common table expression :( 
while exists (select *
from #temp
where LastValue - FirstValue > 32766 )
BEGIN
       insert into #temp
       select CONVERT(nvarchar(4000), FirstValue) + '-' + CONVERT(nvarchar(4000), FirstValue + @MagicRecursionNumber),
              FirstValue, FirstValue + @MagicRecursionNumber
       from #temp
       where LastValue - FirstValue > @MagicRecursionNumber

       update #temp
       set [Sequence] = CONVERT(nvarchar(4000), FirstValue + @MagicRecursionNumber + 1) + '-' + CONVERT(nvarchar(4000), LastValue),
      FirstValue = FirstValue + @MagicRecursionNumber + 1
       from #temp where LastValue - FirstValue > @MagicRecursionNumber
end
;

WITH
       cte_split
       AS
       (
              SELECT [Sequence],
                            FirstValue AS Number
                , LastValue
                     FROM #temp
              UNION ALL
                     SELECT Sequence,
                            Number + 1 AS Number,
                            LastValue
                     FROM cte_split
                     WHERE  cte_split.Number < LastValue
       )

SELECT CONVERT( NVARCHAR(1000), number) as Number
INTO    #cte_split
FROM cte_split
OPTION (MAXRECURSION 32767);

-- check of de helft overeen komt 
select SUM(convert(bigint, number))  as answer1
from #cte_split
where LEN(number) % 2 <> 1 and right(Number, LEN(Number) / 2) = LEFT(Number, LEN(Number) / 2)

/*
Maximale grote van een getal is 10 tekens. 
Mogelijke herhalingen:
       1 => geen 
       2 => 1, 2
       3 => 3
       4 => 2, 4
       5 => geen 
       6 => 2, 3, 6 
       7 => geen
       8 => 2, 4, 8
       9 => 3, 9
       10=> 2, 5, 10
*/


select SUM(convert(bigint, number))  as answer2
FROM #cte_split
where 
       -- 1 getal kan niet herhalen
       LEN(number) > 1
       
       and (
              -- Check of alle nummers herhalen
              number = REPLICATE( LEFT(number, 1) , LEN(number))
       
              -- check of de helft herhaald voor de volgende nummers 
              or (
                     LEN(Number) in (4, 6, 8, 10)
                     and LEFT(number, LEN(Number)/ 2) = right(Number, LEN(Number)/ 2)
              )
              -- check of 2 getallen herhalen 
              or
              ( 
                     LEN(Number) in (4, 6, 8, 10)
                     and replace( number, LEFT(number, 2), '--') = REPLICATE('-', LEN(number))
              )
              -- check of de er 3 getallen herhalen
              or ( 
                     LEN(Number) = 9
                     and LEFT(number, 3) = right(Number,3 ) AND SUBSTRING(number, 4, 3) = left(number, 3)
              )
       )

DROP TABLE #temp
DROP TABLE #cte_split

