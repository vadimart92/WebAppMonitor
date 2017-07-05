set QUOTED_IDENTIFIER ON;
IF (OBJECT_ID('QueryStatInfo', 'V') IS NOT NULL) 
	DROP VIEW QueryStatInfo;
IF (OBJECT_ID('QueryStatInfo', 'U') IS NOT NULL) 
	DROP TABLE QueryStatInfo;
DROP VIEW IF EXISTS QueryStatInfo_Old;
DROP VIEW IF EXISTS VwQueryStatInfo;
DROP VIEW  IF EXISTS StatementStats;
DROP VIEW IF EXISTS LockersStats;
DROP VIEW IF EXISTS LockedStats;
DROP VIEW  IF EXISTS VwStatementStats;
DROP VIEW IF EXISTS VwLockersStats;
DROP VIEW IF EXISTS VwLockedStats;
DROP TABLE IF EXISTS Grouped_QueryStatInfo;
DROP VIEW IF EXISTS VwQueryTextByDate;
DROP VIEW IF EXISTS VwReaderLogStats;
GO
SET QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
CREATE VIEW dbo.QueryStatInfo_Old
AS
SELECT
	d.Date [Date]
   ,CAST(ROUND(SUM(qh.duration_sec), 2) AS DECIMAL(18, 2)) TotalDuration
   ,CAST(ROUND(AVG(qh.duration_sec), 2) AS DECIMAL(18, 2)) AvgDuration
   ,CAST(ROUND(COUNT(qth.QueryHistoryId), 2) AS DECIMAL(18, 2)) Count
   ,CAST(ROUND(AVG(qh.row_count), 2) AS DECIMAL(18, 2)) AvgRowCount
   ,CAST(ROUND(AVG(qh.logical_reads), 2) AS DECIMAL(18, 2)) AvgLogicalReads
   ,CAST(ROUND(AVG(qh.cpu_time_sec), 2) AS DECIMAL(18, 2)) AvgCPU
   ,CAST(ROUND(AVG(qh.writes), 2) AS DECIMAL(18, 2)) AvgWrites
   ,CAST(0 AS DECIMAL(18, 2)) AS AvgAdoReads
   ,nqth.NormalizedQuery QueryText
   ,qth.NormQueryTextHistoryId NormalizedQueryTextId
FROM QueryTextHistory qth
INNER JOIN QueryHistory qh 	ON qth.QueryHistoryId = qh.Id
INNER JOIN Dates d 	ON qh.DateId = d.Id
INNER JOIN NormQueryTextHistory nqth ON qth.NormQueryTextHistoryId = nqth.Id
GROUP BY qth.NormQueryTextHistoryId, nqth.NormalizedQuery, d.Date
GO
CREATE VIEW VwStatementStats WITH SCHEMABINDING AS 
SELECT
	qh.DateId
	,qth.NormQueryTextHistoryId NormalizedQueryTextId
	,COUNT_BIG(*) [Count]
	,SUM(ISNULL(qh.duration_sec, 0)) TotalDuration
	,SUM(ISNULL(qh.row_count, 0)) TotalRowCount
   ,SUM(ISNULL(qh.logical_reads, 0)) TotalLogicalReads
   ,SUM(ISNULL(qh.cpu_time_sec, 0)) TotalCPU
   ,SUM(ISNULL(qh.writes,0)) TotalWrites
FROM dbo.QueryTextHistory qth
INNER JOIN dbo.QueryHistory qh ON qth.QueryHistoryId = qh.Id
GROUP BY qh.DateId, qth.NormQueryTextHistoryId
GO

CREATE UNIQUE CLUSTERED INDEX Ix_Cl
	ON VwStatementStats (DateId, NormalizedQueryTextId)

CREATE COLUMNSTORE INDEX Ix_AllColumns ON VwStatementStats (DateId, NormalizedQueryTextId, [Count], TotalDuration, TotalRowCount, TotalLogicalReads, TotalCPU, TotalWrites)
GO
CREATE VIEW VwLockersStats WITH SCHEMABINDING
AS 
SELECT li.DateId
	  ,li.BlockerQueryId NormalizedQueryTextId
	  ,COUNT_BIG(*) [Count]
	  ,SUM(ISNULL(Duration/1000000, 0)) TotalDuration	
