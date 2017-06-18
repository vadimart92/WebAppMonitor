import { Component, OnInit } from '@angular/core';

import { QueryStatsService } from './query-stats.service'

@Component({
  selector: 'app-query-stats',
  templateUrl: './query-stats.component.html',
  styleUrls: ['./query-stats.component.css']
})
export class QueryStatsComponent implements OnInit {
  public queryStats :any[];
  constructor(private _statsService: QueryStatsService) { }

  ngOnInit() {
    this.loadData();
  }
  loadData() {
    this._statsService.getStats(new Date()).then(data => this.queryStats = data);
  }
}
