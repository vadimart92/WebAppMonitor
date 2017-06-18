import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { QueryStatsComponent } from './query-stats.component';

describe('QueryStatsComponent', () => {
  let component: QueryStatsComponent;
  let fixture: ComponentFixture<QueryStatsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ QueryStatsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(QueryStatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should be created', () => {
    expect(component).toBeTruthy();
  });
});
