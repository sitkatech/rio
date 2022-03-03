import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { WaterAccountsAddComponent } from './water-accounts-add.component';

describe('WaterAccountsAddComponent', () => {
  let component: WaterAccountsAddComponent;
  let fixture: ComponentFixture<WaterAccountsAddComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ WaterAccountsAddComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WaterAccountsAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
