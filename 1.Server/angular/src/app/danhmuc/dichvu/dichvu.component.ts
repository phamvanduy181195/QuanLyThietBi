import { Component, Injector } from '@angular/core';
import { MatDialog } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import { DanhMucDichVuServiceProxy, DanhMucDichVuDto, LookupTableDto, LookupTableServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateDichVuDialogComponent } from './create-dichvu/create-dichvu-dialog.component';
import { EditDichVuDialogComponent } from './edit-dichvu/edit-dichvu-dialog.component';

class PagedDichVuRequestDto extends PagedRequestDto {
    keyword: string;
    nhomDichVuId: number;
    isActive: boolean | null;
}

@Component({
    templateUrl: './dichVu.component.html',
    animations: [appModuleAnimation()],
    styles: [`
        mat-form-field {
            padding: 10px;
        }
    `]
})
export class DanhMucDichVuComponent extends PagedListingComponentBase<DanhMucDichVuDto> {
    danhMucDichVu: DanhMucDichVuDto[] = [];
    keyword = '';
    nhomDichVuId: number;
    isActive: boolean;
    danhSachNhomDichVu: LookupTableDto[] = [];

    constructor(
        injector: Injector,
        private _danhMucDichVuService: DanhMucDichVuServiceProxy,
        private _lookupTableService: LookupTableServiceProxy,
        private _dialog: MatDialog
    ) {
        super(injector);

        this._lookupTableService.getAllNhomDichVuForLookupTable().subscribe(result => {
            this.danhSachNhomDichVu = result;
        })
    }

    protected list(
        request: PagedDichVuRequestDto,
        pageNumber: number,
        finishedCallback: Function
    ): void {
        request.keyword = this.keyword;
        request.nhomDichVuId = this.nhomDichVuId;
        request.isActive = this.isActive;

        this._danhMucDichVuService.getAll(
            request.keyword,
            request.nhomDichVuId,
            request.isActive,
            request.skipCount,
            request.maxResultCount
        ).pipe(
            finalize(() => {
                finishedCallback();
            })
        ).subscribe(result => {
            this.danhMucDichVu = result.items;
            this.showPaging(result, pageNumber);
        });
    }

    create(): void {
        this.showCreateOrEditDialog();
    }

    edit(danhMucDichVuId: number): void {
        this.showCreateOrEditDialog(danhMucDichVuId);
    }

    protected delete(danhMucDichVu: DanhMucDichVuDto): void {
        abp.message.confirm(
            this.l('DeleteWarningMessage', danhMucDichVu.name),
            this.l('AreYouSure'),
            (result: boolean) => {
                if (result) {
                    this._danhMucDichVuService.delete(danhMucDichVu.id).subscribe(() => {
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
            createOrEditDialog = this._dialog.open(CreateDichVuDialogComponent);
        } else {
            createOrEditDialog = this._dialog.open(EditDichVuDialogComponent, {
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
