import { Component, Injector } from '@angular/core';
import { MatDialog } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import { TramDichVuServiceProxy, TramDichVuDto, GetTramDichVuForView } from '@shared/service-proxies/service-proxies';
import { CreateTramDichVuDialogComponent } from './create-tram/create-tram-dialog.component';
import { EditTramDichVuDialogComponent } from './edit-tram/edit-tram-dialog.component';

class PagedTramRequestDto extends PagedRequestDto {
    keyword: string;
}

@Component({
    templateUrl: './tramdichvu.component.html',
    animations: [appModuleAnimation()],
    styles: [`
        mat-form-field {
        padding: 10px;
        }
    `]
})
export class TramDichVuComponent extends PagedListingComponentBase<TramDichVuDto> {
    tramdichvu: GetTramDichVuForView[] = [];
    keyword = '';

    constructor(
        injector: Injector,
        private _tramDichVuServiceProxy: TramDichVuServiceProxy,
        private _dialog: MatDialog
    ) {
        super(injector);
    }

    createTramDichVu(): void {
        this.showCreateOrEditDialog();
    }

    editTramDichVu(tramDichVuId: number): void {
        this.showCreateOrEditDialog(tramDichVuId);
    }

    protected list(
        request: PagedTramRequestDto,
        pageNumber: number,
        finishedCallback: Function
    ): void {
        request.keyword = this.keyword;
        this._tramDichVuServiceProxy
            .getAllNew(request.keyword, request.skipCount, request.maxResultCount)
            .pipe(
                finalize(() => {
                    finishedCallback();
                })
            )
            .subscribe(result => {
                this.tramdichvu = result.items;
                this.showPaging(result, pageNumber);
            });
    }

    protected delete(tramdichvu: TramDichVuDto): void {
        abp.message.confirm(
            this.l('DeleteWarningMessage', tramdichvu.name),
            this.l('AreYouSure'),
            (result: boolean) => {
                if (result) {
                    this._tramDichVuServiceProxy.delete(tramdichvu.id).subscribe(() => {
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
            createOrEditDialog = this._dialog.open(CreateTramDichVuDialogComponent);
        } else {
            createOrEditDialog = this._dialog.open(EditTramDichVuDialogComponent, {
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
