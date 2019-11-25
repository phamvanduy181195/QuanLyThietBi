import { Component, Injector } from '@angular/core';
import { MatDialog, MatCheckboxChange, MatDialogConfig } from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import { QuanLyCongViecServiceProxy, CongViecDto, EntityDtoOfInt64 } from '@shared/service-proxies/service-proxies';
import { LookupTableServiceProxy, LookupTableDto } from '@shared/service-proxies/service-proxies';
import { CreateCongViecDialogComponent } from './create-congviec/create-congviec-dialog.component';
import { EditCongViecDialogComponent } from './edit-congviec/edit-congviec-dialog.component';
import { PhanBoVeTramDialogComponent } from './phanbo-congviec/phanbo-vetram-dialog.component';
import { PhanBoVeNhanVienDialogComponent } from './phanbo-congviec/phanbo-venhanvien-dialog.component';
import { HuyBoCongViecDialogComponent } from './huybo-congviec/huybo-congviec-dialog.component';
import { ThuHoiCongViecDialogComponent } from './thuhoi-congviec/thuhoi-congviec-dialog.component';
import { ImportCongViecDialogComponent } from './import-congviec/import-congviec-dialog.component';

class PagedCongViecRequestDto extends PagedRequestDto {
    keyword: string;
    tramDichVuId: number;
    trangThaiId: number;
}

@Component({
    templateUrl: './congviec.component.html',
    animations: [appModuleAnimation()],
    styles: [`
        mat-form-field {
            padding: 10px;
        }
    `]
})
export class CongViecComponent extends PagedListingComponentBase<CongViecDto> {
    congViecs: CongViecDto[] = [];
    keyword = '';
    trangThaiId: number;
    tramDichVuId: number;
    danhSachTramDichVu: LookupTableDto[] = [];
    danhSachTrangThai: LookupTableDto[] = [];

    totalCheckedCongViec = 0;
    checkedCongViecMap: { [key: string]: boolean } = {};

    constructor(
        injector: Injector,
        private _quanLyCongViecService: QuanLyCongViecServiceProxy,
        private _lookupTableService: LookupTableServiceProxy,
        private _dialog: MatDialog
    ) {
        super(injector);

        this._lookupTableService.getAllTramDichVuForLookupTable().subscribe(result => {
            this.danhSachTramDichVu = result;
        });
        this._lookupTableService.getAllTrangThaiCongViecForLookupTable().subscribe(result => {
            this.danhSachTrangThai = result;
        });
    }

    getCheckedCongViec(): number[] {
        const congViec: number[] = [];
        _.forEach(this.checkedCongViecMap, function (value, key) {
            if (value) {
                congViec.push(+key);
            }
        });
        return congViec;
    }

    onCongViecChange(congViec: CongViecDto, $event: MatCheckboxChange) {
        this.checkedCongViecMap[congViec.id] = $event.checked;

        if ($event.checked)
            this.totalCheckedCongViec++;
        else
            this.totalCheckedCongViec--;
    }

    phanBoVeTram() {
        let showPhanBoVeTramDialog = this._dialog.open(PhanBoVeTramDialogComponent, {
            data: {
                tramDichVuId: undefined,
                congViecIds: this.getCheckedCongViec()
            },
            disableClose: true
        });

        showPhanBoVeTramDialog.afterClosed().subscribe(result => {
            if (result) {
                // Reset selected
                this.checkedCongViecMap = {};
                this.totalCheckedCongViec = 0;

                this.refresh();
            }
        });
    }

    phanBoNhanVien() {
        let showPhanBoVeNhanVienDialog = this._dialog.open(PhanBoVeNhanVienDialogComponent, {
            data: {
                tramDichVuId: undefined,
                congViecIds: this.getCheckedCongViec()
            },
            disableClose: true
        });

        showPhanBoVeNhanVienDialog.afterClosed().subscribe(result => {
            if (result) {
                // Reset selected
                this.checkedCongViecMap = {};

                this.refresh();
            }
        });
    }

