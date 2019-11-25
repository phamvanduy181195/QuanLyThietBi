import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { AppComponentBase } from '@shared/app-component-base';
import { QuanLyCongViecServiceProxy, CreateCongViecDto, KhachHangLookupTableDto, GetAllQuanHuyenInput } from '@shared/service-proxies/service-proxies';
import { LookupTableServiceProxy, LookupTableDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { DinhkemComponent } from '../DinhKem/dinhkem.component';

@Component({
    templateUrl: './create-congviec-dialog.component.html',
    styles: [`mat-form-field {
        width: 100%;
    }
    mat-checkbox {
        padding-bottom: 5px;
    }`]
})

export class CreateCongViecDialogComponent extends AppComponentBase implements OnInit {
    
    @ViewChild('uploadFile') uploadFile: DinhkemComponent;

    
    saving = false;
    congViec: CreateCongViecDto = new CreateCongViecDto();

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
        private _dialogRef: MatDialogRef<CreateCongViecDialogComponent>
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
    }

    save(): void {
        this.saving = true;

        this._quanLyCongViecService.create(this.congViec).pipe(
            finalize(() => {
                this.saving = false;
            })
        ).subscribe(() => {
            this.notify.info(this.l('SavedSuccessfully'));
            this.close(true);
        });
        this.uploadFile.onBeforeUpload(3);
    }

    onChangeNhomDichVu(nhomDichVuId: number): void {
        // Lấy toàn bộ dịch vụ đưa vào Combobox
        this._lookupTableService.getAllDichVuForLookupTable(nhomDichVuId).subscribe(result => {
            this.danhSachDichVu = result;

            if (this.danhSachDichVu.findIndex(w => w.id == this.congViec.dichVuId) < 0) {
                console.log(this.congViec.dichVuId);
                this.congViec.dichVuId = undefined;
            } else {
                console.log(this.congViec.dichVuId);
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