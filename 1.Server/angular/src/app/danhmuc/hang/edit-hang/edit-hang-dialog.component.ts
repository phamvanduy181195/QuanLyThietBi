import { Component, Injector, Optional, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import { DanhMucHangServiceProxy, DanhMucHangDto } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: 'edit-hang-dialog.component.html',
    styles: [`
        mat-form-field {
            width: 100%;
        }
        mat-checkbox {
            padding-bottom: 5px;
        }
    `]
})
export class EditHangDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    hangName = '';
    danhMucHang: DanhMucHangDto = new DanhMucHangDto();

    constructor(
        injector: Injector,
        private _danhMucHangService: DanhMucHangServiceProxy,
        private _dialogRef: MatDialogRef<EditHangDialogComponent>,
        @Optional() @Inject(MAT_DIALOG_DATA) private _id: number
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this._danhMucHangService.get(this._id).subscribe(result => {
            this.danhMucHang = result;
            
            this.hangName = result.name;
        })
    }

    save(): void {
        this.saving = true;

        this._danhMucHangService.update(this.danhMucHang).pipe(
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
