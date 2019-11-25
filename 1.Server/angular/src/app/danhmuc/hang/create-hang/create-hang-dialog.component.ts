import { Component, Injector } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import { DanhMucHangServiceProxy, CreateDanhMucHangDto } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: 'create-hang-dialog.component.html',
    styles: [`
        mat-form-field {
            width: 100%;
        }
        mat-checkbox {
            padding-bottom: 5px;
        }
    `]
})
export class CreateHangDialogComponent extends AppComponentBase {
    saving = false;
    danhMucHang: CreateDanhMucHangDto = new CreateDanhMucHangDto();

    constructor(
        injector: Injector,
        private _danhMucHangService: DanhMucHangServiceProxy,
        private _dialogRef: MatDialogRef<CreateHangDialogComponent>
    ) {
        super(injector);
    }

    save(): void {
        this.saving = true;

        this._danhMucHangService.create(this.danhMucHang).pipe(
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
