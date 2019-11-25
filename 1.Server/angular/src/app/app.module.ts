import { NgModule, LOCALE_ID } from '@angular/core';
import { CommonModule, registerLocaleData, DatePipe } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { JsonpModule, HttpModule } from '@angular/http';
import { HttpClientModule } from '@angular/common/http';

import { ModalModule } from 'ngx-bootstrap';
import { NgxPaginationModule } from 'ngx-pagination';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { AbpModule } from '@abp/abp.module';

import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { SharedModule } from '@shared/shared.module';

import { UtilsModule } from '@shared/utils/utils.module';

import { HomeComponent } from '@app/home/home.component';
import { AboutComponent } from '@app/about/about.component';
import { TopBarComponent } from '@app/layout/topbar.component';
import { TopBarLanguageSwitchComponent } from '@app/layout/topbar-languageswitch.component';
import { SideBarUserAreaComponent } from '@app/layout/sidebar-user-area.component';
import { SideBarNavComponent } from '@app/layout/sidebar-nav.component';
import { SideBarFooterComponent } from '@app/layout/sidebar-footer.component';
import { RightSideBarComponent } from '@app/layout/right-sidebar.component';

import { Ng2ImgMaxModule } from 'ng2-img-max';
import { FileUploadModule } from 'ng2-file-upload';
import { EditorModule } from 'primeng/editor';

// accounts
import { UpdateInfoComponent } from '../account/update-info/update-info.component'
// roles
import { RolesComponent } from '@app/roles/roles.component';
import { CreateRoleDialogComponent } from './roles/create-role/create-role-dialog.component';
import { EditRoleDialogComponent } from './roles/edit-role/edit-role-dialog.component';
// users
import { UsersComponent } from '@app/users/users.component';
import { CreateUserDialogComponent } from '@app/users/create-user/create-user-dialog.component';
import { EditUserDialogComponent } from '@app/users/edit-user/edit-user-dialog.component';
import { ChangePasswordComponent } from './users/change-password/change-password.component';
import { ResetPasswordDialogComponent } from './users/reset-password/reset-password.component';
// Tram dịch vụ
import { TramDichVuComponent } from '@app/tramdichvu/tramdichvu.component';
import { CreateTramDichVuDialogComponent } from './tramdichvu/create-tram/create-tram-dialog.component';
import { EditTramDichVuDialogComponent } from './tramdichvu/edit-tram/edit-tram-dialog.component';
// Cong viec
import { CongViecComponent } from '@app/congviec/congviec.component';
import { CreateCongViecDialogComponent } from './congviec/create-congviec/create-congviec-dialog.component';
import { EditCongViecDialogComponent } from './congviec/edit-congviec/edit-congviec-dialog.component';
import { PhanBoVeTramDialogComponent } from './congviec/phanbo-congviec/phanbo-vetram-dialog.component';
import { PhanBoVeNhanVienDialogComponent } from './congviec/phanbo-congviec/phanbo-venhanvien-dialog.component';
import { HuyBoCongViecDialogComponent } from './congviec/huybo-congviec/huybo-congviec-dialog.component';
import { ThuHoiCongViecDialogComponent } from './congviec/thuhoi-congviec/thuhoi-congviec-dialog.component';
import { ImportCongViecDialogComponent } from './congviec/import-congviec/import-congviec-dialog.component';
// Khach hang
import { KhachHangComponent } from '@app/khachhang/khachhang.component';
import { CreateKhachHangDialogComponent } from './khachhang/create-khachhang/create-khachhang-dialog.component';
import { EditKhachHangDialogComponent } from './khachhang/edit-khachhang/edit-khachhang-dialog.component';
import { SetPasswordDialogComponent } from './khachhang/set-password/set-password.component';
import { ImportKhachHangDialogComponent } from './khachhang/import-khachhang/import-khachhang-dialog.component';
// Danh mục nhóm dịch vụ
import { DanhMucNhomDichVuComponent } from '@app/danhmuc/nhomdichvu/nhomdichvu.component';
import { CreateNhomDichVuDialogComponent } from './danhmuc/nhomdichvu/create-nhomdichvu/create-nhomdichvu-dialog.component';
import { EditNhomDichVuDialogComponent } from './danhmuc/nhomdichvu/edit-nhomdichvu/edit-nhomdichvu-dialog.component';
// Danh mục dịch vụ
import { DanhMucDichVuComponent } from '@app/danhmuc/dichvu/dichvu.component';
import { CreateDichVuDialogComponent } from './danhmuc/dichvu/create-dichvu/create-dichvu-dialog.component';
import { EditDichVuDialogComponent } from './danhmuc/dichvu/edit-dichvu/edit-dichvu-dialog.component';
// Danh mục hạng mục
import { DanhMucHangMucComponent } from '@app/danhmuc/hangmuc/hangmuc.component';
import { CreateHangMucDialogComponent } from './danhmuc/hangmuc/create-hangmuc/create-hangmuc-dialog.component';
import { EditHangMucDialogComponent } from './danhmuc/hangmuc/edit-hangmuc/edit-hangmuc-dialog.component';
// Danh mục hãng
import { DanhMucHangComponent } from '@app/danhmuc/hang/hang.component';
import { CreateHangDialogComponent } from './danhmuc/hang/create-hang/create-hang-dialog.component';
import { EditHangDialogComponent } from './danhmuc/hang/edit-hang/edit-hang-dialog.component';

