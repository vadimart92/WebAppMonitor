import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule, MdNativeDateModule } from '@angular/material';
import 'hammerjs';
import { AgGridModule } from "ag-grid-angular/main";
import 'moment-duration-format';
import { ClipboardModule } from 'ngx-clipboard';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing/app-routing.module';

import { QueryStatsComponent } from './query-stats/query-stats.component';
import { QueryStatsService } from './query-stats/query-stats.service';
import { QueryInfoComponent } from './query-info/query-info.component';
import { RowsLoadingDialogComponent } from './rows-loading-dialog/rows-loading-dialog.component';
import { OptionsComponent } from './options/options.component';

@NgModule({
  declarations: [
    AppComponent,
	  QueryStatsComponent,
	  QueryInfoComponent,
	  RowsLoadingDialogComponent,
	  OptionsComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    MaterialModule,
    MdNativeDateModule,
    HttpModule,
	AgGridModule.withComponents([]),
	ClipboardModule
  ],
  entryComponents: [RowsLoadingDialogComponent],
  providers: [
    QueryStatsService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

