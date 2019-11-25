import { Component, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MatCheckboxChange } from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import { TramDichVuServiceProxy, CreateTramDichVuDto, GetAllQuanHuyenInput } from '@shared/service-proxies/service-proxies';
import { LookupTableServiceProxy, LookupTableDto } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './create-tram-dialog.component.html',
    styles: [`
        mat-form-field {
            width: 100%;
        }
        mat-checkbox {
            padding-bottom: 5px;
        }
    `]
})
export class CreateTramDichVuDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    tramdichvu: CreateTramDichVuDto = new CreateTramDichVuDto();
    danhSachTramTruong: LookupTableDto[] = [];
    danhSachNhanVien: LookupTableDto[] = [];
    danhSachTinhThanh: LookupTableDto[] = [];
    danhSachQuanHuyen: LookupTableDto[] = [];

    checkedNhanVienMap: { [key: number]: boolean } = {};
    totalNhanVien = 0;

    constructor(
        injector: Injector,
        public _tramDichVuService: TramDichVuServiceProxy,
        public _lookupTableService: LookupTableServiceProxy,
        private _dialogRef: MatDialogRef<CreateTramDichVuDialogComponent>
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.totalNhanVien = 0;

        // Lấy toàn bộ tỉnh thành đưa vào Combobox
        this._lookupTableService.getAllTinhThanhForLookupTable().subscribe(result => {
            this.danhSachTinhThanh = result;
        })

        // Lấy toàn bộ danh sách tài khoản có role là Trạm Trưởnng mà chưa được bổ nhiệm vào trạm nào
        this._lookupTableService.getAllTramTruongForLookupTable(undefined).subscribe(result => {
            this.danhSachTramTruong = result;
        });

        // Lấy toàn bộ nhân viên của trạm hoặc chưa add vào trạm nào
        this._lookupTableService.getAllNhanVienForLookupTable(undefined).subscribe(result => {
            this.danhSachNhanVien = result;
        });
    }

    onNhanVienChange(nhanVien: LookupTableDto, $event: MatCheckboxChange) {
        this.checkedNhanVienMap[nhanVien.id] = $event.checked;

        if ($event.checked)
            this.totalNhanVien++;
        else
            this.totalNhanVien--;
    }

    getCheckedNhanVien(): number[] {
        const nhanVienIds: number[] = [];
        _.forEach(this.checkedNhanVienMap, function (value, key) {
            if (value) {
                nhanVienIds.push(+key);
            }
        });
        return nhanVienIds;
    }

    save(): void {
        this.saving = true;

        this.tramdichvu.userIds = this.getCheckedNhanVien();

        this._tramDichVuService.create(this.tramdichvu)
            .pipe(
                finalize(() => {
                    this.saving = false;
                })
            ).subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close(true);
            });
    }

    // onChangeTinhThanh(tinhThanhId: number): void {
    onChangeTinhThanh(tinhThanhId: number): void {
        // Lấy toàn bộ quận huyện đưa vào Combobox
        this._lookupTableService.getAllQuanHuyenForLookupTable(
            new GetAllQuanHuyenInput({ tinhThanhIds: [tinhThanhId] })
        ).subscribe(result => {
            this.danhSachQuanHuyen = result;

            if (this.danhSachQuanHuyen.findIndex(w => w.id == this.tramdichvu.diaChiQuanHuyenId) < 0)
                this.tramdichvu.diaChiQuanHuyenId = undefined;
        });
    }

    close(result: any): void {
        this._dialogRef.close(result);
    }
}
