import { Component, Output, Input, EventEmitter, OnInit} from '@angular/core';
import { MdSnackBar, MdSnackBarConfig } from '@angular/material';
import * as moment from 'moment';
import * as _ from 'underscore';
import { HotkeysService, Hotkey } from 'angular2-hotkeys';

import { TimeUtils } from '../utils/utils'
import { QueryStatsService } from '../query-stats/query-stats.service'
import { ApiDataService, StatsQueryOptions } from '../data.service'
import { ChartData } from "../common/charting"
import { QueryStatInfo, QueryStatInfoDisplayConfig } from "../entities/query-stats-info"
import { SettingsService } from "../settings.service"
import {PreferredStackSource, StackListComponentOptions} from "../stack-list/stack-list.component";

export class QueryData {
	info: QueryStatInfo = null;
	date: Date = null;
}

@Component({
	selector: 'app-query-info',
	templateUrl: './query-info.component.html',
	styleUrls: ['./query-info.component.css']
})
export class QueryInfoComponent implements OnInit {

	@Output() hide = new EventEmitter();
	@Input() queryData: QueryData;
	@Input() visibleColumns: string[] = [];
    private timeUtils = new TimeUtils();
    private queryStatsInfo: any[];
	public avgDurationData: ChartData;
	public countData: ChartData;
	public totalDurationData: ChartData;
	public selectedTabIndex: number = 1;
	public chartData: any[];
	public stackListOptions: StackListComponentOptions;
	private  chartsConfig: Object;
	private displayConfigProvider = new QueryStatInfoDisplayConfig();
	private columnsConfig: any[];
	constructor(private _snackBar: MdSnackBar, private _statsService: QueryStatsService,
				private _hotkeysService: HotkeysService, private _dataService: ApiDataService,
				private settingsService: SettingsService) {
		var navRight = this._hotkeysService.add(new Hotkey('right', this.onArrowKey.bind(this, 1)));
		var navLeft = this._hotkeysService.add(new Hotkey('left', this.onArrowKey.bind(this, -1)));
		var closeHotKey = this._hotkeysService.add(new Hotkey('esc', (event: KeyboardEvent): boolean => {
			this._hotkeysService.remove(closeHotKey);
			this._hotkeysService.remove(navRight);
			this._hotkeysService.remove(navLeft);
			this.hideMe();
			return false;
		}));
		this.chartsConfig = this.displayConfigProvider.getChartsConfig();
		this.columnsConfig = this.displayConfigProvider.getColumnsConfig().columns;
		this.initQueryDataMock();
	}
	onArrowKey(direction: number, event: KeyboardEvent): boolean {
		this.tryChangeTab(direction);
		return false;
	}
	tryChangeTab(direction: number):void {
		var result = this.selectedTabIndex + direction;
		if (result > -1 && result < this.chartsData.length + 1) {
			this.selectedTabIndex = result;
		}
	}
	getCurrentInfo():QueryStatInfo {
		return this.queryData.info;
	}
	async ngOnInit() {
		let info = this.getCurrentInfo();
		let rows = await this._dataService.getStats(<StatsQueryOptions>{
			orderBy: ["date"],
			queryTextId: info.normalizedQueryTextId,
			dateId: info.dateId
		});
		_.extend(this.queryData.info, rows[rows.length - 1]);
		this.initStackListOptions();
		this.prepareChartData(rows);
	}

	private initQueryDataMock() {
		if (!this.queryData) {
			this.queryData = <QueryData>{
				date: new Date(),
				info: {
					"normalizedQueryTextId": "a73a0e9c-acb7-4921-a5a6-db0781eb605a",
					"dateId": undefined
				}
			};
			let visibleColumnIds = this.settingsService.getSettingsProvider("queryStatsGrid").getVisibleColumnIds();
			this.visibleColumns = this.visibleColumns.concat(visibleColumnIds);
		}
	}

	chartsData: any[] = [];
	prepareChartData(rows) {
		this.queryStatsInfo = rows;
		var chartsData = _.mapObject(this.chartsConfig, (chartConfig) => {
			var data = new ChartData();
			data.yAxisType = chartConfig.yAxisType;
			data.chartCaption = chartConfig.label;
			data.column = chartConfig.dataColumn;
			return data;
		});
		var chartNames = _.keys(this.chartsConfig);
		let seriesDataEmpty = true;
		_.each(this.queryStatsInfo, (infoRow) => {
			let date = infoRow.date;
			_.each(chartNames, (chartName) => {
				var config = this.chartsConfig[chartName];
				let yValue = infoRow[config.dataColumn];
				if (seriesDataEmpty && !yValue) {
					return;
				} else {
					seriesDataEmpty = false;
				}
				chartsData[chartName].seriesData.push({
					x: date,
					y: yValue
				});
			});
		});
		this.chartsData = _.values(chartsData);
	}
	public hideMe(): void {
		this.hide.emit(null);
	}
	public onCopySql() {
		this._snackBar.open("Done", null, <MdSnackBarConfig>{
			duration: 1000
		});
	}
	public async formatSql() {
		var text = await this._statsService.formatSql(this.getCurrentInfo());
		this.queryData.info.formattedText = text;
	}

	private initStackListOptions() {
		this.stackListOptions = new StackListComponentOptions();
		let info = this.getCurrentInfo();
		this.stackListOptions.preferredStackSource = (info.distinctReaderLogsStacks > info.distinctExecutorLogsStacks)
			? PreferredStackSource.Reader
			: PreferredStackSource.Executor;
	}
	getIsStacksVisible():boolean {
		let info = this.getCurrentInfo();
		return Boolean(info.distinctReaderLogsStacks || info.distinctExecutorLogsStacks);
	}
}