// Hệ thống
// Khuyến mãi
import { KhuyenMaiComponent } from '@app/hethong/khuyenmai/khuyenmai.component';
import { CreateKhuyenMaiDialogComponent } from './hethong/khuyenmai/create-khuyenmai/create-khuyenmai-dialog.component';
import { EditKhuyenMaiDialogComponent } from './hethong/khuyenmai/edit-khuyenmai/edit-khuyenmai-dialog.component';
// Điều khoản sử dụng
import { DieuKhoanSuDungComponent } from '@app/hethong/dieukhoansudung/dieukhoansudung.component';
// Thông tin hệ thống
import { ThongTinHeThongComponent } from '@app/hethong/thongtinhethong/thongtinhethong.component';

import localeGB from '@angular/common/locales/vi';
import { DinhkemComponent } from './congviec/DinhKem/dinhkem.component';
import { DropzoneModule } from 'ngx-dropzone-wrapper';
import { DROPZONE_CONFIG } from 'ngx-dropzone-wrapper';
import { DropzoneConfigInterface } from 'ngx-dropzone-wrapper';
import { DinhkemSampleChitietComponent } from './congviec/DinhKem/dinhkem-chitiet.component';
registerLocaleData(localeGB);

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent,
        AboutComponent,
        TopBarComponent,
        TopBarLanguageSwitchComponent,
        SideBarUserAreaComponent,
        SideBarNavComponent,
        SideBarFooterComponent,
        RightSideBarComponent,
        // accounts
        UpdateInfoComponent,
        // roles
        RolesComponent,
        CreateRoleDialogComponent,
        EditRoleDialogComponent,
        // users
        UsersComponent,
        CreateUserDialogComponent,
        EditUserDialogComponent,
        ChangePasswordComponent,
        ResetPasswordDialogComponent,
        // Trạm dịch vụ
        TramDichVuComponent,
        CreateTramDichVuDialogComponent,
        EditTramDichVuDialogComponent,
        // Công việc
        CongViecComponent,
        CreateCongViecDialogComponent,
        EditCongViecDialogComponent,
        PhanBoVeTramDialogComponent,
        PhanBoVeNhanVienDialogComponent,
        HuyBoCongViecDialogComponent,
        ThuHoiCongViecDialogComponent,
        ImportCongViecDialogComponent,
        // Khách hàng
        KhachHangComponent,
        CreateKhachHangDialogComponent,
        EditKhachHangDialogComponent,
        SetPasswordDialogComponent,
        ImportKhachHangDialogComponent,
        // Danh mục nhóm dịch vụ
        DanhMucNhomDichVuComponent,
        CreateNhomDichVuDialogComponent,
        EditNhomDichVuDialogComponent,
        // Danh mục dịch vụ
        DanhMucDichVuComponent,
        CreateDichVuDialogComponent,
        EditDichVuDialogComponent,
        // Danh mục hạng mục
        DanhMucHangMucComponent,
        CreateHangMucDialogComponent,
        EditHangMucDialogComponent,
        // Danh mục hãng
        DanhMucHangComponent,
        CreateHangDialogComponent,
        EditHangDialogComponent,

        // Khuyến mãi
        KhuyenMaiComponent,
        CreateKhuyenMaiDialogComponent,
        EditKhuyenMaiDialogComponent,
        // Điều khoản sử dụng
        DieuKhoanSuDungComponent,
        
        DinhkemComponent,

        DinhkemSampleChitietComponent,
        // Thông tin hệ thống
        ThongTinHeThongComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        HttpClientModule,
        JsonpModule,
        ModalModule.forRoot(),
        AbpModule,
        AppRoutingModule,
        ServiceProxyModule,
        SharedModule,
        NgxPaginationModule,
        Ng2ImgMaxModule,
        FileUploadModule,
        UtilsModule,
        EditorModule,
        HttpModule,
        DropzoneModule
    ],
    providers: [
        { provide: [LOCALE_ID, DROPZONE_CONFIG], useValue: ["vi", "DEFAULT_DROPZONE_CONFIG"],},
        DatePipe
        
    ],
    entryComponents: [
        // roles
        CreateRoleDialogComponent,
        EditRoleDialogComponent,
        // users
        CreateUserDialogComponent,
        EditUserDialogComponent,
        ResetPasswordDialogComponent,
        // Trạm dịch vụ
        CreateTramDichVuDialogComponent,
        EditTramDichVuDialogComponent,
        // Công việc
        CreateCongViecDialogComponent,
        EditCongViecDialogComponent,
        PhanBoVeTramDialogComponent,
        PhanBoVeNhanVienDialogComponent,
        HuyBoCongViecDialogComponent,
        ThuHoiCongViecDialogComponent,
        ImportCongViecDialogComponent,
        // Khách hàng
        CreateKhachHangDialogComponent,
        EditKhachHangDialogComponent,
        SetPasswordDialogComponent,
        ImportKhachHangDialogComponent,
        // Danh mục nhóm dịch vụ
        CreateNhomDichVuDialogComponent,
        EditNhomDichVuDialogComponent,
        // Danh mục dịch vụ
        CreateDichVuDialogComponent,
        EditDichVuDialogComponent,
        // Danh mục hạng mục
        CreateHangMucDialogComponent,
        EditHangMucDialogComponent,
        // Danh mục hãng
        CreateHangDialogComponent,
        EditHangDialogComponent,
        // Khuyến mãi
        CreateKhuyenMaiDialogComponent,
        EditKhuyenMaiDialogComponent
    ]
})
export class AppModule { }