FROM dbo.LongLocksInfo li
GROUP BY li.DateId, li.BlockerQueryId
GO
CREATE UNIQUE CLUSTERED INDEX IX_Cl ON VwLockersStats (DateId, NormalizedQueryTextId);
CREATE COLUMNSTORE INDEX Ix_AllColumns ON VwLockersStats (DateId, NormalizedQueryTextId, [Count], TotalDuration);
GO

CREATE VIEW VwLockedStats WITH SCHEMABINDING
AS 
SELECT li.DateId
	  ,li.BlockedQueryId NormalizedQueryTextId
	  ,COUNT_BIG(*) [Count]
	  ,SUM(ISNULL(Duration/1000000, 0)) TotalDuration
FROM dbo.LongLocksInfo li
GROUP BY li.DateId, li.BlockedQueryId
GO
CREATE UNIQUE CLUSTERED INDEX IX_Cl ON VwLockedStats (DateId, NormalizedQueryTextId);
CREATE COLUMNSTORE INDEX Ix_AllColumns ON VwLockedStats (DateId, NormalizedQueryTextId, [Count], TotalDuration);
GO
CREATE VIEW VwReaderLogStats WITH SCHEMABINDING
AS 
SELECT rl.DateId
	  ,rl.QueryId NormalizedQueryTextId
	  ,COUNT_BIG(*) [Count]
    ,SUM(ISNULL(rl.Rows, 0)) TotalReads
FROM dbo.ReaderLog rl
GROUP BY rl.DateId, rl.QueryId
GO
CREATE UNIQUE CLUSTERED INDEX IX_Cl ON VwReaderLogStats (DateId, NormalizedQueryTextId);
CREATE COLUMNSTORE INDEX Ix_AllColumns ON VwReaderLogStats (DateId, NormalizedQueryTextId, [Count], TotalReads);
GO

CREATE VIEW VwQueryTextByDate as 
select nqth.Id TextId, 
	d.id DateId
FROM dbo.NormQueryTextHistory nqth
CROSS JOIN dbo.Dates d
go
DROP VIEW IF EXISTS VwQueryStatInfo
go
CREATE VIEW VwQueryStatInfo
AS
SELECT
	qd.DateId [DateId]
   ,CAST(ROUND(s.TotalDuration, 2) AS DECIMAL(18, 2)) TotalDuration
   ,CAST(ROUND(s.TotalDuration / NULLIF(s.[count], 0), 2) AS DECIMAL(18, 2)) AvgDuration
   ,s.[count] [count]
   ,CAST(ROUND(TotalRowCount / NULLIF(s.[count], 0), 2) AS DECIMAL(18, 2)) AvgRowCount
   ,CAST(ROUND(TotalLogicalReads / NULLIF(s.[count], 0), 2) AS DECIMAL(18, 2)) AvgLogicalReads
   ,CAST(ROUND(TotalCPU / NULLIF(s.[count], 0), 2) AS DECIMAL(18, 2)) AvgCPU
   ,CAST(ROUND(TotalWrites / NULLIF(s.[count], 0), 2) AS DECIMAL(18, 2)) AvgWrites
   ,qd.TextId NormalizedQueryTextId
   ,lrs.[count] LockerCount
   ,CAST(ROUND(lrs.TotalDuration, 2) AS DECIMAL(18, 2)) LockerTotalDuration
   ,CAST(ROUND(lrs.TotalDuration / NULLIF(lrs.[count], 0), 2) AS DECIMAL(18, 2)) LockerAvgDuration
   ,lds.[count] LockedCount
   ,CAST(ROUND(lds.TotalDuration, 2) AS DECIMAL(18, 2)) LockedTotalDuration
   ,CAST(ROUND(lds.TotalDuration / NULLIF(lds.[count], 0), 2) AS DECIMAL(18, 2)) LockedAvgDuration
   ,(SELECT COUNT_BIG(*) FROM DeadLocksInfo dli WHERE dli.DateId = qd.DateId AND (dli.QueryAId = qd.TextId OR dli.QuerybId = qd.TextId)) DeadLocksCount
   ,CAST(ROUND(rls.Count, 2) AS DECIMAL(18, 2)) ReaderLogsCount
   ,CAST(ROUND(rls.TotalReads, 2) AS DECIMAL(18, 2)) TotalReaderLogsReads
   ,CAST(ROUND(rls.TotalReads / NULLIF(rls.Count, 0), 2) AS DECIMAL(18, 2)) AvgReaderLogsReads
   ,(SELECT COUNT_BIG(DISTINCT rl.StackId) FROM ReaderLog rl WHERE rl.DateId = qd.DateId AND rl.QueryId = qd.TextId) DistinctReaderLogsStacks
