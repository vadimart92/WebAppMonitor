import { Injectable } from '@angular/core';
import { QueryStatInfo } from "./entities/query-stats-info"
import { EntityManager, EntityQuery, DataService, NamingConvention, EntityType } from 'breeze-client';
import * as moment from 'moment';
import * as _ from "underscore"
import {formatAsDate, camelize} from "./utils/utils"

import { BaseEntity } from './entities/base-entity';

export class QueryOptions{
	constructor() {
		this.where = {};
	}
	take: number;
	orderBy: string[];
	where: Object;
}

export class StatsQueryOptions extends QueryOptions{
	date: Date;
	queryTextId: string;
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
	async get<TEntity extends BaseEntity>(ctor: { new (): TEntity; }, options: QueryOptions): Promise<TEntity[]> {
		let query = new EntityQuery({
			from: ctor.name,
			where: options.where || {},
			take: options.take || 100,
			orderBy: options.orderBy
		});
		return this._em.executeQuery(query)
			.then(res => {
				var keyMap = {};
				var getKey = (key) => {
					if (!keyMap[key]) {
						keyMap[key] = camelize(key);
					}
					return keyMap[key];
				}
				return _.map(res.results, r => {
					var item = new ctor();
					_.each(r, (value, key) => {
						item[getKey(key)] = value;
					});
					return item;
				});
			})
			.catch((error) => {
				console.log(error);
				return Promise.reject(error);
			});
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
				return _.map(res.results, r => {
					return new QueryStatInfo(r);
				});
			})
			.catch((error) => {
				console.log(error);
				return Promise.reject(error);
			});
  }
}
