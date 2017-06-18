import { NgModule }             from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
 
import { QueryStatsComponent } from '../query-stats/query-stats.component';
 
const routes: Routes = [
  { path: '', redirectTo: '/queryStats', pathMatch: 'full' },
  { path: 'queryStats', component: QueryStatsComponent }
];
 
@NgModule({
  imports: [ RouterModule.forRoot(routes) ],
  exports: [ RouterModule ]
})
export class AppRoutingModule {}