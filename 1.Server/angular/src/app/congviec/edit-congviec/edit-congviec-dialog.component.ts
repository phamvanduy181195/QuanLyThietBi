import { Component, Injector, Optional, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { AppComponentBase } from '@shared/app-component-base';
import { QuanLyCongViecServiceProxy, CongViecDto, KhachHangLookupTableDto, GetAllQuanHuyenInput } from '@shared/service-proxies/service-proxies';
import { LookupTableServiceProxy, LookupTableDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
    templateUrl: './edit-congviec-dialog.component.html',
    styles: [
        `
      mat-form-field {
        width: 100%;
      }
      mat-checkbox {
        padding-bottom: 5px;
      }
    `
    ]
})
export class EditCongViecDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    congViec: CongViecDto = new CongViecDto();

    nhomDichVuId: number;
    
    danhSachTramDichVu: LookupTableDto[] = [];
    danhSachNhomDichVu: LookupTableDto[] = [];
    danhSachDichVu: LookupTableDto[] = [];
    danhSachKhachHang: KhachHangLookupTableDto[] = [];
    danhSachTinhThanh: LookupTableDto[] = [];
    danhSachQuanHuyen: LookupTableDto[] = [];

    constructor(
        injector: Injector,
        public _quanLyCongViecService: QuanLyCongViecServiceProxy,
        public _lookupTableService: LookupTableServiceProxy,
        private _dialogRef: MatDialogRef<EditCongViecDialogComponent>,
        @Optional() @Inject(MAT_DIALOG_DATA) private _id: number
    ) {
        super(injector);
    }

    ngOnInit(): void {
        // Lấy toàn bộ trạm dịch vụ đưa vào Combobox
        this._lookupTableService.getAllTramDichVuForLookupTable().subscribe(result => {
            this.danhSachTramDichVu = result;
        });

        // Lấy toàn bộ nhóm dịch vụ đưa vào Combobox
        this._lookupTableService.getAllNhomDichVuForLookupTable().subscribe(result => {
            this.danhSachNhomDichVu = result;
        });

        // Lấy toàn bộ khách hàng đưa vào Combobox
        this._lookupTableService.getAllKhachHangForLookupTable().subscribe(result => {
            this.danhSachKhachHang = result;
        });

        // Lấy toàn bộ tỉnh thành đưa vào Combobox
        this._lookupTableService.getAllTinhThanhForLookupTable().subscribe(result => {
            this.danhSachTinhThanh = result;
        });

        // Lấy thông tin công việc cần edit
        this._quanLyCongViecService.get(this._id).subscribe(result => {
            this.congViec = result;

            if (this.congViec.diaChiTinhThanhId > 0) {
                this.onChangeTinhThanh(this.congViec.diaChiTinhThanhId);
            }
        });
    }

    save(): void {
        this.saving = true;

        this._quanLyCongViecService.update(this.congViec).pipe(
            finalize(() => {
                this.saving = false;
            })
        ).subscribe(() => {
            this.notify.info(this.l('SavedSuccessfully'));
            this.close(true);
        });
    }

    onChangeNhomDichVu(nhomDichVuId: number): void {
        // Lấy toàn bộ dịch vụ đưa vào Combobox
        this._lookupTableService.getAllDichVuForLookupTable(nhomDichVuId).subscribe(result => {
            this.danhSachDichVu = result;

            if (this.danhSachDichVu.findIndex(w => w.id == this.congViec.dichVuId) < 0) {
                this.congViec.dichVuId = undefined;
            }
        });
    }

    onChangeTinhThanh(tinhThanhId: number): void {
        // Lấy toàn bộ quận huyện đưa vào Combobox
        this._lookupTableService.getAllQuanHuyenForLookupTable(
            new GetAllQuanHuyenInput({tinhThanhIds: [tinhThanhId]})
        ).subscribe(result => {
            this.danhSachQuanHuyen = result;

            if (this.danhSachQuanHuyen.findIndex(w => w.id == this.congViec.diaChiQuanHuyenId) < 0)
                this.congViec.diaChiQuanHuyenId = undefined;
        });
    }

    onChangeKhachHang(khachHangId: number): void {
        // Update thông tin khách hàng
        let khachHang = this.danhSachKhachHang.find(w => w.id == khachHangId);
        if (khachHang) {
            this.congViec.khachHangName = khachHang.name;
            this.congViec.soDienThoai = khachHang.phoneNumber;
            this.congViec.diaChi = khachHang.address;
            this.congViec.diaChiTinhThanhId = khachHang.provinceId;
            this.congViec.diaChiQuanHuyenId = khachHang.districtId;
        }
    }

    close(result: any): void {
        this._dialogRef.close(result);
    }
}
