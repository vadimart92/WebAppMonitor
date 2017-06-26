import { Injectable } from '@angular/core';
import { QueryStatInfo } from "./entities/query-stats-info"
import { EntityManager, EntityQuery, DataService, NamingConvention, FilterQueryOp } from 'breeze-client';
import * as moment from 'moment';
import * as _ from "underscore"
import {formatAsDate} from "./utils/utils"

export class StatsQueryOptions {
	constructor() {
		this.where = {};
	}
	date: Date;
	take: number;
	orderBy: string[];
	from: string;
	where: Object;
}

@Injectable()
export class ApiDataService {
	private _em: EntityManager;

	constructor() {
		NamingConvention.camelCase.setAsDefault();
		var dataService = new DataService({ serviceName: "/breeze/dataservice", hasServerMetadata: true, uriBuilderName: "json" });
		this._em = new EntityManager({ dataService: dataService });
	}

	getEntityManager(): EntityManager{
		return this._em;
	}

	async getStats(options: StatsQueryOptions): Promise<QueryStatInfo[]> {
		options.from = "QueryStatInfo";
		options.where = options.where || {};
		options.where["date"] = { "eq": formatAsDate(options.date) };
		let query = new EntityQuery(options);
		return this._em.executeQuery(query)
			.then(res => {
				return _.map(res.results, r => new QueryStatInfo(r));
			})
			.catch((error) => {
				console.log(error);
				return Promise.reject(error);
			});
  }
}
