import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { HomeComponent } from './home/home.component';
import { AboutComponent } from './about/about.component';
import { UsersComponent } from './users/users.component';
import { RolesComponent } from 'app/roles/roles.component';
import { ChangePasswordComponent } from './users/change-password/change-password.component';
import { UpdateInfoComponent } from '../account/update-info/update-info.component';
import { TramDichVuComponent } from './tramdichvu/tramdichvu.component';
import { CongViecComponent } from './congviec/congviec.component';
import { KhachHangComponent } from './khachhang/khachhang.component';
import { DanhMucNhomDichVuComponent } from './danhmuc/nhomdichvu/nhomdichvu.component';
import { DanhMucDichVuComponent } from './danhmuc/dichvu/dichvu.component';
import { DanhMucHangMucComponent } from './danhmuc/hangmuc/hangmuc.component';
import { DanhMucHangComponent } from './danhmuc/hang/hang.component';
import { KhuyenMaiComponent } from './hethong/khuyenmai/khuyenmai.component';
import { DieuKhoanSuDungComponent } from './hethong/dieukhoansudung/dieukhoansudung.component';
import { ThongTinHeThongComponent } from './hethong/thongtinhethong/thongtinhethong.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    { path: 'home', component: HomeComponent,  canActivate: [AppRouteGuard] },
                    { path: 'congviec', component: CongViecComponent,  canActivate: [AppRouteGuard] },
                    { path: 'tramdichvu', component: TramDichVuComponent,  canActivate: [AppRouteGuard] },
                    { path: 'khachhang', component: KhachHangComponent,  canActivate: [AppRouteGuard] },
                    { path: 'danhmuc/nhomdichvu', component: DanhMucNhomDichVuComponent,  canActivate: [AppRouteGuard] },
                    { path: 'danhmuc/dichvu', component: DanhMucDichVuComponent,  canActivate: [AppRouteGuard] },
                    { path: 'danhmuc/hangmuc', component: DanhMucHangMucComponent,  canActivate: [AppRouteGuard] },
                    { path: 'danhmuc/hang', component: DanhMucHangComponent,  canActivate: [AppRouteGuard] },
                    { path: 'hethong/khuyenmai', component: KhuyenMaiComponent,  canActivate: [AppRouteGuard] },
                    { path: 'hethong/dieukhoansudung', component: DieuKhoanSuDungComponent,  canActivate: [AppRouteGuard] },
                    { path: 'hethong/thongtinhethong', component: ThongTinHeThongComponent,  canActivate: [AppRouteGuard] },
                    { path: 'users', component: UsersComponent, data: { permission: 'Pages.Users' }, canActivate: [AppRouteGuard] },
                    { path: 'roles', component: RolesComponent, data: { permission: 'Pages.Roles' }, canActivate: [AppRouteGuard] },
                    { path: 'about', component: AboutComponent },
                    { path: 'update-password', component: ChangePasswordComponent },
                    { path: 'update-info', component: UpdateInfoComponent }
                ]
            }
        ])
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
