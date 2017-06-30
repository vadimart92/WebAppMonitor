DROP VIEW IF EXISTS QueryStatInfo_Old;
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

DROP VIEW IF EXISTS VwQueryStatInfo;
DROP VIEW  IF EXISTS StatementStats;
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

IF (OBJECT_ID('QueryStatInfo', 'V')) IS NOT NULL BEGIN
  DROP VIEW QueryStatInfo;  
END
GO	
DROP VIEW IF EXISTS LockersStats;
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
   ,CAST(ROUND(ls.[count], 2) AS DECIMAL(18, 2)) [LockerCount]
   ,CAST(ROUND(ls.TotalDuration, 2) AS DECIMAL(18, 2)) LockerTotalDuration
   ,CAST(ROUND(ls.TotalDuration / NULLIF(ls.[count], 0), 2) AS DECIMAL(18, 2)) LockerAvgDuration
FROM dbo.NormQueryTextHistory nqth
CROSS JOIN dbo.Dates d
LEFT JOIN dbo.StatementStats s
	ON nqth.Id = s.NormalizedQueryTextId
	AND s.DateId = d.Id
LEFT JOIN dbo.LockersStats ls
	ON nqth.Id = ls.NormalizedQueryTextId
	AND ls.DateId = d.Id

GO

DROP PROCEDURE  IF EXISTS ActualizeQueryStatInfo;
CREATE PROCEDURE ActualizeQueryStatInfo AS BEGIN

DROP TABLE IF EXISTS QueryStatInfo;
SELECT *
INTO QueryStatInfo
FROM VwQueryStatInfo;

CREATE CLUSTERED INDEX ix_cl ON QueryStatInfo (Date, NormalizedQueryTextId);
CREATE COLUMNSTORE INDEX Ix_1 ON QueryStatInfo (Date, TotalDuration, AvgDuration, count, AvgRowCount
, AvgLogicalReads, AvgCPU, AvgWrites, NormalizedQueryTextId,
LockerCount, LockerTotalDuration, LockerAvgDuration);

END