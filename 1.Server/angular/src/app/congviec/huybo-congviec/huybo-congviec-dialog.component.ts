import { Component, Injector, Optional, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { AppComponentBase } from '@shared/app-component-base';
import { QuanLyCongViecServiceProxy, HuyBoCongViecInput } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
    templateUrl: './huybo-congviec-dialog.component.html',
    styles: [`
        mat-form-field {
            width: 100%;
        }
    `]
})
export class HuyBoCongViecDialogComponent extends AppComponentBase {
    saving = false;
    huyCongViec: HuyBoCongViecInput = new HuyBoCongViecInput();

    tenKhachHang = '';

    constructor(
        injector: Injector,
        public _quanLyCongViecService: QuanLyCongViecServiceProxy,
        private _dialogRef: MatDialogRef<HuyBoCongViecDialogComponent>,
        @Optional() @Inject(MAT_DIALOG_DATA) private _data: any
    ) {
        super(injector);

        this.huyCongViec.congViecId = _data.congViecId;
        this.tenKhachHang = _data.tenKhachHang;
    }

    save(): void {
        this.saving = true;

        this._quanLyCongViecService.huyBoCongViec(this.huyCongViec).pipe(
            finalize(() => {
                this.saving = false;
            })
        ).subscribe(() => {
            this.notify.info(this.l('SavedSuccessfully'));
            this.close(true);
        });
    }

    close(result: any): void {
        this._dialogRef.close(result);
    }
}
