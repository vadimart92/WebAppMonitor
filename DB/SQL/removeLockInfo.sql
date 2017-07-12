BEGIN TRANSACTION

DELETE DeadLocksInfo;
DELETE LongLocksInfo;
DELETE NormQueryTextSource;

DELETE NormQueryTextHistory
WHERE NOT EXISTS (
  SELECT *
  FROM QueryTextHistory qth
  WHERE qth.NormQueryTextHistoryId = Id  
)

COMMIT TRANSACTION
