import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TransactionCreateModalComponent } from './transaction-create-modal.component';

describe('TransactionCreateModalComponent', () => {
  let component: TransactionCreateModalComponent;
  let fixture: ComponentFixture<TransactionCreateModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TransactionCreateModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TransactionCreateModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
