import { Component, Injector } from '@angular/core';
import { MatDialog } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import { DanhMucHangMucServiceProxy, DanhMucHangMucDto, LookupTableServiceProxy, LookupTableDto } from '@shared/service-proxies/service-proxies';
import { CreateHangMucDialogComponent } from './create-hangmuc/create-hangmuc-dialog.component';
import { EditHangMucDialogComponent } from './edit-hangmuc/edit-hangmuc-dialog.component';

class PagedHangMucRequestDto extends PagedRequestDto {
    keyword: string;
    nhomDichVuId: number;
    dichVuId: number;
    isActive: boolean | null;
}

@Component({
    templateUrl: './hangMuc.component.html',
    animations: [appModuleAnimation()],
    styles: [`
        mat-form-field {
            padding: 10px;
        }
    `]
})
export class DanhMucHangMucComponent extends PagedListingComponentBase<DanhMucHangMucDto> {
    danhMucHangMuc: DanhMucHangMucDto[] = [];
    keyword = '';
    nhomDichVuId: number;
    dichVuId: number;
    isActive: boolean | null;

    danhSachNhomDichVu: LookupTableDto[] = [];
    danhSachDichVu: LookupTableDto[] = [];

    constructor(
        injector: Injector,
        private _danhMucHangMucService: DanhMucHangMucServiceProxy,
        private _lookupTableService: LookupTableServiceProxy,
        private _dialog: MatDialog
    ) {
        super(injector);

        // Lấy danh sách nhóm dịch vụ
        this._lookupTableService.getAllNhomDichVuForLookupTable().subscribe(result => {
            this.danhSachNhomDichVu = result;
        })
    }

    protected list(
        request: PagedHangMucRequestDto,
        pageNumber: number,
        finishedCallback: Function
    ): void {
        request.keyword = this.keyword;
        request.nhomDichVuId = this.nhomDichVuId;
        request.dichVuId = this.dichVuId;
        request.isActive = this.isActive;

        this._danhMucHangMucService.getAll(
            request.keyword,
            request.nhomDichVuId,
            request.dichVuId,
            request.isActive,
            request.skipCount,
            request.maxResultCount
        ).pipe(
            finalize(() => {
                finishedCallback();
            })
        ).subscribe(result => {
            this.danhMucHangMuc = result.items;
            this.showPaging(result, pageNumber);
        });
    }

    create(): void {
        this.showCreateOrEditDialog();
    }

    edit(danhMucHangMucId: number): void {
        this.showCreateOrEditDialog(danhMucHangMucId);
    }

    protected delete(danhMucHangMuc: DanhMucHangMucDto): void {
        abp.message.confirm(
            this.l('DeleteWarningMessage', danhMucHangMuc.name),
            this.l('AreYouSure'),
            (result: boolean) => {
                if (result) {
                    this._danhMucHangMucService.delete(danhMucHangMuc.id).subscribe(() => {
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
            createOrEditDialog = this._dialog.open(CreateHangMucDialogComponent);
        } else {
            createOrEditDialog = this._dialog.open(EditHangMucDialogComponent, {
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

    onChangeNhomDichVu(nhomDichVuId): void {
        this._lookupTableService.getAllDichVuForLookupTable(nhomDichVuId).subscribe(result => {
            this.danhSachDichVu = result;

            if (this.dichVuId > 0) {
                if (this.danhSachDichVu.findIndex(w => w.id == this.dichVuId) < 0)
                    this.dichVuId = undefined;
            }
        })
    }
}
