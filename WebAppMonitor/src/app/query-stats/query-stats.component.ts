﻿import { Component, ViewChild, OnInit } from '@angular/core';
import { MdDatepicker, MdDialog, MdDialogRef } from '@angular/material'
import * as moment from 'moment';
import { GridOptions } from "ag-grid/main";
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';

// Observable operators
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/debounceTime';
import 'rxjs/add/operator/distinctUntilChanged';
import * as _ from 'underscore';
import { HotkeysService, Hotkey } from 'angular2-hotkeys';

import { RowsLoadingDialogComponent } from '../rows-loading-dialog/rows-loading-dialog.component';
import { QueryStatsService } from './query-stats.service'
import { TimeUtils } from '../utils/utils'

@Component({
	selector: 'app-query-stats',
	templateUrl: './query-stats.component.html',
	styleUrls: ['./query-stats.component.css'],
	entryComponents: [RowsLoadingDialogComponent]
})

export class QueryStatsComponent implements OnInit {
	gridOptions: GridOptions;
	columnDefs: any[];
	queryStats: any[];
	daysWithData: Date[];
	currentDate: Date;
	activeQuery: any = null;
	private _searchTerms = new Subject<string>();
	private _timeUtils = new TimeUtils();
	private _dialogRef: MdDialogRef<RowsLoadingDialogComponent>;
	private _dialogTimeout: number;
	sideNavOpened: boolean = true;
	@ViewChild(MdDatepicker) dp: MdDatepicker<Date>;

