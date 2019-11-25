import { Component, OnDestroy, Injector, ViewChild, EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { Subscription } from 'rxjs';
import { DropzoneComponent, DropzoneConfigInterface } from 'ngx-dropzone-wrapper';
import { AppConsts } from '@shared/AppConsts';
import swal from 'sweetalert';
import { RequestOptions, Headers, Http, ResponseContentType} from '@angular/http';
import * as _ from 'lodash';
import { DatePipe } from '@angular/common';
import { QuanLyCongViecServiceProxy, DuplicateThongTinSampleOutput, ThongTinDinhKemKetQuaCongViecDto, ThongTinDinhKemThamDinhDto } from '@shared/service-proxies/service-proxies';
@Component({
    selector: 'dinhKem',
    styleUrls: ['./dinhkem.component.less'],
    
    inputs: ['workingFiles', 'congViec', 'onlyView'],
    outputs: ['successUpload'],
    templateUrl: './dinhkem.component.html'
})
export class DinhkemComponent extends AppComponentBase implements OnDestroy {
    // npm install ngx-dropzone-wrapper --save (lệnh chạy dùng install Dropzone => sinh ra file dropzone.component.d.ts)
    @ViewChild(DropzoneComponent) drpzone: DropzoneComponent;
    /**
     * @Input
     */
    // đã đính kèm lên rồi edit hoặc view
    tepLamViecHienTai: ThongTinDinhKemThamDinhDto[] = [];
    ketQuaThamDinhInput: number | undefined ;
    onlyView: boolean | undefined = false;
    /**
     * @Output
     */
    successUpload: EventEmitter<any | undefined> = new EventEmitter<any>();

    /**
     * @Readonly
     */

    readonly UploadUrl: string = AppConsts.remoteServiceBaseUrl + '/QuanLyCongViec/UploadFile';
    readonly DownloadUrl: string = AppConsts.remoteServiceBaseUrl + '/QuanLyCongViec/Download';
    readonly HttpHeaderAuthorization: string = 'Bearer ' + abp.auth.getToken();
    readonly StatusFileQueued: string = 'queued';
    readonly MaxBytesSizeFileUpload: string = '5242880';
    readonly EmptyImage: string = 'null.png';
    readonly B64DataDefault: string = 'iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==';
    readonly ContentType: string = 'image/png';


    /**
     * @Attribute
     */

    // LÀM VIỆC VỚI FILEUPLOAD
    listFileNameUpload: string[] = [];
    listFileNameDelete: string[] = [];
    totalFileUpload: number = 0;
    createFileExist: boolean = false;

    /**
     * @Config
     */
    config: DropzoneConfigInterface = {
        url: this.UploadUrl,
        maxFilesize: 5,
        acceptedFiles: 'image/*,.doc,.docx,.xls,.xlsx,.txt,.pdf,.ppt,.pptx',
        uploadMultiple: true,
        parallelUploads: 100,
        autoQueue: true,
        autoProcessQueue: false, // auto upload
        addRemoveLinks: true, // remove file upload
        dictRemoveFile: this.l('P1ThongTinDinhKemChiPhi_btnRemoveFile'),
        clickable: false,
        autoReset: null,
        errorReset: null,
        cancelReset: null,
        headers: {
            Authorization: this.HttpHeaderAuthorization
        }
    };

    /**
     * @Subcribe
     */

    apiBeforeUploadSubscription: Subscription = new Subscription();
    
    constructor(
        private _http: Http,
        private datePipe: DatePipe,
        private _quanLyCongViecServiceProxy: QuanLyCongViecServiceProxy,
        injector: Injector
    ) { 
        super(injector);
        this.config.clickable = !this.onlyView;
    }

    ngOnDestroy(): void {
        this.apiBeforeUploadSubscription.unsubscribe();
    }
    /**
     * @EventKetQuaCongViec
     */


    /**
     * REMOVE FILE HÀNG ĐỢI
     */

    removeFileIfExists(listFileNameDuplicate: string[], listFileNameUpload: string[]): void {
        listFileNameUpload.map(item => {
            var removeIndex = listFileNameDuplicate.indexOf(item);
            if (removeIndex > -1) {
                let indexRemoveFile = this.listFileNameUpload.indexOf(item);
                let files = this.drpzone.directiveRef.dropzone().files;

                // XOÁ FILE DROPZONE
                let filesRemove = files.filter((f: any) => f.name == item);
                if (filesRemove != undefined && filesRemove != null) {
                    this.drpzone.directiveRef.dropzone().removeFile(filesRemove);
                }
                // XOÁ TÊN FILE UPLOAD
                this.listFileNameUpload.splice(indexRemoveFile, 1);
            }
        });
    }

    /**
     * TẠO 1 FILE BASE 64
     * @param parts 
     * @param properties 
     */
    createBlob(parts: Array<any>, properties?: BlobPropertyBag | string): Blob {
        parts = parts || []
        properties = properties || {}
        if (typeof properties === 'string') {
            properties = { type: properties } // infer content type
        }
        try {
            return new Blob(parts, properties)
        } catch (e) {
            if (e.name !== 'TypeError') {
                throw e
            }
            var builder = new MSBlobBuilder()
            for (var i = 0; i < parts.length; i += 1) {
                builder.append(parts[i])
            }
            return builder.getBlob(properties.type)
        }
    }
    binaryStringToArrayBuffer(binary: string): ArrayBuffer {
        var length = binary.length
        var buf = new ArrayBuffer(length)
        var arr = new Uint8Array(buf)
        var i = -1
        while (++i < length) {
            arr[i] = binary.charCodeAt(i)
        }
        return buf
    }
    base64StringToBlob(base64: string, type?: string): Blob {
        var parts = [this.binaryStringToArrayBuffer(atob(base64))]
        return type ? this.createBlob(parts, { type: type }) : this.createBlob(parts)
    }

    onBeforeUpload(input:number ): void {
        debugger;
        this.ketQuaThamDinhInput = input;
        this.listFileNameUpload = !_.isNull(this.listFileNameUpload) && !_.isUndefined(this.listFileNameUpload) ? this.listFileNameUpload : [];
        // KIỂM TRA TRÙNG TỆP
        this.apiBeforeUploadSubscription = this._quanLyCongViecServiceProxy
            .getTotalDuplicateFile(this.ketQuaThamDinhInput, this.listFileNameUpload)
            .subscribe((result: DuplicateThongTinSampleOutput) => {
                if (result.total > 0) {
                    swal(
                        this.l('ThamDinh_titDKConfirmUpload'),
                        this.l(
                            'ThamDinh_msgDKConfirmUpload',
                            result
                        ),
                        'warning', {
                            buttons: {
                                btnCancel: {
                                    text: this.l('ThamDinh_btnDKCancel')
                                },
                                btnSave: {
                                    text: this.l('ThamDinh_btnDKSave')
                                }
                            }
                        }
                    ).then((value: string) => {
                        switch (value) {
                            case 'btnSave':
                                // SWAL('KẾT QUẢ', 'ĐÃ LƯU MỚI!', 'SUCCESS');
                                this.createFileExist = true;
                                this.drpzone.directiveRef.dropzone().processQueue();
                                break;
                            default:
                                // XOÁ FILE HIỆN TẠI ĐẨY LÊN
                                this.removeFileIfExists(result.listFileNameDuplicate, this.listFileNameUpload)
                                this.drpzone.directiveRef.dropzone().processQueue();
                        }
                    });
                } else {
                    // XỬ LÝ FILE SAU KHI KHÔNG CÓ FILE NÀO TRÙNG
                    if (this.listFileNameUpload.length > 0) {
                        this.drpzone.directiveRef.dropzone().processQueue();
                    } else {
                        // ADD FILE EMPTY ĐỂ ĐẨY LÊN SERVER XỬ LÝ
                        const blob = this.base64StringToBlob(this.B64DataDefault, this.ContentType);
                        var file: File = new File([blob], this.EmptyImage, { type: this.ContentType });
                        this.drpzone.directiveRef.dropzone().addFile(file)
                        this.drpzone.directiveRef.dropzone().processQueue();
                    }
                }
            });
    }


    /**
     * @EventDropzone
     */

    /**
     * GỬI FILE LÊN SERVER
     * @param formData THÊM THUỘC TÍNH TUỲ CHỌN LÊN SERVER NHƯ LÀ MODULE ĐANG TẠO
     */
    onSending(formData: any[]): void {
        let _formData = formData[2];
        _formData.append('ThamDinhId', this.ketQuaThamDinhInput);
        _formData.append('CreateFileExist', this.createFileExist);
        _formData.append('DeleteFileName', this.listFileNameDelete);
    }

    /**
     * UPLOAD FILE THÀNH CÔNG
     */
    onSuccess(input: any): void {
        var outputFileUpload = input[1];
        // PHỤ THUỘC VÀO KIỂU ĐẦU RA TRÊN SERVER
        if (outputFileUpload.result == null) {
            this.clearUploadFile();
        } else {
            // TRƯỜNG HỢP UPLOAD FILE KHÔNG THÀNH CÔNG CHO PHÉP UPLOAD LẠI FILE
            // FILE TỪ TRẠNG THÁI UPLOAD THÀNH CÔNG CHUYỂN SANG HÀNG ĐỢI CHỜ UPLOAD success->queued
            this.drpzone.directiveRef.dropzone().files.map((item: any) =>
                item.status = this.StatusFileQueued
            )
        }
        this.successUpload.emit(outputFileUpload.result);
    }



    // THÊM FILE CHƯA UPLOAD
    onAddedFile(file: any): void {
        var addIndex = this.listFileNameUpload.indexOf(file.name);
        // KIỂM TRA FILE TỒN TẠI KHÔNG
        if (addIndex < 0) {
            this.listFileNameUpload.push(file.name);
        } else {
            // GỠ FILE TỪ HÀNG ĐỢI
            this.drpzone.directiveRef.dropzone().removeFile(file);
        }
    }

    // XOÁ FILE CHƯA UPLOAD
    onRemoveFile(file: any): void {
        var removeIndex = this.listFileNameUpload.indexOf(file.name);
        // KIỂM TRA FILE TỒN TẠI KHÔNG
        if (removeIndex > -1) {
            this.listFileNameUpload.splice(removeIndex, 1);
            this.drpzone.directiveRef.dropzone().removeFile(file);
        }
    }

    // LỖI KHI THÊM FILE
    onError(error: any): void {
        let errorFile: any = error[0];
        if (!errorFile.accepted) {
            // LỖI VỀ KÍCH THƯỚC
            if (errorFile.size >= this.MaxBytesSizeFileUpload) {
                this.message.error(this.l('THCD_msgDKWrongFileSize'));
            }
            // LỖI VỀ KIỂU FILE
            else {
                this.message.error(this.l('THCD_msgDKWrongFileType'));
            }
            this.drpzone.directiveRef.dropzone().removeFile(errorFile);
            this.listFileNameUpload.splice(this.listFileNameUpload.indexOf(errorFile.name), 1);
        }
    }

    /**
     * CHỈ ÁP DỤNG XOÁ 1 FILE
     */
    onDelete(file: ThongTinDinhKemKetQuaCongViecDto) {
        this.message.confirm(
            this.l('THCD_msgDKTepSeBiXoaVinhVienKhoiHeThong',
                file.fileName
            ),
            this.l('THCD_titDKBanCoChacChanKhong'),
            (confirm) => {
                if (confirm) {
                    let deleteIndex = -1;
                    for (let index = 0; index < this.tepLamViecHienTai.length; index++) {
                        if (this.tepLamViecHienTai[index].id == file.id) {
                            this.tepLamViecHienTai.splice(deleteIndex, 1);
                            this.listFileNameDelete.push(file.fileName);
                            // BỔ SUNG GỌI API XOÁ
                            break;
                        }
                    }
                }
            }
        );
    }


    /**
     * CHỈ ÁP DỤNG DOWNLOAD 1 FILE
     * @param fileName
     */
    onDownload(file: ThongTinDinhKemKetQuaCongViecDto): void {
        let fileName = file.fileName;
        let id = file.id;
        let downloadFile = `{"FileList": ["${fileName}"]}`;
        let myHeaders = new Headers({
            Authorization: this.HttpHeaderAuthorization
        });

        let options = new RequestOptions({
            headers: myHeaders,
            responseType: ResponseContentType.Blob
        });
        let postParam = new URLSearchParams();
        postParam.append('Id', id.toString());
        postParam.append('DownloadFile', downloadFile);
        this._http
            .post(this.DownloadUrl, postParam, options)
            .subscribe((result) => {
                let blob = new Blob([result.blob()], {
                    type: 'application/zip'
                });
                const objectUrl: string = URL.createObjectURL(blob);

                const tagAnchor: HTMLAnchorElement = document.createElement('a') as HTMLAnchorElement;
                tagAnchor.href = objectUrl;
                tagAnchor.download = fileName ?
                    `${fileName.substring(0, fileName.lastIndexOf('.'))}.zip` :
                    `${id}_${this.datePipe.transform(Date.now(), 'yyyyMMdd')}.zip`;
                document.body.appendChild(tagAnchor);
                tagAnchor.click();

                document.body.removeChild(tagAnchor);
                URL.revokeObjectURL(objectUrl);
            });
    }


    clearUploadFile(): void {
        this.ketQuaThamDinhInput = 0;
        this.listFileNameUpload = [];
        this.listFileNameDelete = [];
        this.totalFileUpload = 0;
        this.createFileExist = false;
        this.drpzone.directiveRef.dropzone().removeAllFiles();
    }
}