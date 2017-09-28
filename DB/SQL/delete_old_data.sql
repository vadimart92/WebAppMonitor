
declare @datesToDelete table (id int)
insert into @datesToDelete 
select id
from Dates
where id < 80

delete deadlocksinfo where DateId IN (select id from @datesToDelete)
delete executorlog where DateId IN (select id from @datesToDelete)
delete longlocksinfo where DateId IN (select id from @datesToDelete)
delete performanceloginfo where DateId IN (select id from @datesToDelete)
delete QueryTextHistory where QueryHistoryId IN  (select id from queryhistory where DateId IN (select id from @datesToDelete))
delete queryhistory where DateId IN (select id from @datesToDelete)
delete readerlog where DateId IN (select id from @datesToDelete)

delete Dates
where Id in (select id from @datesToDelete)