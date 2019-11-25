import { Component, Injector } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { HeThongServiceProxy, CreateDieuKhoanSuDungDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
    templateUrl: './dieukhoansudung.component.html',
    animations: [appModuleAnimation()]
})

export class DieuKhoanSuDungComponent extends AppComponentBase {
    dieuKhoanSuDung = '';
    dieuKhoanHienTai = '';

    constructor(
        injector: Injector,
        private _heThongService: HeThongServiceProxy,
    ) {
        super(injector);

        this._heThongService.getDieuKhoanSuDung().subscribe(result => {
            this.dieuKhoanSuDung = result;
            this.dieuKhoanHienTai = result;
        });
    }

    create(): void {
        this.dieuKhoanHienTai = this.dieuKhoanSuDung;
        
        this._heThongService.createDieuKhoanSuDung(new CreateDieuKhoanSuDungDto({noiDung: this.dieuKhoanSuDung})).subscribe(() => {
            this.notify.info(this.l('SavedSuccessfully'));
        });
    }
}
