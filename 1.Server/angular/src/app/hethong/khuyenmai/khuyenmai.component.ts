import { Component, Injector } from '@angular/core';
import { MatDialog } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import { QuanLyKhuyenMaiServiceProxy, TinTucDto } from '@shared/service-proxies/service-proxies';
import { CreateKhuyenMaiDialogComponent } from './create-khuyenmai/create-khuyenmai-dialog.component';
import { EditKhuyenMaiDialogComponent } from './edit-khuyenmai/edit-khuyenmai-dialog.component';

class PagedNhomDichVuRequestDto extends PagedRequestDto {
    keyword: string;
}

@Component({
    templateUrl: './khuyenmai.component.html',
    animations: [appModuleAnimation()],
    styles: [`
        mat-form-field {
            padding: 10px;
        }
    `]
})
export class KhuyenMaiComponent extends PagedListingComponentBase<TinTucDto> {
    khuyenMai: TinTucDto[] = [];
    keyword = '';

    constructor(
        injector: Injector,
        private _khuyenMaiService: QuanLyKhuyenMaiServiceProxy,
        private _dialog: MatDialog
    ) {
        super(injector);
    }

    protected list(
        request: PagedNhomDichVuRequestDto,
        pageNumber: number,
        finishedCallback: Function
    ): void {
        request.keyword = this.keyword;
        this._khuyenMaiService.getAll(
            request.keyword,
            request.skipCount,
            request.maxResultCount
        ).pipe(
            finalize(() => {
                finishedCallback();
            })
        ).subscribe(result => {
            this.khuyenMai = result.items;
            this.showPaging(result, pageNumber);
        });
    }

    create(): void {
        this.showCreateOrEditDialog();
    }

    edit(danhMucNhomDichVuId: number): void {
        this.showCreateOrEditDialog(danhMucNhomDichVuId);
    }

    protected delete(khuyenMai: TinTucDto): void {
        abp.message.confirm(
            this.l('DeleteWarningMessage', khuyenMai.name),
            this.l('AreYouSure'),
            (result: boolean) => {
                if (result) {
                    this._khuyenMaiService.delete(khuyenMai.id).subscribe(() => {
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
            createOrEditDialog = this._dialog.open(CreateKhuyenMaiDialogComponent);
        } else {
            createOrEditDialog = this._dialog.open(EditKhuyenMaiDialogComponent, {
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
