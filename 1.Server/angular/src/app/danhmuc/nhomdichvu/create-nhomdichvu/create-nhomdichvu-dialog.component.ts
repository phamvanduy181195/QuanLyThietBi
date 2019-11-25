import { Component, Injector } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import { DanhMucNhomDichVuServiceProxy, CreateDanhMucNhomDichVuDto } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: 'create-nhomdichvu-dialog.component.html',
    styles: [`
        mat-form-field {
            width: 100%;
        }
        mat-checkbox {
            padding-bottom: 5px;
        }
    `]
})
export class CreateNhomDichVuDialogComponent extends AppComponentBase {
    saving = false;
    danhMucNhomDichVu: CreateDanhMucNhomDichVuDto = new CreateDanhMucNhomDichVuDto();

    constructor(
        injector: Injector,
        private _danhMucNhomDichVuService: DanhMucNhomDichVuServiceProxy,
        private _dialogRef: MatDialogRef<CreateNhomDichVuDialogComponent>
    ) {
        super(injector);
    }

    save(): void {
        this.saving = true;

        this._danhMucNhomDichVuService.create(this.danhMucNhomDichVu).pipe(
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
