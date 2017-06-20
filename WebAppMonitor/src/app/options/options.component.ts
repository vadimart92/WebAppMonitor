import { Component, OnInit } from '@angular/core';
import { MdSnackBar } from '@angular/material';
import { Http } from '@angular/http';
import * as moment from 'moment';

@Component({
	selector: 'app-options',
	templateUrl: './options.component.html',
	styleUrls: ['./options.component.css']
})
export class OptionsComponent implements OnInit {

	constructor(public snackBar: MdSnackBar, private  _http: Http) { }
	importInProgress: boolean = false;
	fileName: string;
	ngOnInit() {
		var date = moment().format("YYYY-MM-DD");
		this.fileName = "\\\\tscore-dev-13\\WorkAnalisys\\xevents\\Export_" + date + "\\" + date + "\\ts_sqlprofiler_05_sec*.xel";
	}
	clearCache() {
		this._http.post("/api/QueryStatsData/clearCache", {})
			.subscribe(() => {
				this.snackBar.open("Done", null, {
					duration: 1000,
				});
			});
	}
	importData(fileName:string) {
		this.importInProgress = true;
		this._http.post("/api/Admin/importDailyData", { fileName: fileName})
			.subscribe(() => {
				this.importInProgress = false;
				this.snackBar.open("Done", null, {
					duration: 100000
				});
			}, (error) => {
				this.importInProgress = false;
				this.snackBar.open(error.text(), "Error", {
					duration: 100000
				});
			});
		
	}
}
