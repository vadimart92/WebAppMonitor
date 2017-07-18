import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER, } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule, MdNativeDateModule } from '@angular/material';
import 'hammerjs';
import { AgGridModule } from "ag-grid-angular/main";
import 'moment-duration-format';
import { ClipboardModule } from 'ngx-clipboard';
import { HotkeyModule } from 'angular2-hotkeys';
import { ChartsModule } from 'ng2-charts/ng2-charts';
import { BreezeBridgeAngularModule } from 'breeze-bridge-angular' 
import { ɵg } from "@angular/router";


import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing/app-routing.module';
import { QueryStatsComponent } from './query-stats/query-stats.component';
import { QueryStatsService } from './query-stats/query-stats.service';
import { QueryInfoComponent } from './query-info/query-info.component';
import { RowsLoadingDialogComponent } from './rows-loading-dialog/rows-loading-dialog.component';
import { OptionsComponent } from './options/options.component';
import { QueryChartComponent } from './query-chart/query-chart.component';
import { AdminService } from './admin.service';
import { ApiDataService } from './data.service';
import { IsChartVisible, IfProp } from './utils/ng-utils';
import { SettingsService } from "./settings.service";
import { StackListComponent } from './stack-list/stack-list.component'

export function appInit(breezeBridgeAngularModule: BreezeBridgeAngularModule, routerInitializer: ɵg, apiDataService: ApiDataService) {
	var entityManager = apiDataService.getEntityManager();
	return () => routerInitializer.appInitializer().then(() => {
		return entityManager.fetchMetadata().then((metadata: Object) => {
			breezeBridgeAngularModule["metadata"] = metadata;
			apiDataService.onMetadataInitialized();
			return metadata;
		});
	});
}

@NgModule({
	declarations: [
		AppComponent,
		QueryStatsComponent,
		QueryInfoComponent,
		RowsLoadingDialogComponent,
		OptionsComponent,
		QueryChartComponent,
		IsChartVisible,
		IfProp,
		StackListComponent
	],
	imports: [
		BrowserModule,
		BrowserAnimationsModule,
		AppRoutingModule,
		MaterialModule,
		FormsModule,
		MdNativeDateModule,
		HttpModule,
		AgGridModule.withComponents([]),
		ClipboardModule,
		HotkeyModule.forRoot(),
		ChartsModule,
		BreezeBridgeAngularModule
	],
	entryComponents: [RowsLoadingDialogComponent],
	providers: [
		ApiDataService,
		{
			"provide": APP_INITIALIZER,
			"useFactory": appInit,
			"deps": [BreezeBridgeAngularModule, ɵg, ApiDataService],
			"multi": true
		},
		QueryStatsService, AdminService, SettingsService
	],
	bootstrap: [AppComponent]
})
export class AppModule { }

