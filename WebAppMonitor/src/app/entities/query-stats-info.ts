import * as _ from 'underscore';

import { formatAsTime } from "../utils/utils"
import { ChartAxisType } from "../common/charting"
import {BaseEntity} from "./base-entity"

export class QueryStatInfo extends BaseEntity {
	onLoad(){
		super.onLoad();
		this.totalDurationStr = formatAsTime(this.totalDuration);
		this.avgDurationStr = formatAsTime(this.avgDuration);
		this.lockerTotalDurationStr = formatAsTime(this.lockerTotalDuration);
		this.lockerAvgDurationStr = formatAsTime(this.lockerAvgDuration);
		this.lockedTotalDurationStr = formatAsTime(this.lockedTotalDuration);
		this.lockedAvgDurationStr = formatAsTime(this.lockedAvgDuration);
		this.totalExecutorDurationStr = formatAsTime(this.totalExecutorDuration);
		this.avgExecutorDurationStr = formatAsTime(this.avgExecutorDuration);
	}
	date:Date;
	dateId:number;
	totalDuration: number;
	avgDuration: number;
	count:number;
	avgRowCount:number;
	avgLogicalReads: number;
	avgCPU:number;
	avgWrites: number;
	lockerCount: number;
	lockerTotalDuration: number;
	lockerAvgDuration: number;
	queryText: string;
	normalizedQueryTextId: string;
	lockedCount: number;
	lockedTotalDuration: number;
	lockedAvgDuration: number;
	deadlocksCount: number;
	readerLogsCount:number;
	totalReaderLogsReads:number;
	avgReaderLogsReads:number;
	distinctReaderLogsStacks:number;
	executorLogsCount: number;
	totalExecutorDuration: number;
	avgExecutorDuration: number;
	distinctExecutorLogsStacks: number;

	totalDurationStr: string;
	avgDurationStr: string;
	lockerTotalDurationStr: string;
	lockerAvgDurationStr: string;
	lockedTotalDurationStr: string;
	lockedAvgDurationStr: string;
	totalExecutorDurationStr: string;
	avgExecutorDurationStr: string;

	formattedText: string;
}

export class QueryStatInfoDisplayConfig {
	private columns: any[] = null;
	private groupsMap: Object = null;
	getColumnsConfig(): ColumnsConfig {
		if (!this.columns) {
			var groupList = new ColumnGroup("statements", [
					new NumberColumnConfig("count", "Count").with.sort("desc").width(70).freeze().build(),
					new TimeColumnConfig("totalDuration", "Total duration").with.width(120).freeze().build(),
					new TimeColumnConfig("avgDuration", "AVG duration").with.width(110).freeze().build(),
					new NumberColumnConfig("avgRowCount", "AVG rows").with.width(90).freeze().build(),
					new NumberColumnConfig("avgCPU", "AVG CPU").with.width(100).freeze().build(),
					new NumberColumnConfig("avgLogicalReads", "AVG reads").with.width(100).freeze().build(),
					new NumberColumnConfig("avgWrites", "AVG writes").with.width(100).freeze().build(),
				]).next("readerLogs", [
					new NumberColumnConfig("readerLogsCount", "Reader logs").with.width(100).freeze().build(),
					new NumberColumnConfig("totalReaderLogsReads", "Total ado reads").with.width(100).freeze().build(),
					new NumberColumnConfig("avgReaderLogsReads", "AVG ado reads").with.width(100).freeze().build(),
					new NumberColumnConfig("distinctReaderLogsStacks", "Read stacks").with.headerDesc("Distinct read stacks").width(100).freeze().build()
				]).next("executorLogs", [
					new NumberColumnConfig("executorLogsCount", "Executor logs").with.width(100).freeze().build(),
					new TimeColumnConfig("totalExecutorDuration", "Total ado duration").with.width(100).freeze().build(),
					new TimeColumnConfig("avgExecutorDuration", "AVG ado duration").with.width(100).freeze().build(),
					new NumberColumnConfig("distinctExecutorLogsStacks", "Execute stacks").with.headerDesc("Distinct execute stacks").width(100).freeze().build()
				])
				.next("locks", [
					new NumberColumnConfig("deadlocksCount", "Deadlocks count").with.width(100).freeze().build(),
					new NumberColumnConfig("lockerCount", "Locker count").with.headerDesc("Locking other count").width(100).freeze().build(),
					new TimeColumnConfig("lockerTotalDuration", "Total as locker").with.headerDesc("Locking other total").width(120).freeze().build(),
					new TimeColumnConfig("lockerAvgDuration", "AVG as locker").with.headerDesc("AVG locking other").width(120).freeze().build(),
					new NumberColumnConfig("lockedCount", "Locked count").with.headerDesc("Locked by other count").width(100).freeze().build(),
					new TimeColumnConfig("lockedTotalDuration", "Total locked").with.headerDesc("Locked by other total").width(120).freeze().build(),
					new TimeColumnConfig("lockedAvgDuration", "AVG locked").with.headerDesc("AVG locked by other").width(120).freeze().build()
				])
				.next("text", [
					new ColumnConfig("queryText", "Text").with.hideInfoItem().cellClass("query-stats-sql-cell").build()
				]);
			this.groupsMap = groupList.toGroups();
			this.groupsMap["statements"].modifyItems(c => c.hide = false);
			this.groupsMap["text"].modifyItems(c => c.hide = false);
			this.columns = groupList.toArray();
		}
		return <ColumnsConfig>{
			columns: this.columns as ColumnConfig[],
			groupsMap: this.groupsMap
		};
	}
	getChartsConfig(): Object {
		let columns = this.getColumnsConfig().columns;
		let result = {};
		_.each(columns, (column) => {
			if (column.displayChart()) {
				result[column.colId] = column.getChartMetaData();
			}
		});
		return result;
	}
}

