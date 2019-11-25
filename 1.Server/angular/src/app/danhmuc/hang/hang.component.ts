import { Component, Injector } from '@angular/core';
import { MatDialog } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import { DanhMucHangServiceProxy, DanhMucHangDto } from '@shared/service-proxies/service-proxies';
import { CreateHangDialogComponent } from './create-hang/create-hang-dialog.component';
import { EditHangDialogComponent } from './edit-hang/edit-hang-dialog.component';

class PagedHangRequestDto extends PagedRequestDto {
    keyword: string;
}

@Component({
    templateUrl: './hang.component.html',
    animations: [appModuleAnimation()],
    styles: [`
        mat-form-field {
            padding: 10px;
        }
    `]
})
export class DanhMucHangComponent extends PagedListingComponentBase<DanhMucHangDto> {
    danhMucHang: DanhMucHangDto[] = [];
    keyword = '';

    constructor(
        injector: Injector,
        private _danhMucHangService: DanhMucHangServiceProxy,
        private _dialog: MatDialog
    ) {
        super(injector);
    }

    protected list(
        request: PagedHangRequestDto,
        pageNumber: number,
        finishedCallback: Function
    ): void {
        request.keyword = this.keyword;
        this._danhMucHangService.getAll(
            request.keyword,
            request.skipCount,
            request.maxResultCount
        ).pipe(
            finalize(() => {
                finishedCallback();
            })
        ).subscribe(result => {
            this.danhMucHang = result.items;
            this.showPaging(result, pageNumber);
        });
    }

    create(): void {
        this.showCreateOrEditDialog();
    }

    edit(danhMucHangId: number): void {
        this.showCreateOrEditDialog(danhMucHangId);
    }

    protected delete(danhMucHang: DanhMucHangDto): void {
        abp.message.confirm(
            this.l('DeleteWarningMessage', danhMucHang.name),
            this.l('AreYouSure'),
            (result: boolean) => {
                if (result) {
                    this._danhMucHangService.delete(danhMucHang.id).subscribe(() => {
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
            createOrEditDialog = this._dialog.open(CreateHangDialogComponent);
        } else {
            createOrEditDialog = this._dialog.open(EditHangDialogComponent, {
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
