﻿import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: "isChartVisible" })
export class IsChartVisible implements PipeTransform {
	transform(items: any[], visibleColumns: string[]): any[] {
		return items.filter(item => visibleColumns.indexOf(item.column)>-1);
	}
}
@Pipe({ name: "ifProp" })
export class IfProp implements PipeTransform {
	transform(items: any[], propName: string): any[] {
		return items.filter(item => Boolean(item[propName]));
	}
}