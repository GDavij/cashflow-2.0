import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { twMerge } from 'tailwind-merge';
import { provideNgxMask, NgxMaskDirective } from 'ngx-mask';


@Component({
  selector: 'app-input',
  imports: [NgxMaskDirective],
  templateUrl: './input.component.html',
  styleUrl: './input.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputComponent),
      multi: true
    },
    provideNgxMask()
  ]
})
export class InputComponent implements ControlValueAccessor {
  mergeClasses = twMerge;

  @Input() placeholder: string = '';
  @Input() className: string = '';
  @Input() disabled: boolean = false;
  @Input() type: 'text' | 'number' = 'text';
  @Input() mask: string | null = null;

  value: any;

  writeValue(value: any): void {
    this.value = value;
  }

  registerOnChange(fn: (value: any) => any): void {
    this.onChange = fn;
  }
  
  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  //#region <<Methods to Bind>>
  onChange(value: any): void { };

  onTouched(): void { }

  //#endregion

  handleInputChange(event: Event): void {
    const input = event.target as HTMLInputElement;

    this.onChange(input.value);
    this.writeValue(input.value);
  }

  handleBlur(): void {
    this.onTouched();
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  } 

}
