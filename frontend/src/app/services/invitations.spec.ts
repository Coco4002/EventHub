import { TestBed } from '@angular/core/testing';

import { Invitations } from './invitations';

describe('Invitations', () => {
  let service: Invitations;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Invitations);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
