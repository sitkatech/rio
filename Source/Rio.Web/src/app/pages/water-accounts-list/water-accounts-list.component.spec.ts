import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WaterAccountsListComponent } from './water-accounts-list.component';

describe('WaterAccountsListComponent', () => {
  let component: WaterAccountsListComponent;
  let fixture: ComponentFixture<WaterAccountsListComponent>;

  beforeEach(async(() => {
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
