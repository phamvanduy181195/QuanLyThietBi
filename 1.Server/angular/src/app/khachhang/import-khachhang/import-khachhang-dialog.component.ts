import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { AppConsts } from '@shared/AppConsts';
import { FileUploader, FileUploaderOptions } from 'ng2-file-upload';
import { MatDialogRef } from '@angular/material';
import { KhachHangServiceProxy } from '@shared/service-proxies/service-proxies';
import { TokenService } from 'abp-ng2-module/dist/src/auth/token.service';
import { finalize } from 'rxjs/operators';
import { FileDownloadService } from '@shared/utils/file-download.service'

const URL = AppConsts.remoteServiceBaseUrl + '/api/Import/KhachHang';

@Component({
    templateUrl: './import-khachhang-dialog.component.html',
    styleUrls: ['./import-khachhang-dialog.component.css'],
})

export class ImportKhachHangDialogComponent extends AppComponentBase implements OnInit {
    public uploader: FileUploader;
    private _uploaderOptions: FileUploaderOptions = {};

    saving = false;
    addedFile = false;
    refreshList = false;

    fileUploadName: string = "";
    fileSaveMessage: string = "";

    constructor(
        injector: Injector,
        private _khachHangService: KhachHangServiceProxy,
        private _tokenService: TokenService,
        private _fileDownloadService: FileDownloadService,
        private _dialogRef: MatDialogRef<ImportKhachHangDialogComponent>
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.initFileUpload();
    }

    initFileUpload(): void {
        const self = this;

        //Khới tạo giá trị để Upload File
        self.uploader = new FileUploader({ url: URL, allowedFileType: ["xls", "xlsx"], maxFileSize: 5000000 });
        self._uploaderOptions.authToken = 'Bearer ' + self._tokenService.getToken();
        self._uploaderOptions.autoUpload = false;
        self._uploaderOptions.removeAfterUpload = true;

        //Chọn file bị lỗi
        self.uploader.onWhenAddingFileFailed = (item: any, filter: any, options: any) => {
            switch (filter.name) {
                case 'fileType':
                    this.message.warn(this.l('ImportKhachHang_msgWrongFileType'));
                    break;
                default:
                    this.message.warn(this.l('ImportKhachHang_msgWrongFileSize'));
                    break;
            }

            this.reset();
        }

        //Chọn file thành công
        self.uploader.onAfterAddingFile = (file) => {
            if (this.uploader.queue.length > 1) {
                this.uploader.removeFromQueue(this.uploader.queue[0]);
            }

            this.addedFile = true;
            this.fileUploadName = file.file.name
            this.fileSaveMessage = "";

            file.withCredentials = false;
        }

        self.uploader.onSuccessItem = (item, response, status) => {
            this.addedFile = false;

            var fileUploadedName = JSON.parse(response).result;
            this._khachHangService.importFileExcel(fileUploadedName).pipe(
                finalize(() => { this.saving = false; this.addedFile = false; })
            ).subscribe(result => {
                this.fileSaveMessage = result;

                this.refreshList = true;
            });
        };

        self.uploader.onErrorItem = (item, response, status, headers) => {
            this.addedFile = false;
            self.message.error(self.l('ImportKhachHang_msgUploadFileError'));
        }

        self.uploader.setOptions(self._uploaderOptions);
    }

    reset(): void {
        this.addedFile = false;
        this.fileUploadName = "";
        this.fileSaveMessage = '';

        $("#importFileInput").val('');
    }

    save(): void {
        if (this.uploader.queue.length <= 0)
        {
            abp.message.warn(this.l('ImportKhachHang_lblNotSelectFileMessage'));

            this.reset();
            return;
        }

        this.saving = true;
        this.fileSaveMessage = this.l('ImportKhachHang_msgImportingFile', this.fileUploadName);
        this.uploader.uploadAll();
    }

    download(): void {
        this._khachHangService.downloadFileMau().subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
        });
    }

    close(result: any): void {
        this._dialogRef.close(result);
    }
}