import * as _ from 'underscore';
import * as moment from 'moment';
import 'moment-duration-format';

export class TimeUtils {
	compareTime(timeA:string, timeB:string, inverted:boolean) {
		var a = this.toSeconds(timeA);
		var b = this.toSeconds(timeB);
		if (!a && !b) {
			return 0;
		}
		if (!a) {
			return -1;
		}
		if (!b) {
			return 1;
		}
		return (a - b) * (inverted ? 1 : -1);
	}
	toSeconds(timeString) {
		return timeString.split(":").reverse()
			.map((number, order) => parseInt(number) * Math.pow(60, order))
			.reduce((a, b) => a + b);
	}
	public formatAsTime(seconds: Number): string {
		return moment.duration(seconds, "seconds").format("h:mm:ss", { trim: false });
	}
	public formatAsDate(date: Date): string {
		return moment(date).format("YYYY-MM-DD");
	}
}