class ColumnConfigModifier {
	constructor(private columnConfig: ColumnConfig) { }
	build():ColumnConfig {
		return this.columnConfig;
	}
	width(windth: number): ColumnConfigModifier {
		this.columnConfig.width = windth;
		return this;
	}
	cellClass(value: string): ColumnConfigModifier {
		this.columnConfig.cellClass = value;
		return this;
	}
	hideInfoItem(): ColumnConfigModifier {
		this.columnConfig.showInfoItem = false;
		return this;
	}
	hide(val: boolean): ColumnConfigModifier {
		this.columnConfig.hide = val;
		return this;
	}
	headerDesc(val: string): ColumnConfigModifier {
		this.columnConfig.headerDesc = val;
		return this;
	}
	sort(val: string): ColumnConfigModifier {
		this.columnConfig.sort = val;
		return this;
	}
	freeze(): ColumnConfigModifier {
		this.columnConfig.suppressSizeToFit = true;
		this.columnConfig.suppressResize = true;
		return this;
	}
}

class ChartMetaData {
	label: string;
	yAxisType: ChartAxisType;
	dataColumn: string;
}

class ColumnConfig {
	constructor(colId: string, header: string) {
		this.colId = colId;
		this.headerName = header;
		this.init(this.colId);
	}
	colId: string;
	field: string;
	sortingOrder: string[];
	headerName: string;
	headerDesc: string;
	filter: string;
	width: number;
	suppressSizeToFit: boolean;
	suppressResize: boolean;
	hide: boolean = true;
	sort: string;
	columnGroup: string;
	cellClass: string;
	get with(): ColumnConfigModifier {
		return new ColumnConfigModifier(this);
	}
	init(colId: string) {
		this.field = colId;
		this.sortingOrder = ['desc', 'asc'];
	}

	displayChart(): boolean {
		return false;
	}

	getChartMetaData(): ChartMetaData{
		return <ChartMetaData>{
			dataColumn: this.colId,
			label: this.headerName
		};
	}
	showInfoItem: boolean = true;
}

class NumberColumnConfig extends ColumnConfig {
	init(colId: string) {
		super.init(colId);
		this.filter = 'number';
	}
	getChartMetaData():ChartMetaData {
		var data = super.getChartMetaData();
		data.yAxisType = ChartAxisType.Number;
		return data;
	}
	displayChart(): boolean {
		return true;
	}
}

class TimeColumnConfig extends ColumnConfig {
	init(colId: string) {
		super.init(colId + "Str");
	}
	comparator = (valueA, valueB, nodeA, nodeB) : number => {
		return nodeA.data[this.colId] - nodeB.data[this.colId];
	}

	getChartMetaData(): ChartMetaData {
		var data = super.getChartMetaData();
		data.yAxisType = ChartAxisType.Time;
		return data;
	}
	displayChart(): boolean {
		return true;
	}
}

class ColumnGroup {
	constructor(private groupName: string, private columns: ColumnConfig[], private previousGroup: ColumnGroup = null) {
		_.each(columns, c => {
			c.columnGroup = groupName;
		});
		this.columns = columns || [];
	}
	next(groupName: string, columns: ColumnConfig[]): ColumnGroup {
		return new ColumnGroup(groupName, columns, this);
	}
	toArray(): ColumnConfig[] {
		var prevItems = this.previousGroup ? this.previousGroup.toArray() : [];
		return [].concat(prevItems).concat(this.columns);
	}
	toGroups(obj: Object = null): Object {
		obj = obj || {};
		obj[this.groupName] = this;
		if (this.previousGroup) {
			this.previousGroup.toGroups(obj);
		}
		return obj;
	}
	modifyItems(func: (n: ColumnConfig) => void) {
		_.each(this.columns, func);
	}
}

class ColumnsConfig {
	columns: ColumnConfig[];
	groupsMap: Object;
}