    xacNhanLinhKien(congViec: CongViecDto) {
        abp.message.confirm(
            this.l('ConfirmLinhKienWarningMessage'),
            this.l('AreYouSure'),
            (result: boolean) => {
                if (result) {
                    this._quanLyCongViecService.xacNhanLinhKien(new EntityDtoOfInt64({ 'id': congViec.id })).subscribe(() => {
                        abp.notify.success(this.l('ConfirmedSuccessfully'));
                        this.refresh();
                    });
                }
            }
        );
    }

    xacNhanHoanThanh(congViec: CongViecDto) {
        abp.message.confirm(
            this.l('ConfirmHoanThanhWarningMessage'),
            this.l('AreYouSure'),
            (result: boolean) => {
                if (result) {
                    this._quanLyCongViecService.xacNhanHoanThanh(new EntityDtoOfInt64({ 'id': congViec.id })).subscribe(() => {
                        abp.notify.success(this.l('ConfirmedSuccessfully'));
                        this.refresh();
                    });
                }
            }
        );
    }

    huyCongViec(congViec: CongViecDto) {
        let showHuyBoCongViecDialog = this._dialog.open(HuyBoCongViecDialogComponent, {
            data: {
                congViecId: congViec.id,
                tenKhachHang: congViec.khachHangName
            },
            disableClose: true
        });

        showHuyBoCongViecDialog.afterClosed().subscribe(result => {
            if (result) {
                this.refresh();
            }
        });
    }

    thuHoiCongViec(congViec: CongViecDto) {
        let showThuHoiCongViecDialog = this._dialog.open(ThuHoiCongViecDialogComponent, {
            data: {
                congViecId: congViec.id,
                tenNhanVien: congViec.nhanVienName
            },
            disableClose: true
        });

        showThuHoiCongViecDialog.afterClosed().subscribe(result => {
            if (result) {
                this.refresh();
            }
        });
    }

    createCongViec(): void {
        this.showCreateOrEditDialog();
    }

    editCongViec(tramDichVuId: number): void {
        this.showCreateOrEditDialog(tramDichVuId);
    }

    importCongViec(): void {
        let dialogConfig = new MatDialogConfig();
        dialogConfig.disableClose = false;

        let importDialog = this._dialog.open(ImportCongViecDialogComponent, dialogConfig);

        importDialog.afterClosed().subscribe(result => {
            if (result) {
                this.refresh();
            }
        });
    }

    protected list(
        request: PagedCongViecRequestDto,
        pageNumber: number,
        finishedCallback: Function
    ): void {
        request.keyword = this.keyword;
        request.tramDichVuId = this.tramDichVuId;
        request.trangThaiId = this.trangThaiId;

        this._quanLyCongViecService.getAll(
            request.keyword,
            request.tramDichVuId,
            request.trangThaiId,
            request.skipCount,
            request.maxResultCount
        ).pipe(
            finalize(() => {
                finishedCallback();
            })
        ).subscribe(result => {
            this.congViecs = result.items;
            this.showPaging(result, pageNumber);
        });
    }

    protected delete(congViec: CongViecDto): void {
        abp.message.confirm(
            this.l('DeleteWarningMessage', congViec.tieuDe),
            this.l('AreYouSure'),
            (result: boolean) => {
                if (result) {
                    this._quanLyCongViecService.delete(congViec.id).subscribe(() => {
                        abp.notify.success(this.l('DeletedSuccessfully'));
                        this.refresh();
                    });
                }
            }
        );
    }

    private showCreateOrEditDialog(id?: number): void {
        let createOrEditDialog;
        if (id === undefined || id <= 0) {
            createOrEditDialog = this._dialog.open(CreateCongViecDialogComponent);
        } else {
            createOrEditDialog = this._dialog.open(EditCongViecDialogComponent, {
                data: id,
                disableClose: true
            });
        }

        createOrEditDialog.afterClosed().subscribe(result => {
            if (result) {
                this.refresh();
            }
        });
    }
}
