export interface Transaction {
  id: number;
  description: string;
  doneAt: string;
  value: number;
  transactionMethod: string;
  category: Category;
}