import { Component, Injector } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import { KhachHangServiceProxy, KhachHangDto, EntityDtoOfInt64, LockUnlockInput, } from '@shared/service-proxies/service-proxies';
import { CreateKhachHangDialogComponent } from './create-khachhang/create-khachhang-dialog.component';
import { EditKhachHangDialogComponent } from './edit-khachhang/edit-khachhang-dialog.component';
import { SetPasswordDialogComponent } from './set-password/set-password.component';
import { ImportKhachHangDialogComponent } from './import-khachhang/import-khachhang-dialog.component';

class PagedKhachHangRequestDto extends PagedRequestDto {
    keyword: string;
}

@Component({
    templateUrl: './khachhang.component.html',
    animations: [appModuleAnimation()],
    styles: [
        `
          mat-form-field {
            padding: 5px;
          }
        `
    ]
})
export class KhachHangComponent extends PagedListingComponentBase<KhachHangDto> {
    khachHang: KhachHangDto[] = [];
    keyword = '';

    constructor(
        injector: Injector,
        private _khachHangService: KhachHangServiceProxy,
        private _dialog: MatDialog
    ) {
        super(injector);
    }

    createKhachHang(): void {
        this.showCreateOrEditDialog();
    }

    editKhachHang(khachHangId: number): void {
        this.showCreateOrEditDialog(khachHangId);
    }

    activeKhachHang(khachHang: KhachHangDto): void {
        abp.message.confirm(
            this.l('ActiveConfirmMessage', khachHang.phoneNumber),
            this.l('AreYouSure'),
            (result: boolean) => {
                if (result) {
                    this._khachHangService.activeAccount(new EntityDtoOfInt64({id: khachHang.userId})).subscribe(() => {
                        abp.notify.success(this.l('ActivatedSuccessfully'));
                        this.refresh();
                    });
                }
            }
        );
    }

    lockUnlockKhachHang(khachHang: KhachHangDto): void {
        abp.message.confirm(
            this.l(khachHang.isLocked ? 'UnlockConfirmMessage' : 'LockConfirmMessage', khachHang.phoneNumber),
            this.l('AreYouSure'),
            (result: boolean) => {
                if (result) {
                    this._khachHangService.lockUnlockAccount(new LockUnlockInput({ userId: khachHang.userId, isLocked: !khachHang.isLocked })).subscribe(() => {
                        if (khachHang.isLocked) {
                            abp.notify.success(this.l('UnlockedSuccessfully'));
                        } else {
                            abp.notify.success(this.l('LockedSuccessfully'));
                        }
                        this.refresh();
                    });
                }
            }
        );
    }

    setPasswordKhachHang(khachHang: KhachHangDto): void {
        let setPasswordDialog = this._dialog.open(SetPasswordDialogComponent, {
            data: {
                userId: khachHang.userId,
                phoneNumber: khachHang.phoneNumber
            }
        });

        setPasswordDialog.afterClosed().subscribe(result => {
            if (result) {
                this.refresh();
            }
        });
    }

    createAccountKhachHang(khachHang: KhachHangDto): void {
        abp.message.confirm(
            this.l('CreateAccountConfirmMessage', khachHang.phoneNumber),
            this.l('AreYouSure'),
            (result: boolean) => {
                if (result) {
                    this._khachHangService.createAccount(new EntityDtoOfInt64({id: khachHang.id})).subscribe(() => {
                        abp.notify.success(this.l('CreateAccountSuccessfully'));
                        this.refresh();
                    });
                }
            }
        );
    }

    importKhachHang(): void {
        let dialogConfig = new MatDialogConfig();
        dialogConfig.disableClose = false;

        let importDialog = this._dialog.open(ImportKhachHangDialogComponent, dialogConfig);

        importDialog.afterClosed().subscribe(result => {
            if (result) {
                this.refresh();
            }
        });
    }

    protected list(
        request: PagedKhachHangRequestDto,
        pageNumber: number,
        finishedCallback: Function
    ): void {
        request.keyword = this.keyword;
        this._khachHangService.getAll(request.keyword, request.skipCount, request.maxResultCount).pipe(
            finalize(() => {
                finishedCallback();
            })
        ).subscribe(result => {
            this.khachHang = result.items;
            this.showPaging(result, pageNumber);
        });
    }

    protected delete(khachHang: KhachHangDto): void {
        abp.message.confirm(
            this.l('DeleteWarningMessage', khachHang.name),
            this.l('AreYouSure'),
            (result: boolean) => {
                if (result) {
                    this._khachHangService.delete(khachHang.id).subscribe(() => {
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
            createOrEditDialog = this._dialog.open(CreateKhachHangDialogComponent);
        } else {
            createOrEditDialog = this._dialog.open(EditKhachHangDialogComponent, {
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
