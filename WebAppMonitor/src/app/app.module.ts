import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule, MdNativeDateModule } from '@angular/material';
import 'hammerjs';
import { AgGridModule } from "ag-grid-angular/main";
import 'moment-duration-format';
import { ClipboardModule } from 'ngx-clipboard';
import { HotkeyModule } from 'angular2-hotkeys';
import { ChartsModule } from 'ng2-charts/ng2-charts';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing/app-routing.module';

import { QueryStatsComponent } from './query-stats/query-stats.component';
import { QueryStatsService } from './query-stats/query-stats.service';
import { QueryInfoComponent } from './query-info/query-info.component';
import { RowsLoadingDialogComponent } from './rows-loading-dialog/rows-loading-dialog.component';
import { OptionsComponent } from './options/options.component';
import { QueryChartComponent } from './query-chart/query-chart.component';

@NgModule({
	declarations: [
		AppComponent,
		QueryStatsComponent,
		QueryInfoComponent,
		RowsLoadingDialogComponent,
		OptionsComponent,
		QueryChartComponent
	],
	imports: [
		BrowserModule,
		BrowserAnimationsModule,
		AppRoutingModule,
		MaterialModule,
		MdNativeDateModule,
		HttpModule,
		AgGridModule.withComponents([]),
		ClipboardModule,
		HotkeyModule.forRoot(),
		ChartsModule
	],
	entryComponents: [RowsLoadingDialogComponent],
	providers: [
		QueryStatsService
	],
	bootstrap: [AppComponent]
})
export class AppModule { }

