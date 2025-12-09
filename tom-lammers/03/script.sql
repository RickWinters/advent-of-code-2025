DECLARE @SequenceLength INT = 12;

DECLARE @input NVARCHAR(max) = N'987654321111111
811111111111119
234234234234278
818181911112111';

SELECT Replace(Replace(value, Char(13), ''), Char(10), '') AS VALUE,
       CONVERT(NVARCHAR(400), N'') AS Result,
       CONVERT(BIGINT, 0) AS CurrentIndex
INTO   #temp
FROM   String_split(@input + Char(13), Char(10))

DECLARE @inputLength INT = (SELECT Max(Len(value)) FROM #temp);

WITH cte_split
     AS (SELECT 0 AS ID,
                CONVERT(INT, Substring(value, 0, 1)) AS Number,
                value
         FROM   #temp
         UNION ALL
         SELECT id + 1 AS ID,
                CONVERT(INT, Substring(cte_split.value, id + 1, 1)) AS Number,
                value
         FROM   cte_split
         WHERE  cte_split.id < @inputLength)
SELECT *
INTO   #cte_split
FROM   cte_split

DECLARE @Counter INT = 0;

WHILE ( @Counter < @SequenceLength )
  BEGIN
      SET @Counter = @Counter + 1;

      UPDATE #temp
      SET    result = result
                      + CONVERT(NVARCHAR(4000), FirstCalulation.[number]),
             currentindex = [index]
      FROM   #temp
             JOIN (SELECT Max(number) AS number,
                          Charindex(CONVERT(NVARCHAR(1), Max(number)),
                          #cte_split.[value],
                          currentindex + 1) AS [Index],
                          #cte_split.[value]
                   FROM   #cte_split
                          JOIN #temp
                            ON #cte_split.[value] = #temp.[value]
                   WHERE  id <= ( @inputLength - ( @SequenceLength - ( Len(result) + 1  ) ) )
                          AND id > #temp.currentindex
                   GROUP  BY #cte_split.[value],
                             currentindex) AS FirstCalulation
               ON #temp.[value] = FirstCalulation.[value]
  END

SELECT Sum(CONVERT(BIGINT, result)) AS Result
FROM   #temp

-- select * from #temp
DROP TABLE #temp

DROP TABLE #cte_split
-- In 987654321111111, the largest joltage can be found by turning on everything except some 1s at the end to produce 987654321111.
-- In the digit sequence 811111111111119, the largest joltage can be found by turning on everything except some 1s, producing 811111111119.
-- In 234234234234278, the largest joltage can be found by turning on everything except a 2 battery, a 3 battery, and another 2 battery near the start to produce 434234234278.
-- In 818181911112111, the joltage 888911112111 is produced by turning on everything except some 1s near the front.
