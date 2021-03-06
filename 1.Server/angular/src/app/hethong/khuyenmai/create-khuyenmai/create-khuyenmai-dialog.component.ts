import { Component, Injector } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import { QuanLyKhuyenMaiServiceProxy, CreateTinTucDto } from '@shared/service-proxies/service-proxies';
import { Ng2ImgMaxService } from 'ng2-img-max';
import { AppConsts } from '@shared/AppConsts';

@Component({
    templateUrl: 'create-khuyenmai-dialog.component.html',
    styleUrls: ['./create-khuyenmai-dialog.component.css'],
})
export class CreateKhuyenMaiDialogComponent extends AppComponentBase {
    saving = false;
    khuyenMai: CreateTinTucDto = new CreateTinTucDto();

    constructor(
        injector: Injector,
        private _ng2ImgMaxService: Ng2ImgMaxService,
        private _khuyenMaiService: QuanLyKhuyenMaiServiceProxy,
        private _dialogRef: MatDialogRef<CreateKhuyenMaiDialogComponent>
    ) {
        super(injector);

        this.khuyenMai.imageBase64 = AppConsts.khuyenMaiImage;
    }

    save(): void {
        this.saving = true;

        this._khuyenMaiService.create(this.khuyenMai).pipe(
            finalize(() => {
                this.saving = false;
            })
        ).subscribe(() => {
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
            this._ng2ImgMaxService.resizeImage(file, 800, 600).subscribe(
                result => {
                    reader.readAsBinaryString(result);
                }
            );
        }
    }

    handleReaderLoaded(readerEvt: any) {
        var binaryString = readerEvt.target.result;
        this.khuyenMai.imageBase64 = btoa(binaryString);
    }

    close(result: any): void {
        this._dialogRef.close(result);
    }
}
