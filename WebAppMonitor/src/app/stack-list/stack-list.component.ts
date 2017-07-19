import { Component, OnInit, Input } from '@angular/core';
import { ReaderQueryStack, ExecutorQueryStack, Stack } from '../entities/stack'
import {ApiDataService, QueryOptions} from '../data.service'

@Component({
	selector: 'app-stack-list',
	templateUrl: './stack-list.component.html',
	styleUrls: ['./stack-list.component.css']
})
export class StackListComponent implements OnInit {
	@Input() queryTextId: string;
	@Input() dateId: number;
	stacks: Stack[];
	constructor(private _dataService: ApiDataService) { }
	
	ngOnInit() {
		this.initStacks();
	}
	async initStacks(){
		var options = new QueryOptions();
		options.where = {
			"queryId": {"eq": this.queryTextId},
			"dateId": { "eq": this.dateId}
		}
		var stacks = await this._dataService.get(ReaderQueryStack, options);
		this.stacks = stacks;
	}
}
