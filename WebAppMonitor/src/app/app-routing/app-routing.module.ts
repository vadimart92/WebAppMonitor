import { NgModule }             from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
 
import { QueryStatsComponent } from '../query-stats/query-stats.component';
import { OptionsComponent } from '../options/options.component';
import { QueryInfoComponent } from '../query-info/query-info.component';
 
const routes: Routes = [
  { path: '', redirectTo: '/queryStats', pathMatch: 'full' },
  { path: 'queryStats', component: QueryStatsComponent },
  { path: 'queryInfo', component: QueryInfoComponent },
  { path: 'options', component: OptionsComponent }
];
 
@NgModule({
  imports: [ RouterModule.forRoot(routes) ],
  exports: [ RouterModule ]
})
export class AppRoutingModule {}