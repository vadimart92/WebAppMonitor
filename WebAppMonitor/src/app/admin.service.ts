import { Injectable } from '@angular/core';
import { Http } from '@angular/http'
import 'rxjs/add/operator/toPromise';

@Injectable()
export class AdminService {

	constructor(private _httpService: Http) { }
	getStatsInfo(): Promise<any> {
		return this._httpService.get("/api/Admin/getStatsInfo")
			.toPromise()
			.then(response => response.json());
	}
}
