import { Component, OnInit } from '@angular/core';
import { ButtonComponent } from '../../../../../components/button/button.component';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { BankAccount, SaveBankAccountPayload } from '../../../../../models/financial-distribution/bank-account';
import { Option } from '../../../../../components/select/select.models';
import { DividerComponent } from '../../../../../components/divider/divider.component';
import { LoaderComponent } from "../../../../../components/loader/loader.component";
import { FormFieldComponent } from "../../../../../components/form-field/form-field.component";
import { InputComponent } from "../../../../../components/input/input.component";
import { SelectComponent } from "../../../../../components/select/select.component";
import { AccountType } from '../../../../../models/financial-distribution/account-type';
import { FinancialDistributionService } from '../../../../../services/financial-distribution.service';
import { CacheService } from '../../../../../services/cache.service';
import { SelectContainerComponent } from "../../../../../components/select/select-container/select-container.component";
import { SelectOptionComponent } from "../../../../../components/select/select-option/select-option.component";
import { catchError, EMPTY, finalize, retry } from 'rxjs';
import { Dialog } from '@angular/cdk/dialog';
import { DeleteComponent } from '../../../../../components/dialogs/delete/delete.component';

@Component({
  selector: 'app-edit',
  imports: [ButtonComponent, RouterLink, FormsModule, ReactiveFormsModule, DividerComponent, LoaderComponent, FormFieldComponent, InputComponent, SelectComponent, SelectContainerComponent, SelectOptionComponent],
  templateUrl: './edit.component.html',
  styleUrl: './edit.component.scss'
})
export class EditComponent implements OnInit {
  readonly activeOptions: Option<boolean>[] = [
    {
      label: "Active",
      value: true
    },
    {
      label: "Deactive",
      value: false
    }
  ]


  fetchedBankAccount: BankAccount | null = null;
  isLoadingCurrentBankAccount: boolean = false;
  
  isDeletingBankAccount: boolean = false;
  isSavingBankAccount: boolean = false;

  form!: FormGroup;

  accountOptions: Option<number>[] = []

  constructor(private readonly _activatedRoute: ActivatedRoute,
              private readonly _router: Router,
              private readonly _fb: FormBuilder,
              private readonly _cacheService: CacheService,
              private readonly _financialDistributionService: FinancialDistributionService,
              private readonly _dialog: Dialog) {}

  ngOnInit(): void {
    this.createForm();
    this.loadAccountTypes();
    
    if (this.id) {
      this.loadBankAccount();
    }
  }

  get id(): string | null {
    return this._activatedRoute.snapshot.paramMap.get('id');
  }

  handleSubmit() {
    if (this.isSavingBankAccount || this.form.invalid) {
      return;
    }

    this.isSavingBankAccount = true;
    const { name, type, initialValue, active } = this.form.value;

    const payload: SaveBankAccountPayload = {
      id: Number(this.id),
      name,
      type,
      initialValue,
      active
    }

    this._financialDistributionService.saveBankAccount(payload).subscribe(() => {
      this._cacheService.invalidate(`get-bank-account-${this.id}`);
      this._router.navigate(['/bank-accounts']);
    });
  }

  openDeleteDialog() {
    const dialogRef = this._dialog.open<boolean>(DeleteComponent, {
      data: `Are you sure about deleting bank account ${this.form.get('name')?.value}`
    });

    dialogRef.closed.subscribe((shouldDelete) => {
      if (shouldDelete) {
        this.isDeletingBankAccount = true;
        this.form.disable();
        this._financialDistributionService.deleteBankAccount(this.fetchedBankAccount!).pipe(retry(3), catchError(httpError => {
          console.error({ httpError });
          return EMPTY;
        })).subscribe(() => {
          this._router.navigate(['/bank-accounts']);
        });
      }

      return;      
    })

  }

  resetForm() {
    if (this.id) {
      this.loadExistingBankAccountToForm(this.fetchedBankAccount!);
      return;
    }

    this.createForm();
  }

  private loadExistingBankAccountToForm(bankAccount: BankAccount) {
    this.form.patchValue({
      name: bankAccount.name,
      accountType: bankAccount.type.id,
      active: bankAccount.active
    });

  }

  private loadAccountTypes() {
    const {subject} = this._financialDistributionService.listTypes();

    subject.subscribe(accountTypes => {
      this.accountOptions = accountTypes.map(at => ({label: at.name, value: at.id}));
      
      if (this.form.get('type')?.value === null) {
        console.log({Loaded: this.accountOptions})
        this.form.patchValue({ type: this.accountOptions[0]?.value});
      }
      
    })
  }

  private loadBankAccount() {
    this.isLoadingCurrentBankAccount = true;
    const { subject } = this._cacheService.getOrResolveTo(() => this._financialDistributionService.getBankAccountById(this.id!), `get-bank-account-${this.id}`);

    subject.subscribe(bankAccount => {
      console.log(bankAccount);
      this.fetchedBankAccount = bankAccount;
      this.loadExistingBankAccountToForm(bankAccount);
      this.isLoadingCurrentBankAccount = false;
    })
  }

  private createForm() {
    this.form = this._fb.group({
      type: [null, [Validators.required]],
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(60)]],
      initialValue: [null, []],
      active: [true, [Validators.required]]
    });
  }

}
