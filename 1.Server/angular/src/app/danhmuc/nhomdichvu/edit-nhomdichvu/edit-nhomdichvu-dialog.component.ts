import { Component, Injector, Optional, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import { DanhMucNhomDichVuServiceProxy, DanhMucNhomDichVuDto } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: 'edit-nhomdichvu-dialog.component.html',
    styles: [`
        mat-form-field {
            width: 100%;
        }
        mat-checkbox {
            padding-bottom: 5px;
        }
    `]
})
export class EditNhomDichVuDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    nhomDichVuName = '';
    danhMucNhomDichVu: DanhMucNhomDichVuDto = new DanhMucNhomDichVuDto();

    constructor(
        injector: Injector,
        private _danhMucNhomDichVuService: DanhMucNhomDichVuServiceProxy,
        private _dialogRef: MatDialogRef<EditNhomDichVuDialogComponent>,
        @Optional() @Inject(MAT_DIALOG_DATA) private _id: number
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this._danhMucNhomDichVuService.get(this._id).subscribe(result => {
            this.danhMucNhomDichVu = result;
            
            this.nhomDichVuName = result.name;
        })
    }

    save(): void {
        this.saving = true;

        this._danhMucNhomDichVuService.update(this.danhMucNhomDichVu).pipe(
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
