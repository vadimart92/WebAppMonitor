import { Component, ViewChild, OnInit, AfterViewInit } from '@angular/core';
import { MdDatepicker} from '@angular/material'
import * as moment from 'moment';
import { GridOptions } from "ag-grid/main";
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';

// Observable operators
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/debounceTime';
import 'rxjs/add/operator/distinctUntilChanged';

import { QueryStatsService } from './query-stats.service'

@Component({
  selector: 'app-query-stats',
  templateUrl: './query-stats.component.html',
  styleUrls: ['./query-stats.component.css']
})
export class QueryStatsComponent implements OnInit, AfterViewInit{
  gridOptions: GridOptions;
  columnDefs: any[];
  rowData: any[];
  public queryStats: any[];
  public currentDate: Date;
  private searchTerms = new Subject<string>();
  @ViewChild(MdDatepicker) dp: MdDatepicker<Date>;

  constructor(private _statsService: QueryStatsService) {
    this.currentDate = new Date();
   this.initGridOptions();
    this.columnDefs = [
      { headerName: "Make", field: "make", sortingOrder: ['desc', 'asc']},
      { headerName: "Model", field: "model", sortingOrder: ['desc', 'asc']},
      { headerName: "Price", field: "price", sortingOrder: ['desc', 'asc'], filter: "text"}
    ];

    this.rowData = [
      { make: "Toyota", model: "Celica", price: 35000 },
      { make: "Ford", model: "Mondeo", price: 32000 },
      { make: "Porsche", model: "Boxter", price: 72000 }
    ]
  }
  initGridOptions() {
    this.gridOptions = <GridOptions>{};
    this.gridOptions.rowHeight = 48;
    this.gridOptions.enableSorting = true;
    this.gridOptions.enableFilter = true;
    this.gridOptions.icons = {
      checkboxChecked: '<img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA2ZpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDoxMTQzMkY1NDIyMjhFNjExQkVGOEFCQUI5MzdBNjFEMSIgeG1wTU06RG9jdW1lbnRJRD0ieG1wLmRpZDoyMzBBQkU2ODI4MjQxMUU2QjlDRUZCNUFDREJGRTVDMCIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDoyMzBBQkU2NzI4MjQxMUU2QjlDRUZCNUFDREJGRTVDMCIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgQ1M2IChXaW5kb3dzKSI+IDx4bXBNTTpEZXJpdmVkRnJvbSBzdFJlZjppbnN0YW5jZUlEPSJ4bXAuaWlkOjE0NDMyRjU0MjIyOEU2MTFCRUY4QUJBQjkzN0E2MUQxIiBzdFJlZjpkb2N1bWVudElEPSJ4bXAuZGlkOjExNDMyRjU0MjIyOEU2MTFCRUY4QUJBQjkzN0E2MUQxIi8+IDwvcmRmOkRlc2NyaXB0aW9uPiA8L3JkZjpSREY+IDwveDp4bXBtZXRhPiA8P3hwYWNrZXQgZW5kPSJyIj8+O+zv0gAAAQ1JREFUeNpilJvw35OBgWEuEEsyEAeeA3EyI1DjMxI0wTUzkaEJBCSZiFVpJcvAsDqEgUFVCMInSqOeOAPDLG8GBjNpBoZCCyI1KggwMCzwZ2DgZWdgOPWUgaF4F5pGDxWgqT4MDPzsSB7hYWBYHMDAIMzJwHDjDQND0mYGhu9/0DT6qTEwuCszMOyIZmAwkoTYALJJjp+B4cEHBoaEjQwMn38iDAVFx38wA4gzTBgYSiwhEi++MDDI8DEwvP3OwBC0CqIZGcBtBOmefoaBIXQNA8PvfxBNf4B03AZMTVgD5xwwXcQDFX/8wcAw+RQDw5VX2AMN7lRSARM07ZEKXoA0poAYJGh6CkrkAAEGAKNeQxaS7i+xAAAAAElFTkSuQmCC"/>'
    };
  }
  ngOnInit() {
    this.loadData();
    this.searchTerms
      .debounceTime(300)
      .distinctUntilChanged()
      .catch(error => {
        console.log(error);
        return Observable.of<string>();
      })
      .subscribe((term) => {
        this.gridOptions.api.setQuickFilter(term);
      });
  }
  ngAfterViewInit() {
   // setTimeout(() => this.dp.open(), 100);
  }
  loadData() {
    this._statsService.getStats(new Date()).then(data => this.queryStats = data);
  }

  getIsAvalilableDate(d: Date) {
    var m = moment(d);
    return true;
  }
  dateChanged(d: Date) {
    debugger;
  }
  search(term: string) : void {
    this.searchTerms.next(term);
  }
  onGridReady(params) {
    params.api.sizeColumnsToFit();
  }
}
