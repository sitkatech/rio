import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { WaterAccountsListComponent } from './water-accounts-list.component';

describe('WaterAccountsListComponent', () => {
  let component: WaterAccountsListComponent;
  let fixture: ComponentFixture<WaterAccountsListComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ WaterAccountsListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WaterAccountsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
