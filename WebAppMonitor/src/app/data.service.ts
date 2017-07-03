import { Injectable } from '@angular/core';
import { QueryStatInfo } from "./entities/query-stats-info"
import { EntityManager, EntityQuery, DataService, NamingConvention, EntityType } from 'breeze-client';
import * as moment from 'moment';
import * as _ from "underscore"
import {formatAsDate} from "./utils/utils"

export class StatsQueryOptions {
	constructor() {
		this.where = {};
	}
	date: Date;
	queryTextId: string;
	take: number;
	orderBy: string[];
	where: Object;
	columns: string[];
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

	onMetadataInitialized() {
		var entity = this._em.metadataStore.getEntityType("QueryStatInfo") as EntityType;
		entity.keyProperties.push(entity.dataProperties[1]);
	}

	async getStats(options: StatsQueryOptions): Promise<QueryStatInfo[]> {
		var queryOptions = {
			from: "QueryStatInfo",
			where: options.where || {},
			take: options.take,
			orderBy: options.orderBy
		}
		if (options.date) {
			queryOptions.where["date"] = { "eq": formatAsDate(options.date) };
		}
		if (options.queryTextId) {
			queryOptions.where["normalizedQueryTextId"] = { "eq": options.queryTextId };
		}
		let query = new EntityQuery(queryOptions);
		if (options.columns && options.columns.length) {
			query = query.select(options.columns);
		}
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
