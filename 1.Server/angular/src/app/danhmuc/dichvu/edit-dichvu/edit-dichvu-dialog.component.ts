import { Component, Injector, OnInit, Inject, Optional } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import { DanhMucDichVuServiceProxy, DanhMucDichVuDto, LookupTableServiceProxy, LookupTableDto } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: 'edit-dichvu-dialog.component.html',
    styles: [`
        mat-form-field {
            width: 100%;
        }
        mat-checkbox {
            padding-bottom: 5px;
        }
    `]
})
export class EditDichVuDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    dichVuName = '';
    danhMucDichVu: DanhMucDichVuDto = new DanhMucDichVuDto();
    danhSachNhomDichVu: LookupTableDto[] = [];
    
    constructor(
        injector: Injector,
        private _danhMucDichVuService: DanhMucDichVuServiceProxy,
        private _lookupTableService: LookupTableServiceProxy,
        private _dialogRef: MatDialogRef<EditDichVuDialogComponent>,
        @Optional() @Inject(MAT_DIALOG_DATA) private _id: number
    ) {
        super(injector);

        this._lookupTableService.getAllNhomDichVuForLookupTable().subscribe(result => {
            this.danhSachNhomDichVu = result;
        })
    }

    ngOnInit(): void {
        this._danhMucDichVuService.get(this._id).subscribe(result => {
            this.danhMucDichVu = result;

            this.dichVuName = result.name;
        })
    }

    save(): void {
        this.saving = true;

        this._danhMucDichVuService.update(this.danhMucDichVu).pipe(
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
