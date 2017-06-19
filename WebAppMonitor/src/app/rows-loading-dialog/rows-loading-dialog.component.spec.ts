import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RowsLoadingDialogComponent } from './rows-loading-dialog.component';

describe('RowsLoadingDialogComponent', () => {
  let component: RowsLoadingDialogComponent;
  let fixture: ComponentFixture<RowsLoadingDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RowsLoadingDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RowsLoadingDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should be created', () => {
    expect(component).toBeTruthy();
  });
});
