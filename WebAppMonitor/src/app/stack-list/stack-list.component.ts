import { Component, OnInit, Input } from '@angular/core';
import { ReaderQueryStack, ExecutorQueryStack, Stack } from '../entities/stack'
import {ApiDataService, QueryOptions} from '../data.service'
import { MdSnackBar, MdSnackBarConfig } from '@angular/material';

export enum PreferredStackSource {
	Executor,
	Reader
}

export class StackListComponentOptions {
	preferredStackSource: PreferredStackSource
}

@Component({
	selector: 'app-stack-list',
	templateUrl: './stack-list.component.html',
	styleUrls: ['./stack-list.component.css']
})
export class StackListComponent implements OnInit {
	@Input() queryTextId: string;
	@Input() dateId: number;
	private _options: StackListComponentOptions;
	@Input() set options(value: StackListComponentOptions){
		if (this._options === value){
			return;
		}
		this._options = value;
		this.stackSource = this._options.preferredStackSource ===PreferredStackSource.Executor ? "executor" : "reader";
		this.initStacks();
	};
	stacks: Stack[];
	gridOptions: any;
	selectedItem: any;
	stackSource: string;
	constructor(private _dataService: ApiDataService, private _snackBar: MdSnackBar) { }

	ngOnInit() {
		this.initOptions();
		this.initStacks();
	}
	async initStacks(){
		if (!this.queryTextId || !this._options){
			return;
		}
		let options = new QueryOptions();
		options.where = {
			"queryId": {"eq": this.queryTextId},
			"dateId": { "eq": this.dateId}
		};
		let stackSource = this.stackSource === "executor" ? ExecutorQueryStack : ReaderQueryStack;
		let stacks = await this._dataService.get(stackSource, options);
		this.stacks = stacks;
	}

	private initOptions() {
		this.gridOptions = {
			animateRows: true,
			enableSorting: true,
			enableFilter: true,
			rowSelection: 'single',
			rowHeight: 48,
			icons: {
				checkboxChecked: '<img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA2ZpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDoxMTQzMkY1NDIyMjhFNjExQkVGOEFCQUI5MzdBNjFEMSIgeG1wTU06RG9jdW1lbnRJRD0ieG1wLmRpZDoyMzBBQkU2ODI4MjQxMUU2QjlDRUZCNUFDREJGRTVDMCIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDoyMzBBQkU2NzI4MjQxMUU2QjlDRUZCNUFDREJGRTVDMCIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgQ1M2IChXaW5kb3dzKSI+IDx4bXBNTTpEZXJpdmVkRnJvbSBzdFJlZjppbnN0YW5jZUlEPSJ4bXAuaWlkOjE0NDMyRjU0MjIyOEU2MTFCRUY4QUJBQjkzN0E2MUQxIiBzdFJlZjpkb2N1bWVudElEPSJ4bXAuZGlkOjExNDMyRjU0MjIyOEU2MTFCRUY4QUJBQjkzN0E2MUQxIi8+IDwvcmRmOkRlc2NyaXB0aW9uPiA8L3JkZjpSREY+IDwveDp4bXBtZXRhPiA8P3hwYWNrZXQgZW5kPSJyIj8+O+zv0gAAAQ1JREFUeNpilJvw35OBgWEuEEsyEAeeA3EyI1DjMxI0wTUzkaEJBCSZiFVpJcvAsDqEgUFVCMInSqOeOAPDLG8GBjNpBoZCCyI1KggwMCzwZ2DgZWdgOPWUgaF4F5pGDxWgqT4MDPzsSB7hYWBYHMDAIMzJwHDjDQND0mYGhu9/0DT6qTEwuCszMOyIZmAwkoTYALJJjp+B4cEHBoaEjQwMn38iDAVFx38wA4gzTBgYSiwhEi++MDDI8DEwvP3OwBC0CqIZGcBtBOmefoaBIXQNA8PvfxBNf4B03AZMTVgD5xwwXcQDFX/8wcAw+RQDw5VX2AMN7lRSARM07ZEKXoA0poAYJGh6CkrkAAEGAKNeQxaS7i+xAAAAAElFTkSuQmCC"/>'
			},
			getRowClass: () => "stack-row",
			suppressRowClickSelection: true,
			columnDefs: [
				{headerName: "Stack", field: "stackTrace", filter: "text"}
			]
		}
	}

	onGridReady(params) {
		params.api.sizeColumnsToFit();
	}

	onCellClick(event:any) {
		let selected = !Boolean(event.node.selected);
		event.node.setSelected(selected);
		this.selectedItem = selected ? event.data : null;
	}
	onStackCopySuccess(){
		this._snackBar.open("Done", null, <MdSnackBarConfig>{
			duration: 1000
		});
	}
	onStackSourceChanged(newValue){
		this.stackSource = newValue;
		this.initStacks();
	}
}
