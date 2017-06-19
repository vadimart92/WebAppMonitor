import { Component, OnInit } from '@angular/core';
import { MdSnackBar } from '@angular/material';
import { Http } from '@angular/http';

@Component({
	selector: 'app-options',
	templateUrl: './options.component.html',
	styleUrls: ['./options.component.css']
})
export class OptionsComponent implements OnInit {

	constructor(public snackBar: MdSnackBar, private  _http: Http) { }

	ngOnInit() {
	}
	clearCache() {
		this._http.post("/api/QueryStatsData/clearCache", {})
			.subscribe(() => {
				this.snackBar.open("Done", null, {
					duration: 1000,
				});
			});
	}
}
