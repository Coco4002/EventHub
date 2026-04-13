import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InvitationList } from './invitation-list';

describe('InvitationList', () => {
  let component: InvitationList;
  let fixture: ComponentFixture<InvitationList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InvitationList],
    }).compileComponents();

    fixture = TestBed.createComponent(InvitationList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
