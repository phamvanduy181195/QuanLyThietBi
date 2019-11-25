import { Component, Injector, Optional, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatCheckboxChange } from '@angular/material';
import { finalize, max } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import { TramDichVuServiceProxy, TramDichVuDto, GetAllQuanHuyenInput } from '@shared/service-proxies/service-proxies';
import { LookupTableServiceProxy, LookupTableDto } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './edit-tram-dialog.component.html',
    styles: [`
        mat-form-field {
            width: 100%;
        }
        mat-checkbox {
            padding-bottom: 5px;
        }
    `]
})
export class EditTramDichVuDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    tramDichVuName = '';

    tramdichvu: TramDichVuDto = new TramDichVuDto();
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
        private _dialogRef: MatDialogRef<EditTramDichVuDialogComponent>,
        @Optional() @Inject(MAT_DIALOG_DATA) private _id: number
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.totalNhanVien = 0;

        // Lấy toàn bộ tỉnh thành đưa vào Combobox
        this._lookupTableService.getAllTinhThanhForLookupTable().subscribe(result => {
            this.danhSachTinhThanh = result;
        })

        // Lấy toàn bộ danh sách tài khoản có role là Tram Truong mà chưa được bổ nhiệm vào trạm nào
        this._lookupTableService.getAllTramTruongForLookupTable(this._id).subscribe(result => {
            this.danhSachTramTruong = result;
        });

        // Lấy thông tin trạm dịch vụ cần edit
        this._tramDichVuService.get(this._id).subscribe(result => {
            this.tramdichvu = result;
            this.tramDichVuName = result.name;
            //this.totalNhanVien = Math.max(result.userIds.length - 1, 0);

            if (this.tramdichvu.diaChiTinhThanhId > 0) {
                this.onChangeTinhThanh(this.tramdichvu.diaChiTinhThanhId);
            }
                    
            // Lấy toàn bộ nhân viên của trạm hoặc chưa add vào trạm nào
            this._lookupTableService.getAllNhanVienForLookupTable(this._id).subscribe(result => {
                this.danhSachNhanVien = result;
                this.setInitialNhanVienStatus();
            });
        });
    }

    setInitialNhanVienStatus(): void {
        this.totalNhanVien = 0;

        _.map(this.danhSachNhanVien, item => {
            this.checkedNhanVienMap[item.id] = this.isNhanVienChecked(
                item.id
            );

            if (this.checkedNhanVienMap[item.id])
            {
                this.totalNhanVien++;
            }
        });
    }

    isNhanVienChecked(userId: number): boolean {
        return _.includes(this.tramdichvu.userIds, userId);
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

        this._tramDichVuService.update(this.tramdichvu)
            .pipe(
                finalize(() => {
                    this.saving = false;
                })
            )
            .subscribe(() => {
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

            if (this.danhSachQuanHuyen.findIndex(w => w.id == this.tramdichvu.diaChiQuanHuyenId) < 0)
                this.tramdichvu.diaChiQuanHuyenId = undefined;
        });
    }
    
    close(result: any): void {
        this._dialogRef.close(result);
    }
}
