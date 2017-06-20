import { Component } from '@angular/core';

@Component({
	selector: 'app-root',
	templateUrl: './app.component.html',
	styleUrls: ['./app.component.css']
})
export class AppComponent {
	localhost: boolean = false;
	constructor() {
		this.localhost = (window.location.host === "localhost:4200");
	}
}