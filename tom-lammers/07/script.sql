DROP TABLE IF EXISTS #Temp
set NOCOUNT on

declare @input NVARCHAR(max) = N'.......S.......
...............
.......^.......
...............
......^.^......
...............
.....^.^.^.....
...............
....^.^...^....
...............
...^.^...^.^...
...............
..^...^.....^..
...............
.^.^.^.^.^...^.
...............'


select Value,
  ROW_NUMBER() over (order by (select null)) as RowNumber
into #Temp
FROM string_split(replace(@input, char(10), ''), char(13))

DECLARE @rows int = (
                        select COUNT(*)
FROM #Temp
                    );
DECLARE @rowLength int = (
                             select MAX(LEN(value))
FROM #Temp
                         );
declare @index int = 0;
declare @Count int = 0;
DECLARE @rowNumber int = 0;

declare @results table
(
  I int,
  value bigint
);
declare @previousResults table
(
  I int,
  value bigint
);


declare @parentRow NVARCHAR(4000) = (
                                        select top 1
  value
from #Temp
order by RowNumber
                                    )
delete top (1)
from #Temp



while @index < @rowLength
begin

  insert into @results
  values
    (@index, 0)
  set @index = @index + 1
end
set @index = 0;

update @results
set value = value + 1
where I = CHARINDEX('S', @parentRow)

declare @CurrentRow NVARCHAR(4000);
while exists (select *
from #temp)
BEGIN
  SET @CurrentRow =
    (
        select top 1
    value
  from #Temp
  order by RowNumber
    )

  delete @previousResults
  insert into @previousResults
  select *
  from @results

  while @index < @rowLength
    begin

    if (SUBSTRING(@parentRow, @index, 1)) = '|'
      AND (SUBSTRING(@CurrentRow, @index, 1)) = '^'
        begin
      set @CurrentRow = SUBSTRING(@CurrentRow, 0, @index) + 'X' + SUBSTRING(@CurrentRow, @index + 1, 99999)
      set @CurrentRow
                = SUBSTRING(@CurrentRow, 0, @index - 1) + '|' + SUBSTRING(@CurrentRow, @index, 1) + '|'
                  + SUBSTRING(@CurrentRow, @index + 2, 99999)

      set @Count = @Count + 1

      if (SUBSTRING(@CurrentRow, @index, 1)) = 'X'
            begin

        update @results
                set [value] = VALUE +
                              (
                                  select value
        from @previousResults
        where i = @index
                              )
                where I in ( @index - 1, @index + 1 )


        update @results
                set [value] = 0
                where I in ( @index )
      end
    end

    if (SUBSTRING(@parentRow, @index, 1)) in ( 'S', '|' )
      AND (SUBSTRING(@CurrentRow, @index, 1)) = '.'
        begin
      set @CurrentRow = SUBSTRING(@CurrentRow, 0, @index) + '|' + SUBSTRING(@CurrentRow, @index + 1, 99999)

    end
    set @index = @index + 1


  end


  print @CurrentRow

  set @index = 0

  SET @parentRow = @CurrentRow
  delete top (1)
    from #Temp

  SET @CurrentRow =
    (
        select top 1
    value
  from #Temp
  order by RowNumber
    )

end

select @Count as Answer1, sum(value) + 1 as Answer2
from @results
where i >= 0
  and i < @rowLength
