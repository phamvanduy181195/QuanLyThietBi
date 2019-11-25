import { Component, Injector, Optional, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { AppComponentBase } from '@shared/app-component-base';
import { QuanLyCongViecServiceProxy, PhanBoVeTramInput } from '@shared/service-proxies/service-proxies';
import { LookupTableServiceProxy, LookupTableDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
    templateUrl: './phanbo-vetram-dialog.component.html',
    styles: [
        `
      mat-form-field {
        width: 100%;
      }
    `
    ]
})
export class PhanBoVeTramDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    phanBo: PhanBoVeTramInput = new PhanBoVeTramInput();

    danhSachTramDichVu: LookupTableDto[] = [];

    constructor(
        injector: Injector,
        public _quanLyCongViecService: QuanLyCongViecServiceProxy,
        public _lookupTableService: LookupTableServiceProxy,
        private _dialogRef: MatDialogRef<PhanBoVeTramDialogComponent>,
        @Optional() @Inject(MAT_DIALOG_DATA) private _data: any
    ) {
        super(injector);

        this.phanBo.congViecIds = _data.congViecIds;
    }

    ngOnInit(): void {
        // Lấy toàn bộ trạm dịch vụ đưa vào Combobox
        this._lookupTableService.getAllTramDichVuForLookupTable().subscribe(result => {
            this.danhSachTramDichVu = result;
        });
    }

    save(): void {
        this.saving = true;

        this._quanLyCongViecService.phanBoVeTram(this.phanBo).pipe(
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
