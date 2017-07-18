import { Component, OnInit, Input } from '@angular/core';

@Component({
	selector: 'app-stack-list',
	templateUrl: './stack-list.component.html',
	styleUrls: ['./stack-list.component.css']
})
export class StackListComponent implements OnInit {
	@Input() queryTextId: string;
	@Input() dateId: number;
	constructor() { }

	ngOnInit() {
	}

}
