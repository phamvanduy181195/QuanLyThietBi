import { Component, Injector, EventEmitter, OnInit } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import { ThongTinDinhKemKetQuaCongViecDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'dinhkemSampleChitiet',
  templateUrl: './dinhkem-chitiet.component.html',
  inputs: ['uploadedFile', 'onlyView'],
  outputs: ['onDownload', 'onDelete']
})
export class DinhkemSampleChitietComponent extends AppComponentBase implements OnInit {

    uploadedFile: ThongTinDinhKemKetQuaCongViecDto = new ThongTinDinhKemKetQuaCongViecDto();

    onDownload: EventEmitter<ThongTinDinhKemKetQuaCongViecDto> = new EventEmitter<ThongTinDinhKemKetQuaCongViecDto>();
    onDelete: EventEmitter<ThongTinDinhKemKetQuaCongViecDto> = new EventEmitter<ThongTinDinhKemKetQuaCongViecDto>();

    onlyView: boolean | undefined = false;
    /**
     * @Readonly
     */
    readonly fileExtension: string[] = [
        '.doc',
        '.docx',
        '.xls',
        '.xlsx',
        '.txt',
        '.pdf',
        '.ppt',
        '.pptx'
    ];

    readonly ThumbFileClass = 'dz-preview dz-file-preview';
    readonly ThumbImageClass = 'dz-preview dz-image-preview';
    readonly EmptyImage: string = 'null.png';
    /**
     * @ATTRIBUTE
     */
    fileSize: string;
    fileUnit: string;
    fileName: string;
    imageUrl: string;
    isImage: boolean;
    isFile: boolean;
    thumbClass: string;

    constructor(injector: Injector) {
        super(injector);
    }



    ngOnInit() {
        this.isFile = false;
        if (!_.isUndefined(this.uploadedFile.fileName) && !_.isNull(this.uploadedFile.fileName) && this.uploadedFile.fileName != this.EmptyImage) {
            this.isFile = true;

            this.fileSize = this.changeFileSize(this.uploadedFile.fileSize);
            this.fileName = this.uploadedFile.fileName;
            this.fileUnit = this.uploadedFile.fileName.substring(this.uploadedFile.fileName.lastIndexOf('.'), this.uploadedFile.fileName.length);

            this.isImage = this.fileExtension.indexOf(this.fileUnit) < 0;
            this.thumbClass = this.isImage ? this.ThumbImageClass : this.ThumbFileClass;
            this.imageUrl = this.isImage ?
                `${AppConsts.remoteServiceBaseUrl}${this.uploadedFile.fileURL}` :
                undefined;
        }
    }

    changeFileSize(fileSize: any): string {
        fileSize = fileSize / 1024;
        if (fileSize > 1024) {
            return `${(fileSize / 1024).toFixed(2)} MB`;
        }
        else if (fileSize < 1) {
            fileSize = 1;
        }
        return `${fileSize.toFixed(2)} KB`;
    }

    download(): void {
        this.onDownload.emit(this.uploadedFile);
    }

    delete(): void {
        this.onDelete.emit(this.uploadedFile);
    }
}
