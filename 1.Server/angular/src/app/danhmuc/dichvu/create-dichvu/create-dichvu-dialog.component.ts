import { Component, Injector } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import { DanhMucDichVuServiceProxy, CreateDanhMucDichVuDto, LookupTableServiceProxy, LookupTableDto } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: 'create-dichvu-dialog.component.html',
    styles: [`
        mat-form-field {
            width: 100%;
        }
        mat-checkbox {
            padding-bottom: 5px;
        }
    `]
})
export class CreateDichVuDialogComponent extends AppComponentBase {
    saving = false;
    danhMucDichVu: CreateDanhMucDichVuDto = new CreateDanhMucDichVuDto();
    danhSachNhomDichVu: LookupTableDto[] = [];
    
    constructor(
        injector: Injector,
        private _danhMucDichVuService: DanhMucDichVuServiceProxy,
        private _lookupTableService: LookupTableServiceProxy,
        private _dialogRef: MatDialogRef<CreateDichVuDialogComponent>
    ) {
        super(injector);

        this._lookupTableService.getAllNhomDichVuForLookupTable().subscribe(result => {
            this.danhSachNhomDichVu = result;
        })
    }

    save(): void {
        this.saving = true;
        this._danhMucDichVuService.create(this.danhMucDichVu).pipe(
            finalize(() => {
                this.saving = false;
            })
        ).subscribe(() => {
            this.notify.info(this.l('SavedSuccessfully'));
            this.close(true);
        });
    }

    close(result: any): void {
        this._dialogRef.close(result);
    }
}
