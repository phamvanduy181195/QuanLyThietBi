import { Component, Injector } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import { DanhMucHangMucServiceProxy, CreateDanhMucHangMucDto, LookupTableServiceProxy, LookupTableDto } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: 'create-hangmuc-dialog.component.html',
    styles: [`
        mat-form-field {
            width: 100%;
        }
        mat-checkbox {
            padding-bottom: 5px;
        }
    `]
})
export class CreateHangMucDialogComponent extends AppComponentBase {
    saving = false;
    danhMucHangMuc: CreateDanhMucHangMucDto = new CreateDanhMucHangMucDto();
    danhSachNhomDichVu: LookupTableDto[] = [];
    danhSachDichVu: LookupTableDto[] = [];
    
    constructor(
        injector: Injector,
        private _danhMucHangMucService: DanhMucHangMucServiceProxy,
        private _lookupTableService: LookupTableServiceProxy,
        private _dialogRef: MatDialogRef<CreateHangMucDialogComponent>
    ) {
        super(injector);

        this._lookupTableService.getAllNhomDichVuForLookupTable().subscribe(result => {
            this.danhSachNhomDichVu = result;
        });
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
        this._danhMucHangMucService.create(this.danhMucHangMuc).pipe(
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
