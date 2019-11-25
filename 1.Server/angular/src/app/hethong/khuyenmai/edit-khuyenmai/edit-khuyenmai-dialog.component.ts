import { Component, Injector, Optional, Inject } from '@angular/core';
import { MatDialogRef, _MatOptgroupMixinBase, MAT_DIALOG_DATA } from '@angular/material';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import { QuanLyKhuyenMaiServiceProxy, TinTucDto } from '@shared/service-proxies/service-proxies';
import { Ng2ImgMaxService } from 'ng2-img-max';
import { AppConsts } from '@shared/AppConsts';

@Component({
    templateUrl: 'edit-khuyenmai-dialog.component.html',
    styleUrls: ['./edit-khuyenmai-dialog.component.css'],
})
export class EditKhuyenMaiDialogComponent extends AppComponentBase {
    saving = false;
    imageBase64 = AppConsts.khuyenMaiImage;
    khuyenMai: TinTucDto = new TinTucDto();

    constructor(
        injector: Injector,
        private _ng2ImgMaxService: Ng2ImgMaxService,
        private _khuyenMaiService: QuanLyKhuyenMaiServiceProxy,
        private _dialogRef: MatDialogRef<EditKhuyenMaiDialogComponent>,
        @Optional() @Inject(MAT_DIALOG_DATA) _id: number
    ) {
        super(injector);

        this._khuyenMaiService.get(_id).subscribe(result => {
            this.khuyenMai = result;
            if (this.khuyenMai.imageBase64.length > 0)
                this.imageBase64 = this.khuyenMai.imageBase64;
        });
    }

    save(): void {
        this.saving = true;

        this.khuyenMai.imageBase64 = this.imageBase64;
        this._khuyenMaiService.update(this.khuyenMai).pipe(
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
        this.imageBase64 = btoa(binaryString);
    }

    close(result: any): void {
        this._dialogRef.close(result);
    }
}
