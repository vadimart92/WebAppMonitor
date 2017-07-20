import { Injectable } from '@angular/core';
import { QueryStatInfo } from "./entities/query-stats-info"
import { EntityManager, EntityQuery, DataService, NamingConvention, EntityType } from 'breeze-client';
import * as _ from "underscore"
import {formatAsDate, camelize} from "./utils/utils"
import { BaseEntity } from './entities/base-entity';
import {ExecutorQueryStack, ReaderQueryStack} from "./entities/stack"

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
		let store = this._em.metadataStore;
		let entity = store.getEntityType("QueryStatInfo") as EntityType;
		entity.keyProperties.push(entity.dataProperties[1]);
		store.registerEntityTypeCtor('QueryStatInfo', QueryStatInfo);
		store.registerEntityTypeCtor('ExecutorQueryStack', ExecutorQueryStack);
		store.registerEntityTypeCtor('ReaderQueryStack', ReaderQueryStack);
	}
	async get<TEntity extends BaseEntity>(ctor: { new (): TEntity; }, options: QueryOptions): Promise<TEntity[]> {
		let prototype = ctor.prototype;
		let entityType = prototype ? prototype.entityType : null;
		let from = entityType ? entityType.defaultResourceName : null;
		if (!from){
			return Promise.reject("cant find entity name")
		}
		let query = new EntityQuery({
			from: from,
			where: options.where || {},
			take: options.take || 100,
			orderBy: options.orderBy
		});
		return this.executeQuery(query);
	}
	initQueryResults(res: any): any {
		let list = res.results;
		_.each(list, (item)=>{
			(item as BaseEntity).onLoad();
		});
		return list;
	}
	executeQuery(query: EntityQuery) : Promise<any> {
		return this._em.executeQuery(query)
			.then(this.initQueryResults)
			.catch((error) => {
				console.log(error);
				return Promise.reject(error);
			});
	}
	async getStats(options: StatsQueryOptions): Promise<QueryStatInfo[]> {
		let queryOptions = {
			from: "QueryStatInfo",
			where: options.where || {},
			take: options.take,
			orderBy: options.orderBy
		};
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
		return this.executeQuery(query);
  }
}
