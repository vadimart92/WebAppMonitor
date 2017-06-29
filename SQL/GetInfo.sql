IF OBJECT_ID('GetQueriesByTotalTime', 'P') IS NULL
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = 'CREATE PROCEDURE GetQueriesByTotalTime AS BEGIN SET NOCOUNT ON END'
	EXEC sys.sp_executesql @SQL
END
GO
ALTER PROCEDURE GetQueriesByTotalTime @date DATE, @rowsCount INT = 100, @totalDuration INT = 300 AS BEGIN
	SELECT qth.NormQueryTextHistoryId NormQueryTextHistoryId
		, COUNT(qh.Id) Count
	   ,SUM(qh.duration_sec) TotalDuration
	   ,AVG(qh.duration_sec) AverageDuration
		,Round(AVG(qh.row_count), 2) AverageRowCount
		,ROUND(AVG(qh.cpu_time_sec), 2) AverageCPU
		,ROUND(AVG(qh.logical_reads), 2) AverageLogicalReads
		,Round(AVG(qh.writes), 2) AverageWrites
		, (SELECT nqth.NormalizedQuery FROM NormQueryTextHistory nqth WHERE nqth.Id = qth.NormQueryTextHistoryId ) QueryText
	FROM QueryHistory qh
	INNER JOIN QueryTextHistory qth ON qth.QueryHistoryId = qh.Id
	WHERE qh.DateId = (SELECT Id FROM Dates d WHERE d.Date = @date)
	GROUP BY qth.NormQueryTextHistoryId
	HAVING SUM(qh.duration_sec) > @totalDuration
	ORDER BY AVG(qh.duration_sec) DESC
	OFFSET 0 ROWS FETCH FIRST @rowsCount+100 ROWS ONLY
END
GO
IF (OBJECT_ID('QueryStatInfo', 'V')) IS NOT NULL BEGIN
  DROP VIEW QueryStatInfo;  
END
go
CREATE VIEW QueryStatInfo AS 
  SELECT 
		d.Date [Date]
      , CAST(ROUND(SUM(qh.duration_sec),2) AS DECIMAL(18,2)) TotalDuration
      , CAST(ROUND(AVG(qh.duration_sec),2) AS DECIMAL(18,2)) AvgDuration
      , CAST(ROUND(COUNT(qth.QueryHistoryId),2) AS DECIMAL(18,2)) Count
	  , CAST(ROUND(AVG(qh.row_count),2) AS DECIMAL(18,2)) AvgRowCount
	  , CAST(ROUND(AVG(qh.logical_reads),2) AS DECIMAL(18,2)) AvgLogicalReads
	 , CAST(ROUND(AVG(qh.cpu_time_sec),2) AS DECIMAL(18,2)) AvgCPU
	 , CAST(ROUND(AVG(qh.writes),2) AS DECIMAL(18,2)) AvgWrites
	 ,  CAST(0 AS DECIMAL(18,2)) AS AvgAdoReads
	,nqth.NormalizedQuery QueryText
	,qth.NormQueryTextHistoryId NormalizedQueryTextId
  FROM QueryTextHistory qth
  INNER JOIN QueryHistory qh ON qth.QueryHistoryId = qh.Id
  INNER JOIN Dates d ON qh.DateId = d.Id
  INNER JOIN NormQueryTextHistory nqth ON qth.NormQueryTextHistoryId = nqth.Id
  GROUP BY qth.NormQueryTextHistoryId, nqth.NormalizedQuery, d.Date
GO
