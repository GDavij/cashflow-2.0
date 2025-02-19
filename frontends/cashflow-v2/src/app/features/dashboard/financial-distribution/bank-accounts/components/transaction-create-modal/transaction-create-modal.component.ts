import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-transaction-create-modal',
  imports: [],
  templateUrl: './transaction-create-modal.component.html',
  styleUrl: './transaction-create-modal.component.scss'
})
export class TransactionCreateModalComponent implements OnInit {
  form!: FormGroup;


  constructor(private readonly _fb: FormBuilder) 
  { }

  ngOnInit(): void {
    this.createForm();    
  }

  createForm() {
    this.form = this._fb.group({
      
    })
  }

}
