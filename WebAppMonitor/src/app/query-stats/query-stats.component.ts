import { Component, ViewChild, OnInit } from '@angular/core';
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
import { ApiDataService, StatsQueryOptions } from "../data.service"
import { QueryStatInfo, QueryStatInfoDisplayConfig } from "../entities/query-stats-info"
import { QueryData } from "../query-info/query-info.component"
import { SettingsService, LocalSettingsProvider } from "../settings.service"


@Component({
	selector: 'app-query-stats',
	templateUrl: './query-stats.component.html',
	styleUrls: ['./query-stats.component.css'],
	entryComponents: [RowsLoadingDialogComponent]
})

export class QueryStatsComponent implements OnInit {
	gridOptions: GridOptions;
	columnDefs: any[];
	queryStats: QueryStatInfo[];
	daysWithData: Date[];
	currentDate: Date;
	activeQuery: QueryData = null;
	columnsSettings: any;
	private _searchTerms = new Subject<string>();
	private _timeUtils = new TimeUtils();
	private _dialogRef: MdDialogRef<RowsLoadingDialogComponent>;
	private _dialogTimeout: number;
	sideNavOpened: boolean = true;
	private _queryFilter: string = null;
	private filterTextOnServer: boolean = false;
	private _useColumnsProjections: boolean = false;
	private _queryStatInfoDisplayConfigProvider = new QueryStatInfoDisplayConfig();
	visibleColumns: string[] = [];
	settingsProvider: LocalSettingsProvider;
	@ViewChild(MdDatepicker) dp: MdDatepicker<Date>;

	constructor(private _statsService: QueryStatsService, public dialog: MdDialog, private _hotkeysService: HotkeysService,
		private _dataService: ApiDataService, private settingsService: SettingsService) {
		this.settingsProvider = settingsService.getSettingsProvider("queryStatsGrid");
		this.currentDate = new Date();
		this.daysWithData = [];
		this.initGridOptions();
		this.initColumns();
		this.initHotKeys();
	}
	initColumns() {
		let columnsConfig = this._queryStatInfoDisplayConfigProvider.getColumnsConfig();
		var savedVisibleColumns = this.settingsProvider.getVisibleColumnIds();
		if (savedVisibleColumns) {
			_.each(columnsConfig.columns, column => {
				column.hide = savedVisibleColumns.indexOf(column.colId) === -1;
			});
		}
		this.columnDefs = columnsConfig.columns;
		this.actualizeVisibleColumns();
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
			suppressRowClickSelection: true,
			onSortChanged: this.onGridSortChanged.bind(this)
		};
	}
	onColumnVisibleChanged(columnConfig) {
		this.gridOptions.columnApi.setColumnVisible(columnConfig.colId, columnConfig.hide);
		columnConfig.hide = !columnConfig.hide;
		this.gridOptions.api.sizeColumnsToFit();
		this.actualizeVisibleColumns();
	}
	onGridSortChanged() {
		this.loadGridData();
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
				this._queryFilter = term ? term.toString() : null;
				if (this.filterTextOnServer) {
					this.loadGridData();
				}
				let api = this.gridOptions.api;
				api.setQuickFilter(term);
			});
	}
	async loadData() {
		var days = await this._statsService.getDatesWithData();
		this.daysWithData = days || [];
		if (days.length) {
			this.currentDate = days[days.length - 1];
		}
		this.loadGridData();
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
	getGridOptions(): StatsQueryOptions {
		var sortModel = this.gridOptions.api.getSortModel();
		var orderBy = sortModel.map(sm => `${sm.colId} ${sm.sort}`);
		let options = <StatsQueryOptions>{
			orderBy: orderBy,
			take: 100,
			date: this.currentDate,
			columns: []
		};
		if (this._useColumnsProjections) {
			_.each(this.columnDefs, column => {
				if (column.hide) {
					return;
				}
				options.columns.push(column.colId);
			});
		}
		if (this.filterTextOnServer && this._queryFilter) {
			options.where = {
				"queryText": {
					"contains": this._queryFilter
				}
			}
		}
		return options;
	}
	async loadGridData() {
		this.queryStats = [];
		this.toggleLoadMask();
		var options = this.getGridOptions();
		var stats = await this._dataService.getStats(options);
		this.toggleLoadMask();
		if (!stats.length) {
			this.gridOptions.api.showNoRowsOverlay();
		}
		this.queryStats = stats;
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
	actualizeVisibleColumns() {
		this.visibleColumns = this.columnDefs ? this.columnDefs.filter(c => !c.hide).map(c => c.colId) : [];
		this.settingsProvider.saveVisibleColumns(this.visibleColumns);
	}
	onGridSelectionChanged() {
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
