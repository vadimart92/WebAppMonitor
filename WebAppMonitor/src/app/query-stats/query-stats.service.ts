import { Injectable } from '@angular/core';import { Http } from '@angular/http'import 'rxjs/add/operator/toPromise';import * as moment from 'moment';

@Injectable()export class QueryStatsService {

  constructor(private _httpService: Http) { }  getStats(date: Date): Promise<any> {	  var formattedDate = moment(date).format("YYYY-MM-DD");	  return this._httpService.get(`/api/QueryStatsData/byTotalTime/${formattedDate}`)      .toPromise()      .then(response => response.json());  }  getQueryInfo(textId: Date): Promise<any> {      return this._httpService.get(`/api/QueryStatsData/queryInfo/${textId}`)      .toPromise()		  .then(response => response.json().result);  }
  getDatesWithData():Promise<Date[]> {	  return this._httpService.get('/api/QueryStatsData/info')      .toPromise()		  .then(response => response.json().datesWithData);  }
  formatSql(queryId: string):Promise<string> {	  return this._httpService.post('/api/QueryStatsData/formatSQl', { queryId: queryId})		.map(response => {			return response.json().result;		  }).toPromise();  }
}
