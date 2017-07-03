import * as _ from 'underscore';

import { formatAsTime } from "../utils/utils"
import { ChartAxisType } from "../common/charting"

export class QueryStatInfo  {
	constructor(json: any) {
		this.count = json.count;
		this.date = json.date;
		this.totalDuration = json.totalDuration;
		this.avgDuration = json.avgDuration;
		this.avgRowCount = json.avgRowCount;
		this.avgLogicalReads = json.avgLogicalReads;
		this.avgCPU = json.avgCPU;
		this.avgWrites = json.avgWrites;
		this.avgAdoReads = json.avgAdoReads;
		this.lockerCount = json.lockerCount;
		this.lockerTotalDuration = json.lockerTotalDuration;
		this.lockerAvgDuration = json.lockerAvgDuration;
		this.lockedCount = json.lockedCount;
		this.lockedTotalDuration = json.lockedTotalDuration;
		this.lockedAvgDuration = json.lockedAvgDuration;
		this.queryText = json.queryText;
		this.normalizedQueryTextId = json.normalizedQueryTextId;
		this.deadlocksCount = json.deadlocksCount;

		this.totalDurationStr = formatAsTime(this.totalDuration);
		this.avgDurationStr = formatAsTime(this.avgDuration);
		this.lockerTotalDurationStr = formatAsTime(this.lockerTotalDuration);
		this.lockerAvgDurationStr = formatAsTime(this.lockerAvgDuration);
		this.lockedTotalDurationStr = formatAsTime(this.lockedTotalDuration);
		this.lockedAvgDurationStr = formatAsTime(this.lockedAvgDuration);
	}
	date:Date;
	totalDuration: number;
	avgDuration: number;
	count:number;
	avgRowCount:number;
	avgLogicalReads: number;
	avgCPU:number;
	avgWrites: number;
	avgAdoReads: number;
	lockerCount: number;
	lockerTotalDuration: number;
	lockerAvgDuration: number;
	queryText: string;
	normalizedQueryTextId: string;
	lockedCount: number;
	lockedTotalDuration: number;
	lockedAvgDuration: number;
	deadlocksCount: number;

	totalDurationStr: string;
	avgDurationStr: string;
	lockerTotalDurationStr: string;
	lockerAvgDurationStr: string;
	lockedTotalDurationStr: string;
	lockedAvgDurationStr: string;

	formatedText: string;
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
					new ColumnConfig("queryText", "Text")
				]);

			//new NumberColumnConfig("avgAdoReads", "AVG ado reads").with.headerDesc("AVG ado.net reads").width(100).freeze().build(),
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
	constructor(private columnsConfig: ColumnConfig) { }
	build():ColumnConfig {
		return this.columnsConfig;
	}
	width(windth: number): ColumnConfigModifier {
		this.columnsConfig.width = windth;
		return this;
	}
	hide(val: boolean): ColumnConfigModifier {
		this.columnsConfig.hide = val;
		return this;
	}
	show(): ColumnConfigModifier {
		return this.hide(false);
	}
	headerDesc(val: string): ColumnConfigModifier {
		this.columnsConfig.headerDesc = val;
		return this;
	}
	sort(val: string): ColumnConfigModifier {
		this.columnsConfig.sort = val;
		return this;
	}
	freeze(): ColumnConfigModifier {
		this.columnsConfig.suppressSizeToFit = true;
		this.columnsConfig.suppressResize = true;
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
	colId: string
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
		return nodeA.data[this.field] - nodeB.data[this.field];
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
		return prevItems.concat(this.columns);
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
