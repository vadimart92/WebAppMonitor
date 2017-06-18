import { Injectable } from '@angular/core';import { Http } from '@angular/http'import 'rxjs/add/operator/toPromise';

@Injectable()export class QueryStatsService {

  constructor(private _httpService: Http) { }  getStats(date: Date):Promise<any> {    return this._httpService.get('/api/values')      .toPromise()      .then(response => response.json().data);  }
}
