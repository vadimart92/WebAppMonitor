import { TestBed, inject } from '@angular/core/testing';

import { QueryStatsService } from './query-stats.service';

describe('RequestStatsService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [QueryStatsService]
    });
  });

  it('should be created', inject([QueryStatsService], (service: QueryStatsService) => {
    expect(service).toBeTruthy();
  }));
});
