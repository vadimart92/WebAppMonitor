DROP PROCEDURE IF EXISTS sp_ReplaceRecord;
GO
CREATE PROCEDURE sp_ReplaceRecord (@tableName NVARCHAR(MAX), @mapTableName NVARCHAR(MAX)) AS BEGIN

	DECLARE @SQL NVARCHAR(MAX);
	
	DECLARE cur CURSOR FAST_FORWARD READ_ONLY LOCAL FOR
	SELECT 'UPDATE [' + t.name + '] SET [' + c.name + '] = map.NewId FROM ['+@mapTableName+'] map WHERE [' + c.name + '] = map.OldId' AS query
	FROM sys.foreign_key_columns AS fk
	INNER JOIN sys.tables AS t ON fk.parent_object_id = t.object_id
	INNER JOIN sys.columns AS c ON fk.parent_object_id = c.object_id
		AND fk.parent_column_id = c.column_id
	WHERE fk.referenced_object_id = OBJECT_ID(@tableName, 'U')
	
	OPEN cur
	
	FETCH NEXT FROM cur INTO @SQL
	BEGIN TRANSACTION
	
	WHILE @@FETCH_STATUS = 0 BEGIN
	
	    --PRINT @SQL
	    EXEC sys.sp_executesql @SQL = @SQL
	
		FETCH NEXT FROM cur INTO @SQL
	
	END
	
	CLOSE cur
	DEALLOCATE cur

	SET @SQL = 'DELETE FROM ['+@tableName+'] WHERE [Id] IN (SELECT map.OldId FROM ['+@mapTableName+'] map)'
	--PRINT @SQL
	EXEC sys.sp_executesql @SQL = @SQL

	COMMIT TRANSACTION
END;
GO
--usage
---CREATE TABLE #map (OldId UNIQUEIDENTIFIER, NewId UNIQUEIDENTIFIER)ж
--insert
--EXEC sp_ReplaceRecord @tableName='NormQueryTextHistory', @mapTableName='#map'
--DROP TABLE #map


-- Trim text or replace by existing
DROP PROCEDURE IF EXISTS sp_TrimNormalizedQueries;
GO
CREATE PROCEDURE sp_TrimNormalizedQueries AS BEGIN

	DROP TABLE IF EXISTS trimData;

	BEGIN TRANSACTION

	SELECT nqth.NormalizedQuery OldQuery
			,norm.NormalizedQuery NewQuery
			, nqth.Id OldQueryId 
			, norm.Id NewQueryId
	INTO trimData
	FROM NormQueryTextHistory nqth
	LEFT JOIN NormQueryTextHistory norm ON norm.QueryHash = HASHBYTES('SHA2_512', RTRIM(LTRIM(nqth.NormalizedQuery)))
	WHERE DATALENGTH(nqth.NormalizedQuery) <> DATALENGTH(RTRIM(LTRIM(nqth.NormalizedQuery)));

	UPDATE NormQueryTextHistory
	SET NormalizedQuery = RTRIM(LTRIM(NormalizedQuery))
		,QueryHash = HASHBYTES('SHA2_512', RTRIM(LTRIM(NormalizedQuery)))
	FROM trimData d
	WHERE d.NewQueryId is NULL
		AND Id = d.OldQueryId;

	DELETE FROM NormQueryTextSource
	WHERE EXISTS (
		SELECT h.QueryHistoryId
		FROM trimData d
		INNER JOIN QueryTextHistory h ON h.NormQueryTextHistoryId = d.NewQueryId
		WHERE d.OldQueryId = NormQueryTextId
	)

	CREATE TABLE #map (OldId UNIQUEIDENTIFIER, NewId UNIQUEIDENTIFIER);

	INSERT INTO #map
	SELECT OldQueryId, NewQueryId
	FROM trimData
	WHERE NewQueryId IS NOT NULL

	EXEC sp_ReplaceRecord @tableName='NormQueryTextHistory', @mapTableName='#map'
	DROP TABLE #map

	COMMIT TRANSACTION

END

--exec sp_TrimNormalizedQueries

SELECT COUNT(*)
FROM NormQueryTextHistory nqth
WHERE DATALENGTH(nqth.NormalizedQuery) <> DATALENGTH(RTRIM(LTRIM(nqth.NormalizedQuery)));