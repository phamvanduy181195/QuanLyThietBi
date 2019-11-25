import { Component, Injector } from '@angular/core';
import { MatDialog } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import { DanhMucNhomDichVuServiceProxy, DanhMucNhomDichVuDto } from '@shared/service-proxies/service-proxies';
import { CreateNhomDichVuDialogComponent } from './create-nhomdichvu/create-nhomdichvu-dialog.component';
import { EditNhomDichVuDialogComponent } from './edit-nhomdichvu/edit-nhomdichvu-dialog.component';

class PagedNhomDichVuRequestDto extends PagedRequestDto {
    keyword: string;
}

@Component({
    templateUrl: './nhomdichvu.component.html',
    animations: [appModuleAnimation()],
    styles: [`
        mat-form-field {
            padding: 10px;
        }
    `]
})
export class DanhMucNhomDichVuComponent extends PagedListingComponentBase<DanhMucNhomDichVuDto> {
    danhMucNhomDichvu: DanhMucNhomDichVuDto[] = [];
    keyword = '';

    constructor(
        injector: Injector,
        private _danhMucNhomDichVuService: DanhMucNhomDichVuServiceProxy,
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
        this._danhMucNhomDichVuService.getAll(
            request.keyword,
            request.skipCount,
            request.maxResultCount
        ).pipe(
            finalize(() => {
                finishedCallback();
            })
        ).subscribe(result => {
            this.danhMucNhomDichvu = result.items;
            this.showPaging(result, pageNumber);
        });
    }

    create(): void {
        this.showCreateOrEditDialog();
    }

    edit(danhMucNhomDichVuId: number): void {
        this.showCreateOrEditDialog(danhMucNhomDichVuId);
    }

    protected delete(danhMucNhomDichvu: DanhMucNhomDichVuDto): void {
        abp.message.confirm(
            this.l('DeleteWarningMessage', danhMucNhomDichvu.name),
            this.l('AreYouSure'),
            (result: boolean) => {
                if (result) {
                    this._danhMucNhomDichVuService.delete(danhMucNhomDichvu.id).subscribe(() => {
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
            createOrEditDialog = this._dialog.open(CreateNhomDichVuDialogComponent);
        } else {
            createOrEditDialog = this._dialog.open(EditNhomDichVuDialogComponent, {
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
