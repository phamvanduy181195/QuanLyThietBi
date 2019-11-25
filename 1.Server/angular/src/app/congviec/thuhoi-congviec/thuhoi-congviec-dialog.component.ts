import { Component, Injector, Optional, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { AppComponentBase } from '@shared/app-component-base';
import { QuanLyCongViecServiceProxy, ThuHoiCongViecInput } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
    templateUrl: './thuhoi-congviec-dialog.component.html',
    styles: [`
        mat-form-field {
            width: 100%;
        }
    `]
})
export class ThuHoiCongViecDialogComponent extends AppComponentBase {
    saving = false;
    thuHoiCongViec: ThuHoiCongViecInput = new ThuHoiCongViecInput();

    tenNhanVien = '';

    constructor(
        injector: Injector,
        public _quanLyCongViecService: QuanLyCongViecServiceProxy,
        private _dialogRef: MatDialogRef<ThuHoiCongViecDialogComponent>,
        @Optional() @Inject(MAT_DIALOG_DATA) private _data: any
    ) {
        super(injector);

        this.thuHoiCongViec.congViecId = _data.congViecId;
        this.tenNhanVien = _data.tenNhanVien;
    }

    save(): void {
        this.saving = true;

        this._quanLyCongViecService.thuHoiCongViec(this.thuHoiCongViec).pipe(
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
