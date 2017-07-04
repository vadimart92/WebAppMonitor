import { Injectable } from '@angular/core';
import * as _ from 'underscore';

@Injectable()
export class SettingsService {

	constructor() { }
	getSettingsProvider(key: string): LocalSettingsProvider {
		var currentKeys = this.getProviderKeys();
		if (currentKeys.indexOf(key) === -1) {
			currentKeys.push(key);
			this.saveProviderKeys(currentKeys);
		}
		return new LocalSettingsProvider(key);
	}
	clearAllSettings() {
		var currentKeys = this.getProviderKeys();
		_.each(currentKeys, key => localStorage.removeItem(key));
	}
	getProviderKeys():string[] {
		var value = localStorage["LocalSettingsProviderKeys"];
		if (value) {
			value = JSON.parse(value);
		} else {
			value = [];
		}
		return value;
	}
	saveProviderKeys(keys: string[]) {
		localStorage["LocalSettingsProviderKeys"] = JSON.stringify(keys);
	}
}

export class LocalSettingsProvider {
	constructor(private key: string) { }
	getVisibleColumnIds(): string[] {
		var storage = this.getSettingsObject();
		return storage.visibleColumns;
	}
	getSettingsObject():any {
		var value = localStorage[this.key];
		if (value) {
			value = JSON.parse(value);
		} else {
			value = {};
		}
		return value;
	}
	saveSettingsObject(storage: Object) {
		localStorage[this.key] = JSON.stringify(storage);
	}
	saveVisibleColumns(columnIds: string[]): void {
		var storage = this.getSettingsObject();
		storage.visibleColumns = columnIds;
		this.saveSettingsObject(storage);
	}
}