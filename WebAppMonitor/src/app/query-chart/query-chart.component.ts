import { Component, OnInit, Input } from '@angular/core';
import * as moment from 'moment';
import * as _ from 'underscore';

import {TimeUtils} from '../utils/utils'

export enum ChartAxisType {
	Number,
	Time
}

export class ChartData {
	seriesData: any[];
	yAxisType: ChartAxisType;
	chartCaption: string; 
	constructor() {
		this.seriesData = [];
		this.yAxisType = ChartAxisType.Number;
	}
}


@Component({
	selector: 'app-query-chart',
	templateUrl: './query-chart.component.html',
    styleUrls: ['./query-chart.component.css']
})


export class QueryChartComponent implements OnInit {
	
    private _timeUtils: TimeUtils = new TimeUtils();
	private _chartData: ChartData;

	@Input()
	set chartData(data: ChartData) {
		if (!data) {
			return;
		}
		this._chartData = data;
		var newData = [{
			fill: false,
			data: data.seriesData,
			label: ''
		}];
		this.setYAxisType(data.yAxisType);
		this.chartDataSets = newData;
	}

	get chartData(): ChartData { return this._chartData; }

	chartDataSets: any[];
	chartYAxisType: ChartAxisType = ChartAxisType.Number;

	public chartOptions: any;
	constructor() {
		this.initChartOptions();
	}
	setYAxisType(type: ChartAxisType) {
		if (type && this.chartYAxisType !== type) {
			this.chartYAxisType = type;
			this.initChartOptions();
		}
	}
	getXAxisConfig() {
		return {
			type: "time",
			time: {
				displayFormats: {
					day: 'YYYY-MM-DD'
				},
				unit: 'day',
				tooltipFormat: 'll HH:mm'
			},
			scaleLabel: {
				display: true,
				labelString: 'Date'
			}
		};
	}
	getYAxisConfig() {
		if (this.chartYAxisType === ChartAxisType.Time) {
			return {
				ticks: {
					callback: (tick, index, ticks) => {
						return this._timeUtils.formatAsTime(tick);
					}
				}
			}
		};
		return {}
	}
	initChartOptions() {
		let xAxisConfig = this.getXAxisConfig();
		let yAxisConfig = this.getYAxisConfig();
		this.chartOptions = {
			responsive: true,
			maintainAspectRatio: false,
			fill: false,
			elements: {
				line: {
					tension: 0
				}
			},
			scales: {
				xAxes: [xAxisConfig],
				yAxes: [yAxisConfig]
			}
		};
	}

	public chartColors: Array<any> = [
		{ // grey
			backgroundColor: 'rgba(148,159,177,0.2)',
			borderColor: 'rgba(148,159,177,1)',
			pointBackgroundColor: 'rgba(148,159,177,1)',
			pointBorderColor: '#fff',
			pointHoverBackgroundColor: '#fff',
			pointHoverBorderColor: 'rgba(148,159,177,0.8)'
		},
		{ // dark grey
			backgroundColor: 'rgba(77,83,96,0.2)',
			borderColor: 'rgba(77,83,96,1)',
			pointBackgroundColor: 'rgba(77,83,96,1)',
			pointBorderColor: '#fff',
			pointHoverBackgroundColor: '#fff',
			pointHoverBorderColor: 'rgba(77,83,96,1)'
		},
		{ // grey
			backgroundColor: 'rgba(148,159,177,0.2)',
			borderColor: 'rgba(148,159,177,1)',
			pointBackgroundColor: 'rgba(148,159,177,1)',
			pointBorderColor: '#fff',
			pointHoverBackgroundColor: '#fff',
			pointHoverBorderColor: 'rgba(148,159,177,0.8)'
		}
	];
	public chartLegend: boolean = false;
	public chartType: string = 'line';

	// events
	public chartClicked(e: any): void {
		console.log(e);
	}

	public chartHovered(e: any): void {
		console.log(e);
		
	}
    ngOnInit() {
	}
}
