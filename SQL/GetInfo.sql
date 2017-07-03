IF (OBJECT_ID('QueryStatInfo', 'V') IS NOT NULL) 
	DROP VIEW QueryStatInfo;
DROP VIEW IF EXISTS QueryStatInfo_Old;
DROP VIEW IF EXISTS VwQueryStatInfo;
DROP VIEW  IF EXISTS StatementStats;
DROP VIEW IF EXISTS LockersStats;
DROP VIEW IF EXISTS LockedStats;
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
CREATE VIEW StatementStats WITH SCHEMABINDING AS 
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
	ON StatementStats (DateId, NormalizedQueryTextId)

CREATE COLUMNSTORE INDEX Ix_AllColumns ON StatementStats (DateId, NormalizedQueryTextId, [Count], TotalDuration, TotalRowCount, TotalLogicalReads, TotalCPU, TotalWrites)
GO
CREATE VIEW LockersStats WITH SCHEMABINDING
AS 
SELECT li.DateId
	  ,li.BlockerQueryId NormalizedQueryTextId
	  ,COUNT_BIG(*) [Count]
	  ,SUM(ISNULL(Duration/1000000, 0)) TotalDuration	
FROM dbo.LongLocksInfo li
GROUP BY li.DateId, li.BlockerQueryId
GO
CREATE UNIQUE CLUSTERED INDEX IX_Cl ON LockersStats (DateId, NormalizedQueryTextId);
CREATE COLUMNSTORE INDEX Ix_AllColumns ON LockersStats (DateId, NormalizedQueryTextId, [Count], TotalDuration);
GO

CREATE VIEW LockedStats WITH SCHEMABINDING
AS 
SELECT li.DateId
	  ,li.BlockedQueryId NormalizedQueryTextId
	  ,COUNT_BIG(*) [Count]
	  ,SUM(ISNULL(Duration/1000000, 0)) TotalDuration
FROM dbo.LongLocksInfo li
GROUP BY li.DateId, li.BlockedQueryId
GO
CREATE UNIQUE CLUSTERED INDEX IX_Cl ON LockedStats (DateId, NormalizedQueryTextId);
CREATE COLUMNSTORE INDEX Ix_AllColumns ON LockedStats (DateId, NormalizedQueryTextId, [Count], TotalDuration);
GO

CREATE VIEW VwQueryStatInfo
AS
SELECT
	d.Date [Date]
   ,CAST(ROUND(s.TotalDuration, 2) AS DECIMAL(18, 2)) TotalDuration
   ,CAST(ROUND(s.TotalDuration / NULLIF(s.[count], 0), 2) AS DECIMAL(18, 2)) AvgDuration
   ,CAST(ROUND(s.[count], 2) AS DECIMAL(18, 2)) [count]
   ,CAST(ROUND(TotalRowCount / NULLIF(s.[count], 0), 2) AS DECIMAL(18, 2)) AvgRowCount
   ,CAST(ROUND(TotalLogicalReads / NULLIF(s.[count], 0), 2) AS DECIMAL(18, 2)) AvgLogicalReads
   ,CAST(ROUND(TotalCPU / NULLIF(s.[count], 0), 2) AS DECIMAL(18, 2)) AvgCPU
   ,CAST(ROUND(TotalWrites / NULLIF(s.[count], 0), 2) AS DECIMAL(18, 2)) AvgWrites
   ,CAST(0 AS DECIMAL(18, 2)) AS AvgAdoReads
   ,nqth.NormalizedQuery QueryText
   ,nqth.Id NormalizedQueryTextId
   ,CAST(ROUND(lrs.[count], 2) AS DECIMAL(18, 2)) LockerCount
   ,CAST(ROUND(lrs.TotalDuration, 2) AS DECIMAL(18, 2)) LockerTotalDuration
   ,CAST(ROUND(lrs.TotalDuration / NULLIF(lrs.[count], 0), 2) AS DECIMAL(18, 2)) LockerAvgDuration
   ,CAST(ROUND(lds.[count], 2) AS DECIMAL(18, 2)) LockedCount
   ,CAST(ROUND(lds.TotalDuration, 2) AS DECIMAL(18, 2)) LockedTotalDuration
   ,CAST(ROUND(lds.TotalDuration / NULLIF(lds.[count], 0), 2) AS DECIMAL(18, 2)) LockedAvgDuration
   ,(SELECT COUNT_BIG(*) FROM DeadLocksInfo dli WHERE dli.DateId = d.Id AND (dli.QueryAId = nqth.Id OR dli.QuerybId = nqth.Id)) DeadLocksCount
FROM dbo.NormQueryTextHistory nqth
CROSS JOIN dbo.Dates d
LEFT JOIN dbo.StatementStats s ON nqth.Id = s.NormalizedQueryTextId AND s.DateId = d.Id
LEFT JOIN dbo.LockersStats lrs 	ON nqth.Id = lrs.NormalizedQueryTextId AND lrs.DateId = d.Id
LEFT JOIN dbo.LockedStats lds 	ON nqth.Id = lds.NormalizedQueryTextId AND lds.DateId = d.Id

GO

DROP PROCEDURE  IF EXISTS ActualizeQueryStatInfo;
GO

CREATE PROCEDURE ActualizeQueryStatInfo AS BEGIN

DROP TABLE IF EXISTS QueryStatInfo;

SELECT *
INTO QueryStatInfo
FROM VwQueryStatInfo;

CREATE CLUSTERED INDEX ix_cl ON QueryStatInfo (Date, NormalizedQueryTextId);
CREATE COLUMNSTORE INDEX Ix_1 ON QueryStatInfo (Date, TotalDuration, AvgDuration, count, AvgRowCount
, AvgLogicalReads, AvgCPU, AvgWrites, NormalizedQueryTextId,
LockerCount, LockerTotalDuration, LockerAvgDuration, DeadLocksCount);


DBCC SHRINKFILE (N'work_analisys', 0, TRUNCATEONLY)
DBCC SHRINKFILE (N'work_analisys_log', 0, TRUNCATEONLY)

END

--EXEC ActualizeQueryStatInfo