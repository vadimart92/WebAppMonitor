import { BaseEntity } from './base-entity';

export class Stack extends BaseEntity {
	onLoad(): void {
		
	}
	public stackTrace: string;
	public queryId: string;
	public dateId: string;
	public stackId: string;
}
export class ReaderQueryStack extends Stack {
	
}
export class ExecutorQueryStack extends Stack {
	
}