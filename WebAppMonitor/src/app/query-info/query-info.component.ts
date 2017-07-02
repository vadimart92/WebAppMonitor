import { Component, Output, Input, EventEmitter, OnInit} from '@angular/core';
import { MdSnackBar } from '@angular/material';
import * as moment from 'moment';
import * as _ from 'underscore';
import { HotkeysService, Hotkey } from 'angular2-hotkeys';

import { TimeUtils } from '../utils/utils'
import { QueryStatsService } from '../query-stats/query-stats.service'
import { ApiDataService, StatsQueryOptions } from '../data.service'
import { ChartAxisType, ChartData } from "../common/charting"
import { QueryStatInfo, QueryStatInfoDisplayConfig } from "../entities/query-stats-info"

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
	public selectedTabIndex: number = 0;
	public chartData: any[];
	private  chartsConfig: Object;
	private displayConfigProvider = new QueryStatInfoDisplayConfig();
	constructor(private _snackBar: MdSnackBar, private _statsService: QueryStatsService, private _hotkeysService: HotkeysService, private _dataService: ApiDataService) {
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
	ngOnInit() {
		if (!this.queryData) {
			this.queryData = <QueryData>{
				date: new Date(),
				info: {
					"normalizedQueryTextId": "a8c059cd-88f1-4a25-a6e5-5334c1fc79ef",
					"count": 1,
					"totalDuration": 584.00,
					"avgDuration": 584.000000,
					"avgRowCount": 31212,
					"avgCPU": 554.000000,
					"avgLogicalReads": 279581951,
					"avgWrites": 10595,
					"queryText": " sql",
					"avgAdoReads": 10
				}
			}
		}
		var info = this.getCurrentInfo();
		this._dataService.getStats(<StatsQueryOptions>{
			orderBy: ["date"],
			queryTextId: info.normalizedQueryTextId
		}).then(rows => {
			this.prepareChartData(rows);
		});
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
	public formatTime(seconds: number): string {
		return this.timeUtils.formatAsTime(seconds);
	}
	public formatDate(date: Date): string {
		return this.timeUtils.formatAsDate(date);
	}
	public onCopySql() {
		this._snackBar.open("Done", null, {
			duration: 1000
		});
	}
	public formatSql() {
		this._statsService.formatSql(this.getCurrentInfo())
			.then(text => {
				this.queryData.info.formatedText = text;
			});
	}
}


