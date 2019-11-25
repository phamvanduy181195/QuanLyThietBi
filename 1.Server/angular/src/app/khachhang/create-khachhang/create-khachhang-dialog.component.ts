import { Component, Injector, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import { KhachHangServiceProxy, CreateKhachHangDto, GetAllQuanHuyenInput } from '@shared/service-proxies/service-proxies';
import { LookupTableServiceProxy, LookupTableDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
    templateUrl: './create-khachhang-dialog.component.html',
    styles: [`mat-form-field {
        width: 100%;
    }
    mat-checkbox {
        padding-bottom: 5px;
    }`]
})

export class CreateKhachHangDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    khachHang: CreateKhachHangDto = new CreateKhachHangDto();

    danhSachTinhThanh: LookupTableDto[] = [];
    danhSachQuanHuyen: LookupTableDto[] = [];

    constructor(
        injector: Injector,
        public _khachHangService: KhachHangServiceProxy,
        public _lookupTableService: LookupTableServiceProxy,
        private _dialogRef: MatDialogRef<CreateKhachHangDialogComponent>
    ) {
        super(injector);
    }

    ngOnInit(): void {
        // Lấy toàn bộ tỉnh thành đưa vào Combobox
        this._lookupTableService.getAllTinhThanhForLookupTable().subscribe(result => {
            this.danhSachTinhThanh = result;
        });
    }

    save(): void {
        this.saving = true;

        this._khachHangService.create(this.khachHang).pipe(
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
