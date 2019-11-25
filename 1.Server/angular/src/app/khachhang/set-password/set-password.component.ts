import { Component, OnInit, Optional, Injector, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs/operators';
import { KhachHangServiceProxy, SetPasswordDto } from '@shared/service-proxies/service-proxies';

@Component({
    selector: 'app-reset-password',
    templateUrl: './set-password.component.html'
})
export class SetPasswordDialogComponent extends AppComponentBase
    implements OnInit {
    public isLoading = false;
    public setPasswordDto: SetPasswordDto;
    public phoneNumber = "";

    constructor(
        injector: Injector,
        private _khachHangService: KhachHangServiceProxy,
        private _dialogRef: MatDialogRef<SetPasswordDialogComponent>,
        @Optional() @Inject(MAT_DIALOG_DATA) private data: any
    ) {
        super(injector);
    }

    ngOnInit() {
        this.isLoading = true;
        this.setPasswordDto = new SetPasswordDto();
        this.setPasswordDto.userId = this.data.userId;
        this.phoneNumber = this.data.phoneNumber;

        this.isLoading = false;
    }

    public setPassword(): void {
        this.isLoading = true;
        this._khachHangService.setPassword(this.setPasswordDto).pipe(
            finalize(() => {
                this.isLoading = false;
            })
        ).subscribe(() => {
            this.notify.success(this.l('SetPasswordSuccessfully'));
            this.close(true);
        });
    }

    close(result: any): void {
        this._dialogRef.close(result);
    }
}
