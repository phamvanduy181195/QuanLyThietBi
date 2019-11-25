import { Component, Injector } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Ng2ImgMaxService } from 'ng2-img-max';
import { AppComponentBase } from '@shared/app-component-base';
import { AccountServiceProxy, UpdateInfoDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { Router } from '@angular/router';

@Component({
    animations: [appModuleAnimation()],
    templateUrl: './update-info.component.html',
    styleUrls: ['./update-info.component.css'],
})

export class UpdateInfoComponent extends AppComponentBase {
    saving = false;
    updateInfo: UpdateInfoDto = new UpdateInfoDto();
    tramDichVuName = "";
    
    public constructor(
        injector: Injector,
        private _ng2ImgMaxService: Ng2ImgMaxService,
        private _accountService: AccountServiceProxy,
        private router: Router
    ) {
        super(injector);

        this._accountService.getInfo().subscribe(result => {
            this.updateInfo.name = result.name;
            this.updateInfo.phoneNumber = result.phoneNumber;
            this.updateInfo.emailAddress = result.emailAddress;
            this.updateInfo.description = result.description;
            this.updateInfo.profilePicture = result.profilePicture;
            this.tramDichVuName = result.tramDichVu;
        });
    }

    public save() {
        this.saving = true;

        this._accountService.updateInfo(this.updateInfo)
            .pipe(
                finalize(() => {
                    this.saving = false;
                })
            ).subscribe(result => {
                if (result == "OK")
                {
                    abp.message.success(this.l('InfoChangedSuccessfully'), this.l('Success'));
                    this.router.navigate(['/']);
                }
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
            this._ng2ImgMaxService.resizeImage(file, 200, 200).subscribe(
                result => {
                    reader.readAsBinaryString(result);
                }
            );
        }
    }

    handleReaderLoaded(readerEvt: any) {
        var binaryString = readerEvt.target.result;
        this.updateInfo.profilePicture = btoa(binaryString);
    }
}
