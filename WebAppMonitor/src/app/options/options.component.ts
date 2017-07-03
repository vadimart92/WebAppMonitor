import { Component, OnInit } from '@angular/core';
import { MdSnackBar } from '@angular/material';
import { Http } from '@angular/http';
import { Router, NavigationEnd } from '@angular/router';
import * as moment from 'moment';
import { AdminService } from '../admin.service';
import { SettingsService } from "../settings.service"

@Component({
	selector: 'app-options',
	templateUrl: './options.component.html',
	styleUrls: ['./options.component.css']
})
export class OptionsComponent implements OnInit {
	lastQueryInHistory: Date;
	totalRecords: number;
	constructor(public snackBar: MdSnackBar, private _http: Http, private router: Router, private _adminService: AdminService, private settingsService: SettingsService) {
		router.events.subscribe((val) => {
			if (val instanceof NavigationEnd) {
				this.snackBar.dismiss();
			}
		});
	}
	moment = moment;
	importInProgress: boolean = false;
	fileName: string;
	importJobActive: boolean = false;
	importSettings: any = null;
	ngOnInit() {
		var date = moment().format("YYYY-MM-DD");
		this.fileName = "\\\\tscore-dev-13\\WorkAnalisys\\xevents\\Export_" + date + "\\" + date + "\\ts_sqlprofiler_05_sec*.xel";
		this.refreshStatsInfo();
	}
	clearCache() {
		this._http.post("/api/QueryStatsData/clearCache", {})
			.subscribe(() => {
				this.snackBar.open("Done", null, {
					duration: 1000,
				});
			});
	}
	clearAllSettings() {
		this.settingsService.clearAllSettings();
		window.location.reload(true);
	}
	async refreshStatsInfo() {
		var info = await this._adminService.getStatsInfo();
		this.lastQueryInHistory = info.LastQueryInHistory;
		this.totalRecords = info.TotalRecords;
		this.importJobActive = info.ImportJobActive;
		this.importSettings = info.ImportSettings;
	}
	importData(fileName: string) {
		this.snackBar.dismiss();
		this.importInProgress = true;
		this._http.post("/api/Admin/importDailyData", { ImportSettings: null})
			.subscribe(() => {
				this.importInProgress = false;
				this.snackBar.open("Done", null, {
					duration: 100000
				});
				this.refreshStatsInfo();
			}, (error) => {
				this.importInProgress = false;
				this.snackBar.open(error.text(), "Error", {
					duration: 100000
				});
				this.refreshStatsInfo();
			});
	}
	toggleImportJob() {
		this._http.post("/api/Admin/toggleImportJob", null)
			.subscribe(() => {
				this.refreshStatsInfo();
				this.snackBar.open("Done", null, {
					duration: 1000
				});
			});
	}
	openHangfire() {
		window.open("/hangfire", '_blank');
	}
}