FROM VwQueryTextByDate qd
LEFT JOIN dbo.VwStatementStats s ON qd.TextId = s.NormalizedQueryTextId AND s.DateId = qd.DateId
LEFT JOIN dbo.VwLockersStats lrs ON qd.TextId = lrs.NormalizedQueryTextId AND lrs.DateId = qd.DateId
LEFT JOIN dbo.VwLockedStats lds ON qd.TextId = lds.NormalizedQueryTextId AND lds.DateId = qd.DateId
LEFT JOIN dbo.VwReaderLogStats rls ON qd.TextId = rls.NormalizedQueryTextId AND rls.DateId = qd.DateId

GO

GO
SELECT TOP 0 *
INTO Grouped_QueryStatInfo
FROM VwQueryStatInfo;

CREATE CLUSTERED INDEX ix_cl ON Grouped_QueryStatInfo (DateId, NormalizedQueryTextId);
CREATE COLUMNSTORE INDEX Ix_1 ON Grouped_QueryStatInfo (DateId, TotalDuration, AvgDuration, count, AvgRowCount
, AvgLogicalReads, AvgCPU, AvgWrites, NormalizedQueryTextId
,LockerCount, LockerTotalDuration, LockerAvgDuration, DeadLocksCount
,ReaderLogsCount,TotalReaderLogsReads,AvgReaderLogsReads,DistinctReaderLogsStacks);
GO

CREATE VIEW QueryStatInfo AS
SELECT gi.*
	,t.NormalizedQuery QueryText
	,d.[Date] [Date]
FROM Grouped_QueryStatInfo gi
INNER JOIN	 dbo.NormQueryTextHistory t ON t.Id = gi.NormalizedQueryTextId
INNER JOIN	dbo.Dates d ON d.Id = gi.DateId
GO

DROP PROCEDURE  IF EXISTS ActualizeQueryStatInfo;
GO

CREATE PROCEDURE ActualizeQueryStatInfo AS BEGIN
set QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

DECLARE @date DATETIME2 = DATEADD(DAY, -5, CAST(GETDATE() AS DATE));
IF NOT EXISTS (SELECT * FROM Grouped_QueryStatInfo) BEGIN
	SET @date = CAST('2000-01-01' AS DATE);
END ELSE BEGIN
	DELETE Grouped_QueryStatInfo
	FROM  dbo.Dates d
	WHERE d.[Date] >= @date
	AND Id = d.Id
END;

PRINT 'Date: ' + CAST(@date AS NVARCHAR(MAX));

INSERT INTO Grouped_QueryStatInfo
SELECT v.*
FROM VwQueryStatInfo v
INNER JOIN dbo.Dates d ON d.Id = v.DateId
WHERE d.[Date] >= @date;

DBCC SHRINKFILE (N'work_analisys', 0, TRUNCATEONLY)
DBCC SHRINKFILE (N'work_analisys_log', 0, TRUNCATEONLY)

END

--EXEC ActualizeQueryStatInfo
-- delete Grouped_QueryStatInfo