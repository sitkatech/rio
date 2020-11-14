import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WaterAccountsManageComponent } from './water-accounts-manage.component';

describe('WaterAccountsManageComponent', () => {
  let component: WaterAccountsManageComponent;
  let fixture: ComponentFixture<WaterAccountsManageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WaterAccountsManageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WaterAccountsManageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
