import { Component, Injector, OnInit, Inject, Optional } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import { DanhMucHangMucServiceProxy, DanhMucHangMucDto, LookupTableServiceProxy, LookupTableDto } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: 'edit-hangmuc-dialog.component.html',
    styles: [`
        mat-form-field {
            width: 100%;
        }
        mat-checkbox {
            padding-bottom: 5px;
        }
    `]
})
export class EditHangMucDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    hangMucName = '';
    danhMucHangMuc: DanhMucHangMucDto = new DanhMucHangMucDto();
    danhSachNhomDichVu: LookupTableDto[] = [];
    danhSachDichVu: LookupTableDto[] = [];
    
    constructor(
        injector: Injector,
        private _danhMucHangMucService: DanhMucHangMucServiceProxy,
        private _lookupTableService: LookupTableServiceProxy,
        private _dialogRef: MatDialogRef<EditHangMucDialogComponent>,
        @Optional() @Inject(MAT_DIALOG_DATA) private _id: number
    ) {
        super(injector);

        this._lookupTableService.getAllNhomDichVuForLookupTable().subscribe(result => {
            this.danhSachNhomDichVu = result;
        });
    }

    ngOnInit(): void {
        this._danhMucHangMucService.get(this._id).subscribe(result => {
            this.danhMucHangMuc = result;

            this.hangMucName = result.name;

            if (result.nhomDichVuId > 0)
            {
                this.onChangeNhomDichVu(result.nhomDichVuId);
            }
        })
    }

    onChangeNhomDichVu(nhomDichVuId: number): void {
        this._lookupTableService.getAllDichVuForLookupTable(nhomDichVuId).subscribe(result => {
            this.danhSachDichVu = result;

            if (this.danhMucHangMuc.dichVuId > 0) {
                if (this.danhSachDichVu.findIndex(w => w.id == this.danhMucHangMuc.dichVuId) < 0)
                    this.danhMucHangMuc.dichVuId = undefined;
            }
        })
    }

    save(): void {
        this.saving = true;
        this._danhMucHangMucService.update(this.danhMucHangMuc).pipe(
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
