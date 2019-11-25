import { Component, Injector, Optional, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { AppComponentBase } from '@shared/app-component-base';
import { QuanLyCongViecServiceProxy, PhanBoVeNhanVienInput } from '@shared/service-proxies/service-proxies';
import { LookupTableServiceProxy, LookupTableDto, NhanVienLookupTableDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
    templateUrl: './phanbo-venhanvien-dialog.component.html',
    styles: [
        `
      mat-form-field {
        width: 100%;
      }
    `
    ]
})
export class PhanBoVeNhanVienDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    phanBo: PhanBoVeNhanVienInput = new PhanBoVeNhanVienInput();

    danhSachTramDichVu: LookupTableDto[] = [];
    danhSachNhanVien: NhanVienLookupTableDto[] = [];

    constructor(
        injector: Injector,
        public _quanLyCongViecService: QuanLyCongViecServiceProxy,
        public _lookupTableService: LookupTableServiceProxy,
        private _dialogRef: MatDialogRef<PhanBoVeNhanVienDialogComponent>,
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

        // Kiểm tra trạm dịch vụ mặc định
        this._quanLyCongViecService.getTramIdMacDinh().subscribe(tramDichVuId => {
            this.phanBo.tramDichVuId = tramDichVuId > 0 ? tramDichVuId : undefined;

            if (tramDichVuId > 0)
                this.onChangeTramDichVu(tramDichVuId);
        });
    }

    save(): void {
        this.saving = true;

        this._quanLyCongViecService.phanBoVeNhanVien(this.phanBo).pipe(
            finalize(() => {
                this.saving = false;
            })
        ).subscribe(() => {
            this.notify.info(this.l('SavedSuccessfully'));
            this.close(true);
        });
    }

    onChangeTramDichVu(tramDichVuId: number): void {
        // Lấy toàn bộ dịch vụ đưa vào Combobox
        this._lookupTableService.getAllNhanVienByTramForLookupTable(tramDichVuId).subscribe(result => {
            this.danhSachNhanVien = result;
        });
    }

    close(result: any): void {
        this._dialogRef.close(result);
    }
}
