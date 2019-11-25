import { Component, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MatCheckboxChange } from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { Ng2ImgMaxService } from 'ng2-img-max';
import { AppComponentBase } from '@shared/app-component-base';
import { UserServiceProxy, CreateUserDto, RoleDto, GetAllQuanHuyenInput } from '@shared/service-proxies/service-proxies';
import { LookupTableServiceProxy, LookupTableDto } from '@shared/service-proxies/service-proxies';
import { AppConsts } from '@shared/AppConsts';

@Component({
    templateUrl: './create-user-dialog.component.html',
    styleUrls: ['./create-user-dialog.component.css'],
})

export class CreateUserDialogComponent extends AppComponentBase
    implements OnInit {
    saving = false;
    user: CreateUserDto = new CreateUserDto();
    roles: RoleDto[] = [];
    danhSachTramDichVu: LookupTableDto[] = [];
    danhSachNhomDichVu: LookupTableDto[] = [];
    danhSachTinhThanh: LookupTableDto[] = [];
    danhSachQuanHuyen: LookupTableDto[] = [];

    checkedRolesMap: { [key: string]: boolean } = {};
    defaultRoleCheckedStatus = false;
    totalRole = 0;

    testNhomDichVu: number[] = [1];

    constructor(
        injector: Injector,
        private _ng2ImgMaxService: Ng2ImgMaxService,
        private _userService: UserServiceProxy,
        private _lookupTableService: LookupTableServiceProxy,
        private _dialogRef: MatDialogRef<CreateUserDialogComponent>
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.user.profilePictureBase64 = AppConsts.profilePicture;
        
        this._lookupTableService.getAllTramDichVuForLookupTable().subscribe(result => {
            this.danhSachTramDichVu = result;
        });
        this._lookupTableService.getAllNhomDichVuForLookupTable().subscribe(result => {
            this.danhSachNhomDichVu = result;
        });
        this._lookupTableService.getAllTinhThanhForLookupTable().subscribe(result => {
            this.danhSachTinhThanh = result;
        })

        this.user.isActive = true;
        this._userService.getRoles().subscribe(result => {
            this.roles = result.items;
            this.setInitialRolesStatus();
        });
    }

    onChangeTinhThanh(isOpenSelect: boolean): void {
        if (!isOpenSelect) {
            if (this.user.phuTrachTinhThanhIds && this.user.phuTrachTinhThanhIds.length > 0)
                this._lookupTableService.getAllQuanHuyenForLookupTable(
                    new GetAllQuanHuyenInput({tinhThanhIds: this.user.phuTrachTinhThanhIds})
                ).subscribe(result => {
                    this.danhSachQuanHuyen = result;
                    
                    if (this.user.phuTrachQuanHuyenIds)
                    {
                        let self = this;
                        this.user.phuTrachQuanHuyenIds = this.user.phuTrachQuanHuyenIds.filter(function(value) {
                            return self.danhSachQuanHuyen.findIndex(w => w.id == value) >= 0;
                        });
                    }
                });
            else
            {
                this.danhSachQuanHuyen = [];
                this.user.phuTrachQuanHuyenIds = [];
            }
        }
    }

    setInitialRolesStatus(): void {
        _.map(this.roles, item => {
            this.checkedRolesMap[item.normalizedName] = item.isDefault;

            if (item.isDefault)
                this.totalRole++;

            // this.checkedRolesMap[item.normalizedName] = this.isRoleChecked(
            //     item.normalizedName
            // );
        });
    }

    isRoleChecked(normalizedName: string): boolean {
        // just return default role checked status
        // it's better to use a setting
        return this.defaultRoleCheckedStatus;
    }

    onRoleChange(role: RoleDto, $event: MatCheckboxChange) {
        this.checkedRolesMap[role.normalizedName] = $event.checked;

        if ($event.checked)
            this.totalRole++;
        else
            this.totalRole--;
    }

    getCheckedRoles(): string[] {
        const roles: string[] = [];
        _.forEach(this.checkedRolesMap, function (value, key) {
            if (value) {
                roles.push(key);
            }
        });
        return roles;
    }

    save(): void {
        this.saving = true;

        this.user.roleNames = this.getCheckedRoles();

        this._userService
            .create(this.user)
            .pipe(
                finalize(() => {
                    this.saving = false;
                })
            )
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close(true);
            });
    }

    // Đọc file sau khi chọn
    handleFileSelect(evt: any) {
        var files = evt.target.files;
        var file = files[0];

        if (files && file) {
            var reader = new FileReader();

            // Binding hàm onload với function handleReaderLoaded
            reader.onload = this.handleReaderLoaded.bind(this);

            // Đọc file
            this._ng2ImgMaxService.resizeImage(file, 200, 200).subscribe(
                result => {
                    reader.readAsBinaryString(result);
                }
            );
        }
    }

    handleReaderLoaded(readerEvt: any) {
        var binaryString = readerEvt.target.result;
        this.user.profilePictureBase64 = btoa(binaryString);
    }

    close(result: any): void {
        this._dialogRef.close(result);
    }
}
