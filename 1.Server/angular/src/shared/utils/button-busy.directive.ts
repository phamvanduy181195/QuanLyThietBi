import { Directive, ElementRef, Input, AfterViewInit, OnChanges, SimpleChanges } from '@angular/core';

@Directive({
    selector: '[buttonBusy]'
})
export class ButtonBusyDirective implements OnChanges {

    @Input('buttonBusy') isBusy: boolean;
    @Input() autoEnable: boolean = true;
    // @Input() busyText: string;

    private _$button: JQuery;
    private _$buttonInnerSpan: JQuery;
    private _$buttonIcon: JQuery;
    private _$buttonMatIcon: JQuery;

    constructor(
        private _element: ElementRef
        ) {
    }

    ngOnChanges(changes: SimpleChanges): void {
        this._$button = $(this._element.nativeElement);
        // this._$buttonInnerSpan = this._$button.find('span');
        this._$buttonIcon = this._$button.find('i');
        this._$buttonMatIcon = this._$button.find('mat-icon');

        if (!this._$button) {
            return;
        }

        if (changes['isBusy'].currentValue) {

            // disable button
            if (this.autoEnable)
                this._$button.attr('disabled', 'disabled');

            //change icon
            if (this._$buttonIcon.length) {
                this._$buttonIcon.data('_originalClasses', this._$buttonIcon.attr('class'));
                this._$buttonIcon.removeClass();
                this._$buttonIcon.addClass('fa fa-spin fa-spinner');
            }

            //change mat-icon
            if (this._$buttonMatIcon.length) {
                this._$buttonMatIcon.data('_originalClasses', this._$buttonMatIcon.attr('class'));
                this._$buttonMatIcon.data('_originalText', this._$buttonMatIcon.html());

                this._$buttonMatIcon.removeClass();
                this._$buttonMatIcon.html('');

                this._$buttonMatIcon.addClass('fa fa-spin fa-spinner');
            }

            // // change text
            // if (this.busyText && this._$buttonInnerSpan.length) {
            //     this._$buttonInnerSpan.data('_originalText', this._$buttonInnerSpan.html());
            //     this._$buttonInnerSpan.html(this.busyText);
            // }

            this._$button.data('_disabledBefore', true);
        } else {
            if (!this._$button.data('_disabledBefore')) {
                return;
            }

            // enable button
            if (this.autoEnable)
                this._$button.removeAttr('disabled');

            // restore icon
            if (this._$buttonIcon.length && this._$buttonIcon.data('_originalClasses')) {
                this._$buttonIcon.removeClass();
                this._$buttonIcon.addClass(this._$buttonIcon.data('_originalClasses'));
            }

            //change mat-icon
            if (this._$buttonMatIcon.length && this._$buttonMatIcon.data('_originalClasses')) {
                this._$buttonMatIcon.removeClass();
                this._$buttonMatIcon.addClass(this._$buttonMatIcon.data('_originalClasses'));
                this._$buttonMatIcon.html(this._$buttonMatIcon.data('_originalText'));
            }

            // // restore text
            // if (this._$buttonInnerSpan.length && this._$buttonInnerSpan.data('_originalText')) {
            //     this._$buttonInnerSpan.html(this._$buttonInnerSpan.data('_originalText'));
            // }
        }
    }
}