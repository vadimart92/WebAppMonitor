import { BaseEntity } from './base-entity';

export class QueryStatInfo extends BaseEntity {
	onLoad(): void {
		
	}
	public stackTrace: string;
	public normalizedQueryTextId: string;
}