	constructor(private _statsService: QueryStatsService, public dialog: MdDialog, private _hotkeysService: HotkeysService) {
		this.currentDate = new Date();
		this.daysWithData = [];
		this.initGridOptions();
		this.columnDefs = this.getColumns();
		this.initHotKeys();
	}
	initHotKeys() {
		var closeHotKey = this._hotkeysService.add(new Hotkey('alt+n', (event: KeyboardEvent): boolean => {
			this._hotkeysService.remove(closeHotKey);
			this.sideNavOpened = !this.sideNavOpened;
			return false;
		}));
	}
	initGridOptions() {
		this.gridOptions = <GridOptions>{
			animateRows: true,
			enableSorting: true,
			enableFilter: true,
			enableColResize: true,
			rowSelection: 'single',
			rowHeight: 48,
			icons: {
				checkboxChecked: '<img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA2ZpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDoxMTQzMkY1NDIyMjhFNjExQkVGOEFCQUI5MzdBNjFEMSIgeG1wTU06RG9jdW1lbnRJRD0ieG1wLmRpZDoyMzBBQkU2ODI4MjQxMUU2QjlDRUZCNUFDREJGRTVDMCIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDoyMzBBQkU2NzI4MjQxMUU2QjlDRUZCNUFDREJGRTVDMCIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgQ1M2IChXaW5kb3dzKSI+IDx4bXBNTTpEZXJpdmVkRnJvbSBzdFJlZjppbnN0YW5jZUlEPSJ4bXAuaWlkOjE0NDMyRjU0MjIyOEU2MTFCRUY4QUJBQjkzN0E2MUQxIiBzdFJlZjpkb2N1bWVudElEPSJ4bXAuZGlkOjExNDMyRjU0MjIyOEU2MTFCRUY4QUJBQjkzN0E2MUQxIi8+IDwvcmRmOkRlc2NyaXB0aW9uPiA8L3JkZjpSREY+IDwveDp4bXBtZXRhPiA8P3hwYWNrZXQgZW5kPSJyIj8+O+zv0gAAAQ1JREFUeNpilJvw35OBgWEuEEsyEAeeA3EyI1DjMxI0wTUzkaEJBCSZiFVpJcvAsDqEgUFVCMInSqOeOAPDLG8GBjNpBoZCCyI1KggwMCzwZ2DgZWdgOPWUgaF4F5pGDxWgqT4MDPzsSB7hYWBYHMDAIMzJwHDjDQND0mYGhu9/0DT6qTEwuCszMOyIZmAwkoTYALJJjp+B4cEHBoaEjQwMn38iDAVFx38wA4gzTBgYSiwhEi++MDDI8DEwvP3OwBC0CqIZGcBtBOmefoaBIXQNA8PvfxBNf4B03AZMTVgD5xwwXcQDFX/8wcAw+RQDw5VX2AMN7lRSARM07ZEKXoA0poAYJGh6CkrkAAEGAKNeQxaS7i+xAAAAAElFTkSuQmCC"/>'	
			},
			onSelectionChanged: this.onGridSelectionChanged.bind(this),
			getRowClass: () => "query-stats-row",
			suppressRowClickSelection: true
		};
	}
	timeComparator(dataPropName, valueA, valueB, nodeA, nodeB, isInverted): Number {
		return nodeA.data[dataPropName] - nodeB.data[dataPropName];
	}
	ngOnInit() {
		this.loadData();
		this._searchTerms
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
	loadData() {
		this._statsService.getDatesWithData()
			.then(days => {
				this.daysWithData = days || [];
				if (days.length) {
					this.currentDate = days[days.length - 1];
				}
			})
			.then(_ => this.loadGridData());
	}
	getColumns() : any[] {
		return [
			{ headerName: "Count", field: "count", sortingOrder: ['desc', 'asc'], width: 70, suppressSizeToFit: true, suppressResize: true },
			{ headerName: "Total duration", field: "totalDurationStr", sortingOrder: ['desc', 'asc'], width: 120, suppressSizeToFit: true, suppressResize: true, comparator: this.timeComparator.bind(this, "totalDuration") },
			{ headerName: "AVG duration", field: "averageDurationStr", sortingOrder: ['desc', 'asc'], width: 110, suppressSizeToFit: true, suppressResize: true, comparator: this.timeComparator.bind(this, "averageDuration") },
			{ headerName: "AVG rows", field: "averageRowCount", sortingOrder: ['desc', 'asc'], width: 90, suppressSizeToFit: true, suppressResize: true },
			{ headerName: "AVG CPU", field: "averageCPU", sortingOrder: ['desc', 'asc'], width: 100, suppressSizeToFit: true, suppressResize: true },
			{ headerName: "Text", field: "queryText",cellClass: 'query-stats-sql-cell'}
		];
	}
	toggleLoadMask() {
		if (!this._dialogRef && !this._dialogTimeout) {
			this._dialogTimeout = window.setTimeout(() => this._dialogRef = this.dialog.open(RowsLoadingDialogComponent), 500);
		} else {
			window.clearTimeout(this._dialogTimeout);
			this._dialogTimeout = null;
			this.dialog.closeAll();
			this._dialogRef = null;
		}
	}
	loadGridData() {
		this.toggleLoadMask();
		this._statsService.getStats(this.currentDate).then(data => {
			var stats = data.queries;
			_.each(stats, (statRow:any) => {
				 statRow.totalDurationStr = this._timeUtils.formatAsTime(statRow.totalDuration);
				 statRow.averageDurationStr = this._timeUtils.formatAsTime(statRow.averageDuration);
			});
			this.toggleLoadMask();
			if (!stats.length) {
				this.gridOptions.api.showNoRowsOverlay();
			}
			this.queryStats = stats;
		});
	}
	getIsAvalilableDate(d: Date) {
		var day = moment(d);
		var foundDays = this.daysWithData.filter(dayWithData => day.isSame(dayWithData, "day"));
		return Boolean(foundDays.length);
	}
	public onSelectedDateChanged(d: Date) {
		this.currentDate = d;
		this.activeQuery = null;
		this.loadGridData();
	}
	public changeDate(offset: Number): void {
		let date = moment(this.currentDate).add(offset, "days").toDate();
		this.onSelectedDateChanged(date);
	}

	search(term: string): void {
		this._searchTerms.next(term);
	}
	onGridReady(params) {
		params.api.sizeColumnsToFit();
	}
	onGridSelectionChanged() {
		//if (1 === 1) return;
		var selectedRows = this.gridOptions.api.getSelectedRows();
		if (selectedRows.length === 1) {
			this.activeQuery = {
				info: selectedRows[0],
				date: this.currentDate
			};
		} else {
			this.activeQuery = null;
		}
		
	}
	deselectRows() {
		this.gridOptions.api.deselectAll();
	}
	onCellClick(event:any) {
		if (event.colDef.field !== "queryText") {
			event.node.setSelected(true);
		}
	}
}
