import { AccountType } from "./account-type"
import { Transaction } from "./transaction"

export interface BankAccountListItem {
  id: number;
  name: string;
  currentValue: number;
  accountType: AccountType;
  active: boolean;
}

export interface BankAccount {
  id: number;
  type: AccountType;
  currentValue: number;
  name: string;
  totalTransactionsRegistered: number;
  lastTransactions: Transaction[];
  active: boolean;
}

export interface SaveBankAccountPayload {
  id: number;
  name: string;
  type: number;
  initialValue: number;
  active: boolean
}