CREATE EVENT SESSION [collect_long_locks_data] ON SERVER 
ADD EVENT sqlserver.blocked_process_report(
    ACTION(sqlserver.database_id,sqlserver.database_name)
    WHERE ([package0].[greater_than_equal_uint64]([duration],(20000000))))
ADD TARGET package0.event_file(SET filename=N'collect_long_locks_data',max_file_size=(10),max_rollover_files=(200)),
ADD TARGET package0.ring_buffer(SET max_memory=(2048))
WITH (MAX_MEMORY=4096 KB,EVENT_RETENTION_MODE=ALLOW_MULTIPLE_EVENT_LOSS,MAX_DISPATCH_LATENCY=30 SECONDS,MAX_EVENT_SIZE=0 KB,MEMORY_PARTITION_MODE=NONE,TRACK_CAUSALITY=OFF,STARTUP_STATE=ON)
GO

CREATE EVENT SESSION [collect_deadlock_data] ON SERVER 
ADD EVENT sqlserver.xml_deadlock_report(
    ACTION(sqlserver.client_app_name,sqlserver.client_hostname,sqlserver.database_id,sqlserver.database_name,sqlserver.sql_text))
ADD TARGET package0.event_file(SET filename=N'collect_deadlock_data',max_file_size=(1),max_rollover_files=(20)),
ADD TARGET package0.ring_buffer(SET max_memory=(10240))
WITH (MAX_MEMORY=500 KB,EVENT_RETENTION_MODE=ALLOW_SINGLE_EVENT_LOSS,MAX_DISPATCH_LATENCY=30 SECONDS,MAX_EVENT_SIZE=0 KB,MEMORY_PARTITION_MODE=NONE,TRACK_CAUSALITY=OFF,STARTUP_STATE=ON)
GO

CREATE EVENT SESSION [ts_sqlprofiler_05_sec] ON SERVER 
ADD EVENT sqlserver.rpc_completed(SET collect_statement=(1)
    ACTION(sqlserver.client_hostname,sqlserver.database_name,sqlserver.session_id,sqlserver.sql_text,sqlserver.username)
    WHERE ([package0].[greater_than_equal_uint64]([duration],(500000)) OR [package0].[greater_than_equal_uint64]([cpu_time],(500000)) AND [sqlserver].[database_name]=N'BPMonlineWorkRUS')),
ADD EVENT sqlserver.sql_statement_completed(
    ACTION(sqlserver.client_hostname,sqlserver.database_name,sqlserver.session_id,sqlserver.sql_text,sqlserver.username)
    WHERE ([package0].[greater_than_equal_int64]([duration],(500000)) OR [package0].[greater_than_equal_uint64]([cpu_time],(500000)) AND [sqlserver].[database_name]=N'BPMonlineWorkRUS'))
ADD TARGET package0.event_file(SET filename=N'ts_sqlprofiler_05_sec.xel',max_file_size=(50),max_rollover_files=(200),metadatafile=N'ts_sqlprofiler_05_sec.xem')
WITH (MAX_MEMORY=1024 KB,EVENT_RETENTION_MODE=ALLOW_SINGLE_EVENT_LOSS,MAX_DISPATCH_LATENCY=30 SECONDS,MAX_EVENT_SIZE=0 KB,MEMORY_PARTITION_MODE=NONE,TRACK_CAUSALITY=OFF,STARTUP_STATE=ON)
GO



