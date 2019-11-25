import { Component, Injector, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import { UserServiceProxy, UserDto, GetUserForView } from '@shared/service-proxies/service-proxies';
import { LookupTableServiceProxy, LookupTableDto } from '@shared/service-proxies/service-proxies';
import { CreateUserDialogComponent } from './create-user/create-user-dialog.component';
import { EditUserDialogComponent } from './edit-user/edit-user-dialog.component';
import { ResetPasswordDialogComponent } from './reset-password/reset-password.component';

class PagedUsersRequestDto extends PagedRequestDto {
    keyword: string;
    isActive: boolean | null;
    tramDichVuId: number;
}

@Component({
    templateUrl: './users.component.html',
    animations: [appModuleAnimation()],
    styles: [`
        mat-form-field {
            padding: 10px;
        }
    `]
})
export class UsersComponent extends PagedListingComponentBase<GetUserForView> {
    userForViews: GetUserForView[] = [];
    keyword = '';
    isActive: boolean | null;
    tramDichVuId: number;
    danhSachTramDichVu: LookupTableDto[] = [];
    
    constructor(
        injector: Injector,
        private _userService: UserServiceProxy,
        private _lookupTableService: LookupTableServiceProxy,
        private _dialog: MatDialog
    ) {
        super(injector);

        this._lookupTableService.getAllTramDichVuForLookupTable().subscribe(result => {
            this.danhSachTramDichVu = result;
        });
    }

    createUser(): void {
        this.showCreateOrEditUserDialog();
    }

    editUser(user: UserDto): void {
        this.showCreateOrEditUserDialog(user.id);
    }

    public resetPassword(user: UserDto): void {
        this.showResetPasswordUserDialog(user.id);
    }

    protected list(
        request: PagedUsersRequestDto,
        pageNumber: number,
        finishedCallback: Function
    ): void {
        request.keyword = this.keyword;
        request.isActive = this.isActive;// != null ? this.isActive : undefined;
        request.tramDichVuId = this.tramDichVuId;// > 0 ? this.tramDichVuId : undefined;

        this._userService
            .getAllNew(request.keyword, request.isActive, request.tramDichVuId, request.skipCount, request.maxResultCount)
            .pipe(
                finalize(() => {
                    finishedCallback();
                })
            )
            .subscribe(result => {
                this.userForViews = result.items;
                this.showPaging(result, pageNumber);
            });
    }

    protected delete(userForView: GetUserForView): void {
        abp.message.confirm(
            this.l('DeleteWarningMessage', userForView.user.name),
            this.l('AreYouSure'),
            (result: boolean) => {
                if (result) {
                    this._userService.delete(userForView.user.id).subscribe(() => {
                        abp.notify.success(this.l('DeletedSuccessfully'));
                        this.refresh();
                    });
                }
            }
        );
    }

    private showResetPasswordUserDialog(userId?: number): void {
        this._dialog.open(ResetPasswordDialogComponent, {
            data: userId
        });
    }

    private showCreateOrEditUserDialog(id?: number): void {
        let createOrEditUserDialog;
        if (id === undefined || id <= 0) {
            createOrEditUserDialog = this._dialog.open(CreateUserDialogComponent);
        } else {
            createOrEditUserDialog = this._dialog.open(EditUserDialogComponent, {
                data: id
            });
        }

        createOrEditUserDialog.afterClosed().subscribe(result => {
            if (result) {
                this.refresh();
            }
        });
    }
}
