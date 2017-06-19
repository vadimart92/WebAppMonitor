import { Component, Output, Input, EventEmitter, OnInit } from '@angular/core';
import { MdSnackBar } from '@angular/material';
import * as moment from 'moment';
import { TimeUtils } from '../utils/utils'
import { QueryStatsService } from '../query-stats/query-stats.service'

@Component({
	selector: 'app-query-info',
	templateUrl: './query-info.component.html',
	styleUrls: ['./query-info.component.css']
})
export class QueryInfoComponent implements OnInit {

	@Output() hide = new EventEmitter();
	@Input() queryData: any;
	private _timeUtils = new TimeUtils();
	constructor(private _snackBar: MdSnackBar, private _statsService: QueryStatsService) {
		
	}

	ngOnInit() {
		if (!this.queryData) {
			this.queryData = {
				date: new Date(),
				info: {
					"normQueryTextHistoryId": "a8c059cd-88f1-4a25-a6e5-5334c1fc79ef",
					"count": 1,
					"totalDuration": 584.00,
					"averageDuration": 584.000000,
					"averageRowCount": 31212,
					"averageCPU": 554.000000,
					"averageLogicalReads": 279581951,
					"averageWrites": 10595,
					"queryText": " INSERT INTO [dbo].[QueueItem]([EntityRecordId], [QueueId], [StatusId]) SELECT TOP 1000 [Contact].[Id] [Id], @QueueId, @StatusId FROM [dbo].[Contact] [Contact] WITH(NOLOCK) WHERE (EXISTS ( SELECT [SubLead].[Id] [Id] FROM [dbo].[Lead] [SubLead] WITH(NOLOCK) WHERE [SubLead].[QualifiedContactId] = [Contact].[Id] AND [SubLead].[BulkEmailId] IN (@P1, @P2, @P3, @P4, @P5)) OR ( SELECT COUNT([SubBulkEmailTarget].[Id]) [Count] FROM [dbo].[BulkEmailTarget] [SubBulkEmailTarget] WITH(NOLOCK) WHERE [SubBulkEmailTarget].[ContactId] = [Contact].[Id] AND [SubBulkEmailTarget].[BulkEmailId] IN (@P6, @P7, @P8, @P9, @P10) AND [SubBulkEmailTarget].[BulkEmailResponseId] IN (@P11, @P12)) >= @P13) AND NOT EXISTS ( SELECT [Id] FROM [dbo].[QueueItem] WHERE [Contact].[Id] = [QueueItem].[EntityRecordId] AND [QueueItem].[QueueId] = @P14)"
				}
			}
		}
	}
	public hideMe(): void {
		this.hide.emit(null);
	}
	public formatTime(seconds: number): string {
		return this._timeUtils.formatAsTime(seconds);
	}
	public formatDate(date: Date): string {
		return this._timeUtils.formatAsDate(date);
	}
	public onCopySql() {
		this._snackBar.open("Done", null, {
			duration: 1000,
		});
	}
	public formatSql() {
		this._statsService.formatSql(this.queryData.info.normQueryTextHistoryId)
			.then(text => {
				this.queryData.info.formatedText = text;
			});

	}
}
