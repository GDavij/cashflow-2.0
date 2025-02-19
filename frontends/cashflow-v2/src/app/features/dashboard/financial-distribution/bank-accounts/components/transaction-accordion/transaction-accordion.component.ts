import { Component, Input } from '@angular/core';
import { Transaction } from '../../../../../../models/financial-distribution/transaction';
import { CdkAccordionModule } from '@angular/cdk/accordion';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { NgIcon, NgIconsModule, provideIcons } from '@ng-icons/core';
import { matAccessTimeRound, matArrowBackIosRound, matArrowBackRound } from '@ng-icons/material-icons/round';
import { RouterLink } from '@angular/router';
import { ButtonComponent } from '../../../../../../components/button/button.component';

@Component({
  selector: 'app-transaction-accordion',
  imports: [CdkAccordionModule, DatePipe, NgIcon, CurrencyPipe, RouterLink, ButtonComponent],
  providers: [provideIcons({ matArrowBackIosRound, matAccessTimeRound })],
  templateUrl: './transaction-accordion.component.html',
  styleUrl: './transaction-accordion.component.scss'
})
export class TransactionAccordionComponent {
  @Input({ required: true }) transactions!: Transaction[];
}
