import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from '@angular/material';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing/app-routing.module';

import { QueryStatsComponent } from './query-stats/query-stats.component';
import { QueryStatsService } from './query-stats/query-stats.service';

@NgModule({
  declarations: [
    AppComponent,
    QueryStatsComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    MaterialModule,
	  HttpModule
  ],
  providers: [
    QueryStatsService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

