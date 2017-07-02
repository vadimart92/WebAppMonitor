
export enum ChartAxisType {
	Number,
	Time
}

export class ChartData {
	seriesData: any[];
	yAxisType: ChartAxisType;
	chartCaption: string;
	column: string;
	constructor() {
		this.seriesData = [];
		this.yAxisType = ChartAxisType.Number;
	}
}
