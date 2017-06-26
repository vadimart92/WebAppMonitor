import { Injectable } from '@angular/core';import { Http } from '@angular/http'import 'rxjs/add/operator/toPromise';import { QueryStatInfo} from "../entities/query-stats-info"@Injectable()export class QueryStatsService {

  constructor(private _httpService: Http) { }  getQueryInfo(query: QueryStatInfo): Promise<any> {	  return this._httpService.get(`/api/QueryStatsData/queryInfo/${query.normalizedQueryTextId}`)      .toPromise()		  .then(response => response.json().Result);  }
  getDatesWithData():Promise<Date[]> {	  return this._httpService.get('/api/QueryStatsData/info')      .toPromise()		  .then(response => response.json().DatesWithData);  }
  formatSql(query: QueryStatInfo):Promise<string> {	  return this._httpService.post('/api/QueryStatsData/formatSQl', { queryId: query.normalizedQueryTextId})		.map(response => {			return response.json().Result;		  }).toPromise();  }
}
