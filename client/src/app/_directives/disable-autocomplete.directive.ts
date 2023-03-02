import { Directive, AfterViewInit, ElementRef } from '@angular/core';

@Directive({
    selector: '[disableAutocomplete]'
  })
  export class DisableAutocompleteDirective implements AfterViewInit {
    constructor(private elementRef: ElementRef) {}
    
    ngAfterViewInit() {
      this.elementRef.nativeElement.setAttribute('autocomplete', 'off');
    }
  }
  