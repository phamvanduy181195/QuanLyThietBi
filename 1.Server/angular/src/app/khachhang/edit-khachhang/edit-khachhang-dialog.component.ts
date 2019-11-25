import { Component, Injector, OnInit, Optional, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { AppComponentBase } from '@shared/app-component-base';
import { KhachHangServiceProxy, KhachHangDto, GetAllQuanHuyenInput } from '@shared/service-proxies/service-proxies';
import { LookupTableServiceProxy, LookupTableDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
    templateUrl: './edit-khachhang-dialog.component.html',
    styles: [`mat-form-field {
        width: 100%;
    }
    mat-checkbox {
        padding-bottom: 5px;
    }`]
})

export class EditKhachHangDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    khachHang: KhachHangDto = new KhachHangDto();

    danhSachTinhThanh: LookupTableDto[] = [];
    danhSachQuanHuyen: LookupTableDto[] = [];

    constructor(
        injector: Injector,
        public _khachHangService: KhachHangServiceProxy,
        public _lookupTableService: LookupTableServiceProxy,
        private _dialogRef: MatDialogRef<EditKhachHangDialogComponent>,
        @Optional() @Inject(MAT_DIALOG_DATA) private _id: number
    ) {
        super(injector);
    }

    ngOnInit(): void {
        // Lấy toàn bộ tỉnh thành đưa vào Combobox
        this._lookupTableService.getAllTinhThanhForLookupTable().subscribe(result => {
            this.danhSachTinhThanh = result;
        });

        // Lấy thông tin khách hàng cần edit
        this._khachHangService.get(this._id).subscribe(result => {
            this.khachHang = result;

            if (this.khachHang.provinceId > 0) {
                this.onChangeTinhThanh(this.khachHang.provinceId);
            }
        });
    }

    save(): void {
        this.saving = true;

        this._khachHangService.update(this.khachHang).pipe(
            finalize(() => {
                this.saving = false;
            })
        ).subscribe(() => {
            this.notify.info(this.l('SavedSuccessfully'));
            this.close(true);
        });
    }

    onChangeTinhThanh(tinhThanhId: number): void {
        // Lấy toàn bộ quận huyện đưa vào Combobox
        this._lookupTableService.getAllQuanHuyenForLookupTable(
            new GetAllQuanHuyenInput({tinhThanhIds: [tinhThanhId]})
        ).subscribe(result => {
            this.danhSachQuanHuyen = result;

            if (this.danhSachQuanHuyen.findIndex(w => w.id == this.khachHang.districtId) < 0)
                this.khachHang.districtId = undefined;
        });
    }

    close(result: any): void {
        this._dialogRef.close(result);
    }
}
