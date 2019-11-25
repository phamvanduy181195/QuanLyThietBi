import { Component, Injector, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { MenuItem } from '@shared/layout/menu-item';

@Component({
    templateUrl: './sidebar-nav.component.html',
    selector: 'sidebar-nav',
    encapsulation: ViewEncapsulation.None
})
export class SideBarNavComponent extends AppComponentBase {

    menuItems: MenuItem[] = [
        new MenuItem(this.l('HomePage'), '', 'home', '/app/home'),

        new MenuItem(this.l('mnuQuanLyCongViec'), 'Pages.QuanLyCongViec', 'build', '/app/congviec'),
        new MenuItem(this.l('TramDichVu'), 'Pages.TramDichVu', 'store', '/app/tramdichvu'),

        // new MenuItem(this.l('Tenants'), 'Pages.Tenants', 'business', '/app/tenants'),
        new MenuItem(this.l('mnuNhanVien'), 'Pages.Users', 'people', '/app/users'),
        new MenuItem(this.l('mnuKhachHang'), 'Pages.KhachHang', 'perm_contact_calendar', '/app/khachhang'),
        new MenuItem(this.l('Roles'), 'Pages.Roles', 'local_offer', '/app/roles'),
        // new MenuItem(this.l('About'), '', 'info', '/app/about'),

        new MenuItem(this.l('DanhMuc'), '', 'menu', '', [
            new MenuItem(this.l('DanhMucNhomDichVu'), 'Pages.DanhMucNhomDichVu', '', '/app/danhmuc/nhomdichvu'),
            new MenuItem(this.l('DanhMucDichVu'), 'Pages.DanhMucDichVu', '', '/app/danhmuc/dichvu'),
            new MenuItem(this.l('DanhMucHangMuc'), 'Pages.DanhMucHangMuc', '', '/app/danhmuc/hangmuc'),
            new MenuItem(this.l('DanhMucHang'), 'Pages.DanhMucHang', '', '/app/danhmuc/hang'),
        ]),

        new MenuItem(this.l('HeThong'), '', 'menu', '', [
            new MenuItem(this.l('KhuyenMai'), 'Pages.QuanLyKhuyenMai', '', '/app/hethong/khuyenmai'),
            new MenuItem(this.l('DieuKhoanSuDung'), 'Pages.QuanLyHeThong', '', '/app/hethong/dieukhoansudung'),
            new MenuItem(this.l('ThongTinHeThong'), 'Pages.QuanLyHeThong', '', '/app/hethong/thongtinhethong'),
        ])
    ];

    constructor(
        injector: Injector
    ) {
        super(injector);
    }

    showMenuItem(menuItem): boolean {
        if (menuItem.permissionName) {
            return this.permission.isGranted(menuItem.permissionName);
        }

        return true;
    }
}
