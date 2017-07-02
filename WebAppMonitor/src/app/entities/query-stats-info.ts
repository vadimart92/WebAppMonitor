import {formatAsTime} from "../utils/utils"
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

	totalDurationStr: string;
	avgDurationStr: string;
	lockerTotalDurationStr: string;
	lockerAvgDurationStr: string;
	lockedTotalDurationStr: string;
	lockedAvgDurationStr: string;

	formatedText: string;
}

export class QueryStatInfoDisplayConfig {

	timeComparator(dataPropName, valueA, valueB, nodeA, nodeB, isInverted): Number {
		return nodeA.data[dataPropName] - nodeB.data[dataPropName];
	}
	getColumnsConfig() : any[] {
		return [
			{ headerName: "Count", colId: "count", field: "count", sortingOrder: ['desc', 'asc'], filter: 'number', width: 70, suppressSizeToFit: true, suppressResize: true },
			{ headerName: "Total duration", field: "totalDurationStr", colId: "totalDuration", sortingOrder: ['desc', 'asc'], sort: "desc", width: 120, suppressSizeToFit: true, suppressResize: true, comparator: this.timeComparator.bind(this, "totalDuration") },
			{ headerName: "AVG duration", field: "avgDurationStr", colId: "avgDuration", sortingOrder: ['desc', 'asc'], width: 110, suppressSizeToFit: true, suppressResize: true, comparator: this.timeComparator.bind(this, "avgDuration") },
			{ headerName: "AVG rows", colId: "avgRowCount", field: "avgRowCount", sortingOrder: ['desc', 'asc'], filter: 'number', width: 90, suppressSizeToFit: true, suppressResize: true },
			{ headerName: "AVG CPU", colId: "avgCPU", field: "avgCPU", sortingOrder: ['desc', 'asc'], filter: 'number', width: 100, suppressSizeToFit: true, suppressResize: true },
			{ headerName: "AVG reads", colId: "avgLogicalReads", field: "avgLogicalReads", sortingOrder: ['desc', 'asc'], hide: true, filter: 'number', width: 100, suppressSizeToFit: true, suppressResize: true },
			{ headerName: "AVG writes", colId: "avgWrites", field: "avgWrites", sortingOrder: ['desc', 'asc'], hide: true, filter: 'number', width: 100, suppressSizeToFit: true, suppressResize: true },
			{ headerName: "AVG ado reads", headerDesc: "AVG ado.net reads", colId: "avgAdoReads", field: "avgAdoReads", sortingOrder: ['desc', 'asc'], hide: true, filter: 'number', width: 100, suppressSizeToFit: true, suppressResize: true },
			{ headerName: "Locker count", headerDesc: "Locking other count", colId: "lockerCount", field: "lockerCount", sortingOrder: ['desc', 'asc'], hide: true, filter: 'number', width: 100, suppressSizeToFit: true, suppressResize: true },
			{ headerName: "Total as locker", headerDesc: "Locking other total", field: "lockerTotalDurationStr", colId: "lockerTotalDuration", sortingOrder: ['desc', 'asc'], hide: true, width: 120, suppressSizeToFit: true, suppressResize: true, comparator: this.timeComparator.bind(this, "lockerTotalDuration") },
			{ headerName: "AVG as locker", headerDesc: "AVG locking other", field: "lockerAvgDurationStr", colId: "lockerAvgDuration", sortingOrder: ['desc', 'asc'], hide: true, width: 120, suppressSizeToFit: true, suppressResize: true, comparator: this.timeComparator.bind(this, "lockerTotalDuration") },
			{ headerName: "Locked count", headerDesc: "Locked by other count", colId: "lockedCount", field: "lockedCount", sortingOrder: ['desc', 'asc'], hide: true, filter: 'number', width: 100, suppressSizeToFit: true, suppressResize: true },
			{ headerName: "Total locked", headerDesc: "Locked by other total", field: "lockedTotalDurationStr", colId: "lockedTotalDuration", sortingOrder: ['desc', 'asc'], hide: true, width: 120, suppressSizeToFit: true, suppressResize: true, comparator: this.timeComparator.bind(this, "lockedTotalDuration") },
			{ headerName: "AVG locked", headerDesc: "AVG locked by other", field: "lockedAvgDurationStr", colId: "lockedAvgDuration", sortingOrder: ['desc', 'asc'], hide: true, width: 120, suppressSizeToFit: true, suppressResize: true, comparator: this.timeComparator.bind(this, "lockedTotalDuration") },
			{ headerName: "Text", colId: "queryText", field: "queryText", cellClass: 'query-stats-sql-cell' }
		];
	}

	getChartsConfig() : Object {
		return {
			"count": {
				label: "Count",
				yAxisType: ChartAxisType.Number,
				dataColumn: "count"
			},
			"totalDuration": {
				label: "Total duration",
				yAxisType: ChartAxisType.Time,
				dataColumn: "totalDuration"
			},
			"avgDuration": {
				label: "AVG duration",
				yAxisType: ChartAxisType.Time,
				dataColumn: "avgDuration"
			},
			"avgCPU": {
				label: "AVG CPU",
				yAxisType: ChartAxisType.Number,
				dataColumn: "avgCPU"
			},
			"avgRowCount": {
				label: "AVG row count",
				yAxisType: ChartAxisType.Number,
				dataColumn: "avgRowCount"
			},
			"avgLogicalReads": {
				label: "AVG logical reads",
				yAxisType: ChartAxisType.Number,
				dataColumn: "avgLogicalReads"
			},
			"avgAdoReads": {
				label: "AVG ado.net reads",
				yAxisType: ChartAxisType.Number,
				dataColumn: "avgAdoReads"
			},
			"lockerCount": {
				label: "Locks count",
				yAxisType: ChartAxisType.Number,
				dataColumn: "lockerCount"
			},
			"lockerTotalDuration": {
				label: "Total locker",
				yAxisType: ChartAxisType.Time,
				dataColumn: "lockerTotalDuration"
			},
			"lockerAvgDuration": {
				label: "AVG locker",
				yAxisType: ChartAxisType.Time,
				dataColumn: "lockerAvgDuration"
			},
			"lockedCount": {
				label: "Locked count",
				yAxisType: ChartAxisType.Number,
				dataColumn: "lockedCount"
			},
			"lockedTotalDuration": {
				label: "Total locked",
				yAxisType: ChartAxisType.Time,
				dataColumn: "lockedTotalDuration"
			},
			"lockedAvgDuration": {
				label: "AVG locked",
				yAxisType: ChartAxisType.Time,
				dataColumn: "lockedAvgDuration"
			}
		}
	}
}