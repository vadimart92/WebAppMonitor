import {formatAsTime} from "../utils/utils"

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
		this.queryText = json.queryText;
		this.normalizedQueryTextId = json.normalizedQueryTextId;
		this.totalDurationStr = formatAsTime(this.totalDuration);
		this.avgDurationStr = formatAsTime(this.avgDuration);
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
	queryText: string;
	normalizedQueryTextId: string;

	totalDurationStr: string;
	avgDurationStr: string;
	avgDurationStr1: string;
}