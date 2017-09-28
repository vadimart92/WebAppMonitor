
--EXEC ImportDailyData @fileName = '\\tscore-dev-13\WorkAnalisys\xevents\Export_2017-06-20\2017-06-19\ts_sqlprofiler_05_sec*.xel'
--EXEC SaveDailyData

IF OBJECT_ID('ImportDailyData', 'P') IS NULL
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = 'CREATE PROCEDURE ImportDailyData AS BEGIN SET NOCOUNT ON END'
	EXEC sys.sp_executesql @SQL
END
GO
ALTER PROCEDURE ImportDailyData @fileName NVARCHAR(MAX)
AS
BEGIN

ALTER TABLE TodayQueryText NOCHECK CONSTRAINT ALL
ALTER TABLE TodayNormQueryText NOCHECK CONSTRAINT ALL
ALTER TABLE TodayQuery NOCHECK CONSTRAINT ALL
DELETE TodayQueryText
DELETE TodayNormQueryText
DELETE TodayQuery
ALTER TABLE TodayQueryText CHECK CONSTRAINT ALL
ALTER TABLE TodayNormQueryText CHECK CONSTRAINT ALL
ALTER TABLE TodayQuery CHECK CONSTRAINT ALL;

TRUNCATE TABLE ts_sqlprofiler;
TRUNCATE TABLE ts_sqlprofiler_data;

PRINT 'Import xml'

INSERT INTO ts_sqlprofiler_data
SELECT CAST(event_data AS XML) XML
FROM sys.fn_xe_file_target_read_file(@fileName, NULL, NULL, NULL)

PRINT 'Parse xml'
INSERT INTO ts_sqlprofiler
SELECT 
	ID = NEWID()
	,[db_name] = XML.value('(event/action[@name="database_name"])[1]', 'NVARCHAR(MAX)')
   ,[username] = XML.value('(event/action[@name="username"])[1]', 'NVARCHAR(MAX)')
   ,[session_id] = XML.value('(event/action[@name="session_id"])[1]', 'NVARCHAR(MAX)')
   ,[client_hostname] = XML.value('(event/action[@name="client_hostname"])[1]', 'NVARCHAR(MAX)')
   ,[duration_sec] = XML.value('(event/data[@name="duration"])[1]', 'BIGINT') / 1000000.0
   ,[cpu_time_sec] = XML.value('(event/data[@name="cpu_time"])[1]', 'BIGINT') / 1000000.0
   ,[logical_reads] = XML.value('(event/data[@name="logical_reads"])[1]', 'BIGINT')
   ,[writes] = XML.value('(event/data[@name="writes"])[1]', 'BIGINT')
   ,[row_count] = XML.value('(event/data[@name="row_count"])[1]', 'BIGINT')
   ,[end_time_utc] = XML.value('(event/@timestamp)[1]', 'DATETIME')
   ,[statement] = XML.value('(event/data[@name="statement"])[1]', 'VARCHAR(MAX)')
FROM ts_sqlprofiler_data t;

PRINT 'Import data';

INSERT INTO TodayQuery (Id, username, session_id, client_hostname, duration_sec, cpu_time_sec, logical_reads, writes, row_count, end_time_utc)
SELECT ts.ID
	  ,ts.username
	  ,ts.session_id
	  ,ts.client_hostname
	  ,ts.duration_sec
	  ,ts.cpu_time_sec
	  ,ts.logical_reads
	  ,ts.writes
	  ,ts.row_count
	  ,ts.end_time_utc
FROM ts_sqlprofiler ts
	WHERE ts.statement NOT LIKE '%master.dbo.xp_instance_regread%'
	AND ts.end_time_utc > (SELECT MAX(end_time_utc) FROM QueryHistory);

DECLARE @todayRecords INT = (SELECT COUNT(*) FROM TodayQuery);
PRINT 'Today records: ' + CAST(@todayRecords AS VARCHAR(MAX))

PRINT 'Import sql'

INSERT INTO TodayQueryText (TodayQueryId, QueryText)
SELECT ID, statement
FROM ts_sqlprofiler
WHERE ID IN (SELECT Id FROM TodayQuery);

PRINT 'Parse sql';

TRUNCATE TABLE TempNormQueryText;

INSERT INTO TempNormQueryText(TodayQueryId, NormalizedQuery, NormalizedQueryHash)
SELECT TodayQueryId
	  , dbo.NormalizeQueryText(QueryText) AS NormalizedQuery
	  ,CAST(null AS VARBINARY(64)) NormalizedQueryHash
FROM TodayQueryText;

UPDATE TempNormQueryText
SET NormalizedQueryHash = HASHBYTES('SHA2_512', NormalizedQuery);

;WITH DublicateReport AS (
	SELECT tnqt.TodayQueryId
		  ,tnqt.NormalizedQuery
		  ,tnqt.NormalizedQueryHash
		  , ROW_NUMBER() OVER (PARTITION BY tnqt.NormalizedQueryHash ORDER BY tnqt.NormalizedQueryHash) DublicateNumber
	FROM TempNormQueryText tnqt	
)
INSERT INTO TodayNormQueryText (NormalizedQuery, QueryHash)
SELECT r.NormalizedQuery, r.NormalizedQueryHash
FROM DublicateReport r
	WHERE r.DublicateNumber = 1

UPDATE TodayQueryText
SET TodayNormQueryTextId = n.Id
FROM TodayNormQueryText n
INNER JOIN TempNormQueryText t ON n.QueryHash = t.NormalizedQueryHash
WHERE t.TodayQueryId = TodayQueryText.TodayQueryId

--if EXISTS( SELECT * FROM TodayQueryText WHERE TodayNormQueryTextId IS NULL ) PRINT 'TodayQueryText contains bad TodayNormQueryTextId';

TRUNCATE TABLE ts_sqlprofiler;
TRUNCATE TABLE ts_sqlprofiler_data;


END