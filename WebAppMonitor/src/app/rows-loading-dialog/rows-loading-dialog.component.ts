import { Component, OnInit } from '@angular/core';
import { MdDialogRef, MdProgressSpinner } from '@angular/material'

@Component({
  selector: 'app-rows-loading-dialog',
  templateUrl: './rows-loading-dialog.component.html',
  styleUrls: ['./rows-loading-dialog.component.css']
})
export class RowsLoadingDialogComponent implements OnInit {

	constructor(public dialogRef: MdDialogRef<RowsLoadingDialogComponent>) { }

  ngOnInit() {
  }

}