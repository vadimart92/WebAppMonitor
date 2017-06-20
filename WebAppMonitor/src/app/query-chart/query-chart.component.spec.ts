import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { QueryChartComponent } from './query-chart.component';

describe('QueryChartComponent', () => {
  let component: QueryChartComponent;
  let fixture: ComponentFixture<QueryChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ QueryChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(QueryChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should be created', () => {
    expect(component).toBeTruthy();
  });
});
