import { TestBed, inject } from '@angular/core/testing';

import { ApiDataService } from './data.service';

describe('DataService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
		providers: [ApiDataService]
    });
  });

  it('should be created', inject([ApiDataService], (service: ApiDataService) => {
    expect(service).toBeTruthy();
  }));
});
