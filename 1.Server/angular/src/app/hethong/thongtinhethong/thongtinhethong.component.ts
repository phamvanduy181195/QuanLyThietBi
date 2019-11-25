import { Component, Injector } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { HeThongServiceProxy, ThongTinHeThongDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
    templateUrl: './thongtinhethong.component.html',
    animations: [appModuleAnimation()]
})

export class ThongTinHeThongComponent extends AppComponentBase {
    thongTinHeThong: ThongTinHeThongDto = new ThongTinHeThongDto();
    thongTinHienTai: ThongTinHeThongDto = new ThongTinHeThongDto();

    constructor(
        injector: Injector,
        private _heThongService: HeThongServiceProxy,
    ) {
        super(injector);

        this._heThongService.getThongTinHeThong().subscribe(result => {
            this.thongTinHeThong = result;

            this.updateThongTinHienTai(result);
        });
    }

    create(): void {
        this.updateThongTinHienTai(this.thongTinHeThong);
        
        this._heThongService.createThongTinHeThong(this.thongTinHeThong).subscribe(() => {
            this.notify.info(this.l('SavedSuccessfully'));
        });
    }

    private updateThongTinHienTai(thongTinHeThong: ThongTinHeThongDto): void {
        this.thongTinHienTai.soHotLine = thongTinHeThong.soHotLine;
        this.thongTinHienTai.email = thongTinHeThong.email;
        this.thongTinHienTai.facebook = thongTinHeThong.facebook;
        this.thongTinHienTai.diaChi = thongTinHeThong.diaChi;
    }
}
