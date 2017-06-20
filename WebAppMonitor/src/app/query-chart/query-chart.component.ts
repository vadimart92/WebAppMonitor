import { Component, OnInit, Input } from '@angular/core';
import * as moment from 'moment';
import { ChartsModule } from 'ng2-charts/ng2-charts';
import Chart from 'chart.js'
import * as _ from 'underscore';
import { Observable } from 'rxjs/Observable';

import {TimeUtils} from '../utils/utils'

@Component({
	selector: 'app-query-chart',
	templateUrl: './query-chart.component.html',
    styleUrls: ['./query-chart.component.css']
})


export class QueryChartComponent implements OnInit {
	
    private _timeUtils: TimeUtils = new TimeUtils();
	private _chartDataRows: any[] = [];

	@Input()
    set chartDataRows(rows: any[]) {
		this._chartDataRows = rows;
        var newData = _.clone(this.chartData);
		newData[0].data = rows;
		this.chartData = newData;
	}

    get chartDataRows(): any[] { return this._chartDataRows; }

    public chartData: any[] = [
		{
			fill: false,
			data: [],
			label: 'Stats'
		}];
	constructor() {
		
	}
	public chartOptions: any = {
		responsive: true,
        maintainAspectRatio: false,
        fill: false,
		elements: {
			line: {
				tension: 0
			}
		},
		scales: {
			xAxes: [{
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
			},],
			yAxes: [{
                ticks: {
                    callback: (tick, index, ticks) => {
	                    return this._timeUtils.formatAsTime(tick);
                    }
                }
			}]
		}
	};
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
